using BondValuation.Core.Models.Enums;

namespace BondValuation.Core.Models
{
    public class ValuationResult
    {
        public required string BondId { get; set; }
        
        public required BondType Type { get; set; }
        
        public decimal PresentValue { get; set; }

        public string? Notes { get; set; }
    }
}