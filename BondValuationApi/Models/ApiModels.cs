using BondValuation.Core.Models;

namespace BondValuationApi.Models
{
    /// <summary>
    /// Response model for valuation file information.
    /// </summary>
    public class ValuationFileResponse
    {
        public required string FileName { get; set; }
        public DateTime LastModified { get; set; }
        public long SizeInBytes { get; set; }
        public List<ValuationResult>? Results { get; set; }
    }

    /// <summary>
    /// Response model for file upload.
    /// </summary>
    public class UploadResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? BlobName { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    /// <summary>
    /// Response model for API errors.
    /// </summary>
    public class ErrorResponse
    {
        public string? Error { get; set; }
        public string? Details { get; set; }
    }
}
