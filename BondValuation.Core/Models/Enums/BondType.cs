namespace BondValuation.Core.Models.Enums
{
    /// <summary>
    /// Represents the type of bond instrument.
    /// </summary>
    public enum BondType
    {
        /// <summary>
        /// Standard fixed-rate bond.
        /// </summary>
        Bond,

        /// <summary>
        /// Bond with principal or coupon linked to inflation index.
        /// </summary>
        InflationLinked,

        /// <summary>
        /// Zero-coupon bond that pays no periodic interest.
        /// </summary>
        ZeroCoupon
    }
}
