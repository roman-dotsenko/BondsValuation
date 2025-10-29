using BondValuation.Core.Models;
using BondValuation.Core.Models.Enums;
using CsvHelper.Configuration;

namespace BondValuation.Core.Utils.Mappings
{
    /// <summary>
    /// CsvHelper ClassMap for mapping CSV data to BondRecord objects.
    /// Handles custom parsing for enums, rates, and semicolon-delimited CSV format.
    /// </summary>
    public sealed class BondRecordMap : ClassMap<BondRecord>
    {
        public BondRecordMap()
        {
            // Map BondID column to BondId property
            Map(m => m.BondId).Name("BondID");

            // Map Issuer directly
            Map(m => m.Issuer).Name("Issuer");

            // Custom conversion for Rate string to RateInfo object
            Map(m => m.Rate)
                 .Name("Rate")
                 .Convert(args => RateInfo.Parse(args.Row.GetField<string>("Rate") ?? string.Empty));

            // Map FaceValue as decimal
            Map(m => m.FaceValue)
                .Name("FaceValue");

            // Custom conversion for PaymentFrequency string to enum
            Map(m => m.PaymentFrequency)
                .Name("PaymentFrequency")
                .Convert(args => ParsePaymentFrequency(args.Row.GetField<string>("PaymentFrequency") ?? string.Empty));

            // Custom conversion for Rating string to enum
            Map(m => m.Rating)
                .Name("Rating")
                .Convert(args => ParseCreditRating(args.Row.GetField<string>("Rating") ?? string.Empty));

            // Custom conversion for Type string to enum
            Map(m => m.Type)
                .Name("Type")
                .Convert(args => ParseBondType(args.Row.GetField<string>("Type") ?? string.Empty));

            // Map YearsToMaturity as double
            Map(m => m.YearsToMaturity)
                .Name("YearsToMaturity");

            // Map DiscountFactor as double
            Map(m => m.DiscountFactor)
                .Name("DiscountFactor");

            // Map DeskNotes (nullable)
            Map(m => m.DeskNotes)
                .Name("DeskNotes")
                .Optional();
        }

        /// <summary>
        /// Parses payment frequency string to enum.
        /// </summary>
        private static PaymentFrequency ParsePaymentFrequency(string value)
        {
            return value.Trim() switch
            {
                "Annual" => PaymentFrequency.Annual,
                "Semi-Annual" => PaymentFrequency.SemiAnnual,
                "Quarterly" => PaymentFrequency.Quarterly,
                "None" => PaymentFrequency.Quarterly,
                _ => throw new ArgumentException($"Unknown payment frequency: {value}")
            };
        }

        /// <summary>
        /// Parses credit rating string to enum.
        /// </summary>
        private static CreditRating ParseCreditRating(string value)
        {
            string normalized = value.Trim().Replace("+", "Plus").Replace("-", "Minus");

            if (Enum.TryParse<CreditRating>(normalized, true, out var rating))
            {
                return rating;
            }

            throw new ArgumentException($"Unknown credit rating: {value}");
        }

        /// <summary>
        /// Parses bond type string to enum.
        /// </summary>
        private static BondType ParseBondType(string value)
        {
            return value.Trim() switch
            {
                "Bond" => BondType.Bond,
                "Inflation-Linked" => BondType.InflationLinked,
                "Zero-Coupon" => BondType.ZeroCoupon,
                _ => throw new ArgumentException($"Unknown bond type: {value}")
            };
        }
    }
}
