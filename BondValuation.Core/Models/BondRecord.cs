using BondValuation.Core.Models.Enums;

namespace BondValuation.Core.Models
{
    /// <summary>
    /// Represents a bond record with all relevant characteristics for valuation.
    /// </summary>
    public class BondRecord
    {
        /// <summary>
        /// Unique identifier for the bond.
        /// </summary>
        public required string BondId { get; set; }

        /// <summary>
        /// Name of the bond issuer.
        /// </summary>
        public required string Issuer { get; set; }
        
        /// <summary>
        /// Interest rate information, supporting both fixed and inflation-linked rates.
        /// </summary>
        public required RateInfo Rate { get; set; }
        
        /// <summary>
        /// Face value (par value) of the bond.
        /// </summary>
        public decimal FaceValue { get; set; }
        
        /// <summary>
        /// Frequency of coupon payments.
        /// </summary>
        public PaymentFrequency PaymentFrequency { get; set; }
        
        /// <summary>
        /// Credit rating of the bond.
        /// </summary>
        public CreditRating Rating { get; set; }
        
        /// <summary>
        /// Type of bond instrument.
        /// </summary>
        public BondType Type { get; set; }
     
        /// <summary>
        /// Years remaining until maturity.
        /// </summary>
        public double YearsToMaturity { get; set; }
  
        /// <summary>
        /// Discount factor for present value calculations.
        /// </summary>
        public decimal DiscountFactor { get; set; }
        
        /// <summary>
        /// Optional notes from the trading desk.
        /// </summary>
        public string? DeskNotes { get; set; }

        /// <summary>
        /// Calculates the number of payment periods based on payment frequency and years to maturity.
        /// </summary>
        public int GetTotalPaymentPeriods() => PaymentFrequency == PaymentFrequency.None ? 0 : (int)Math.Ceiling(YearsToMaturity * (int)PaymentFrequency);

        /// <summary>
        /// Gets the periodic coupon rate.
        /// </summary>
        public decimal GetPeriodicCouponRate() => PaymentFrequency == PaymentFrequency.None ? 0 : Rate.EffectiveRate / (int)PaymentFrequency;
    }
}
