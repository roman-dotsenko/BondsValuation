namespace BondValuation.Core.Models
{
    /// <summary>
    /// Represents the interest rate structure of a bond, supporting both fixed and inflation-linked rates.
    /// </summary>
    public class RateInfo
    {
        /// <summary>
        /// The base rate as a decimal (e.g., 0.031 for 3.1%).
        /// </summary>
        public decimal BaseRate { get; set; }

        /// <summary>
        /// Indicates whether the rate is linked to inflation.
        /// </summary>
        public bool IsInflationLinked { get; set; }

        /// <summary>
        /// The spread over inflation, if applicable (e.g., 0.0092 for +0.92%).
        /// </summary>
        public decimal? InflationSpread { get; set; }

        /// <summary>
        /// The original rate string from the CSV for reference.
        /// </summary>
        public string OriginalRateString { get; set; } = string.Empty;

        /// <summary>
        /// Gets the effective rate for calculation purposes.
        /// For inflation-linked bonds, this returns the spread; for fixed bonds, the base rate.
        /// </summary>
        public decimal EffectiveRate => IsInflationLinked ? (InflationSpread ?? 0m) : BaseRate;

        /// <summary>
        /// Parses a rate string from CSV format (e.g., "3.10%" or "Inflation+0.92%").
        /// </summary>
        public static RateInfo Parse(string rateString)
        {
            RateInfo rateInfo = new() { OriginalRateString = rateString };

            if (string.IsNullOrWhiteSpace(rateString))
            {
                return rateInfo;
            }

            string trimmedRate = rateString.Trim();

            if (trimmedRate.StartsWith("Inflation", StringComparison.OrdinalIgnoreCase))
            {
                rateInfo.IsInflationLinked = true;

                // Parse the spread after "Inflation+" or "Inflation-"
                string[] parts = trimmedRate.Split(new[] { '+', '-' }, 2);
                if (parts.Length == 2)
                {
                    string spreadString = parts[1].Replace("%", "").Trim();
                    if (decimal.TryParse(spreadString, System.Globalization.NumberStyles.Any,
                      System.Globalization.CultureInfo.InvariantCulture, out decimal spread))
                    {
                        rateInfo.InflationSpread = trimmedRate.Contains('-') ? -spread / 100m : spread / 100m;
                        rateInfo.BaseRate = spread / 100m;
                    }
                }
            }
            else
            {
                // Fixed rate bond
                rateInfo.IsInflationLinked = false;
                string numericString = trimmedRate.Replace("%", "").Trim();

                if (decimal.TryParse(numericString, System.Globalization.NumberStyles.Any,
                          System.Globalization.CultureInfo.InvariantCulture, out decimal rate))
                {
                    rateInfo.BaseRate = rate / 100m;
                }
            }

            return rateInfo;
        }

        public override string ToString()
        {
            return OriginalRateString;
        }
    }
}
