namespace BondValuation.Core.Models.Enums
{
    /// <summary>
    /// Represents the frequency of coupon payments.
    /// </summary>
    public enum PaymentFrequency
    {        
        /// <summary>
        /// None payment.
        /// </summary>
        None = 0,

        /// <summary>
        /// Annual payment (once per year).
        /// </summary>
        Annual = 1,

        /// <summary>
        /// Semi-annual payment (twice per year).
        /// </summary>
        SemiAnnual = 2,

        /// <summary>
        /// Quarterly payment (four times per year).
        /// </summary>
        Quarterly = 4,
    }
}
