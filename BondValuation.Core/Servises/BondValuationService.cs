using BondValuation.Core.Models;
using BondValuation.Core.Models.Enums;

namespace BondValuation.Core.Servises
{
    public interface IBondValuationService
    {
        ValuationResult CalculateBondValuation(BondRecord record);
    }

    public class BondValuationService : IBondValuationService
    {
        public ValuationResult CalculateBondValuation(BondRecord record)
        {
            decimal presentValue = record.Type switch
            {
                BondType.Bond => CalculateFixedBondValue(record),
                BondType.InflationLinked => CalculateInflationLinkedBondValue(record),
                BondType.ZeroCoupon => CalculateZeroCouponBondValue(record),
                _ => 0m
            };

            return new ValuationResult
            {
                BondId = record.BondId,
                Type = record.Type,
                PresentValue = presentValue,
                Notes = record.Type == BondType.InflationLinked
                    ? "Approximate value - actual value depends on realized inflation rates"
                    : null
            };
        }

        private static decimal CalculateFixedBondValue(BondRecord record)
        {
            decimal periodicCouponRate = record.GetPeriodicCouponRate();
            int totalPeriods = record.GetTotalPaymentPeriods();


            return (decimal)(Math.Pow((double)(1 + periodicCouponRate), totalPeriods) * (double)record.FaceValue) * record.DiscountFactor;
        }

        private static decimal CalculateInflationLinkedBondValue(BondRecord record)
        {
            // Similar to fixed bond but with inflation adjustment
            // For simplicity, using the same calculation as fixed bond
            // In practice, would need actual inflation data
            return CalculateFixedBondValue(record);
        }

        private static decimal CalculateZeroCouponBondValue(BondRecord record)
        {
            return (decimal)(Math.Pow((double)(1 + record.Rate.EffectiveRate), record.YearsToMaturity) * (double)record.FaceValue) * record.DiscountFactor;
        }
    }
}
