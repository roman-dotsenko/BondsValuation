using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BondValuationApi.Services
{
    /// <summary>
    /// Service for interacting with Azure Blob Storage.
    /// </summary>
    public interface IBlobStorageService
    {
        /// <summary>
        /// Uploads a file to the input folder in blob storage.
        /// </summary>
        Task<string> UploadFileAsync(Stream fileStream, string fileName);

        /// <summary>
        /// Retrieves the last N valuation result files from the output folder.
        /// </summary>
        Task<List<BlobFileInfo>> GetLastValuationFilesAsync(int count = 5);

        /// <summary>
        /// Downloads a blob file content as a stream.
        /// </summary>
        Task<Stream> DownloadFileAsync(string blobName);
    }

    /// <summary>
    /// Implementation of blob storage service.
    /// </summary>
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobStorageOptions _options;
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(BlobServiceClient blobServiceClient,
                                  BlobStorageOptions options,
                                  ILogger<BlobStorageService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _options = options;
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_options.ContainerName);
                await containerClient.CreateIfNotExistsAsync();

                var blobName = $"{_options.InputFolder}/{fileName}";
                var blobClient = containerClient.GetBlobClient(blobName);

                await blobClient.UploadAsync(fileStream, overwrite: true);

                _logger.LogInformation($"File uploaded successfully: {blobName}");
                return blobName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading file: {fileName}");
                throw;
            }
        }

        public async Task<List<BlobFileInfo>> GetLastValuationFilesAsync(int count = 5)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_options.ContainerName);

                if (!await containerClient.ExistsAsync())
                {
                    _logger.LogWarning($"Container {_options.ContainerName} does not exist");
                    return new List<BlobFileInfo>();
                }

                var blobs = new List<BlobFileInfo>();

                await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: _options.OutputFolder))
                {
                    if (blobItem.Properties.LastModified.HasValue)
                    {
                        blobs.Add(new BlobFileInfo
                        {
                            Name = blobItem.Name,
                            LastModified = blobItem.Properties.LastModified.Value.DateTime,
                            Size = blobItem.Properties.ContentLength ?? 0
                        });
                    }
                }

                // Sort by last modified descending and take the requested count
                var result = blobs
                    .OrderByDescending(b => b.LastModified)
                    .Take(count)
                    .ToList();

                _logger.LogInformation($"Retrieved {result.Count} valuation files from blob storage");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving valuation files");
                throw;
            }
        }

        public async Task<Stream> DownloadFileAsync(string blobName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_options.ContainerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                var response = await blobClient.DownloadAsync();
                _logger.LogInformation($"File downloaded successfully: {blobName}");

                return response.Value.Content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading file: {blobName}");
                throw;
            }
        }
    }

    /// <summary>
    /// Information about a blob file.
    /// </summary>
    public class BlobFileInfo
    {
        public required string Name { get; set; }
        public DateTime LastModified { get; set; }
        public long Size { get; set; }
    }
}
