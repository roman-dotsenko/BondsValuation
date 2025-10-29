namespace BondValuationApi.Services
{
    /// <summary>
    /// Configuration options for Azure Blob Storage.
    /// </summary>
    public class BlobStorageOptions
    {
        public const string SectionName = "BlobStorage";

        /// <summary>
        /// Azure Storage connection string.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Container name for bond data.
        /// </summary>
        public string ContainerName { get; set; } = "bonds";

        /// <summary>
        /// Input blob folder path.
        /// </summary>
        public string InputFolder { get; set; } = "input";

        /// <summary>
        /// Output blob folder path.
        /// </summary>
        public string OutputFolder { get; set; } = "output";
    }
}
