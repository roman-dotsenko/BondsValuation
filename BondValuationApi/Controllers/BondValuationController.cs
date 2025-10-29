using BondValuation.Core.Models;
using BondValuation.Core.Utils;
using BondValuation.Core.Utils.Mappings;
using BondValuationApi.Models;
using BondValuationApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BondValuationApi.Controllers
{
    /// <summary>
    /// API controller for bond valuation operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BondValuationController : ControllerBase
    {
        private readonly ILogger<BondValuationController> _logger;
        private readonly IBlobStorageService _blobStorageService;

        public BondValuationController(
            ILogger<BondValuationController> logger,
            IBlobStorageService blobStorageService)
        {
            _logger = logger;
            _blobStorageService = blobStorageService;
        }

        /// <summary>
        /// Health check endpoint.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            _logger.LogInformation("BondValuationController GET endpoint called.");
            return Ok(new
            {
                Message = "Bond Valuation API is running.",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0"
            });
        }

        /// <summary>
        /// Retrieves the last 5 (or fewer) valuation results from blob storage.
        /// </summary>
        /// <param name="count">Number of results to retrieve (default: 5, max: 20)</param>
        /// <returns>List of valuation file information with parsed results</returns>
        [HttpGet("valuations")]
        [ProducesResponseType(typeof(List<ValuationFileResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetValuations([FromQuery] int count = 5)
        {
            try
            {
                // Limit the count to a reasonable maximum
                count = Math.Min(Math.Max(count, 1), 20);

                _logger.LogInformation($"Retrieving last {count} valuation files");

                var files = await _blobStorageService.GetLastValuationFilesAsync(count);

                var responses = new List<ValuationFileResponse>();

                foreach (var file in files)
                {
                    var response = new ValuationFileResponse
                    {
                        FileName = Path.GetFileName(file.Name),
                        LastModified = file.LastModified,
                        SizeInBytes = file.Size
                    };

                    try
                    {
                        // Download and parse the CSV file
                        using var stream = await _blobStorageService.DownloadFileAsync(file.Name);
                        var results = CsvParser.Read<ValuationResult>(
                            new StreamReader(stream),
                            delimiter: ";");

                        response.Results = results;
                        _logger.LogInformation($"Parsed {results.Count} results from {file.Name}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Could not parse results from {file.Name}");
                        // Still include the file info even if parsing fails
                    }

                    responses.Add(response);
                }

                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving valuations");
                return StatusCode(500, new ErrorResponse
                {
                    Error = "Failed to retrieve valuations",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Uploads a CSV file containing bond positions to blob storage for processing.
        /// </summary>
        /// <param name="file">CSV file with bond data</param>
        /// <returns>Upload confirmation</returns>
        [HttpPost("valuations")]
        [ProducesResponseType(typeof(UploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadBondPositions([FromForm] IFormFile file)
        {
            try
            {
                // Validate file
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Error = "No file provided or file is empty"
                    });
                }

                // Validate file extension
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (extension != ".csv")
                {
                    return BadRequest(new ErrorResponse
                    {
                        Error = "Invalid file type. Only CSV files are accepted."
                    });
                }

                // Optional: Validate file content by attempting to parse it
                try
                {
                    using var stream = file.OpenReadStream();
                    var bonds = CsvParser.Read<BondRecord, BondRecordMap>(
                        new StreamReader(stream),
                        delimiter: ";");

                    if (bonds.Count == 0)
                    {
                        return BadRequest(new ErrorResponse
                        {
                            Error = "CSV file contains no valid bond records"
                        });
                    }

                    _logger.LogInformation($"Validated CSV file with {bonds.Count} bond records");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "CSV validation failed");
                    return BadRequest(new ErrorResponse
                    {
                        Error = "Invalid CSV format",
                        Details = ex.Message
                    });
                }

                // Upload to blob storage
                using (var uploadStream = file.OpenReadStream())
                {
                    var fileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{file.FileName}";
                    var blobName = await _blobStorageService.UploadFileAsync(uploadStream, fileName);

                    _logger.LogInformation($"File uploaded successfully: {blobName}");

                    return Ok(new UploadResponse
                    {
                        Success = true,
                        Message = "File uploaded successfully and will be processed shortly",
                        BlobName = blobName,
                        UploadedAt = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading bond positions");
                return StatusCode(500, new ErrorResponse
                {
                    Error = "Failed to upload file",
                    Details = ex.Message
                });
            }
        }
    }
}
