using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner.Data;

/// <summary>Sample rebates and products covering each incentive type and an unsupported combination.</summary>
public static class SampleData
{
    public static Rebate[] Rebates =>
    [
        new() { Identifier = "REB-CASH", Incentive = IncentiveType.FixedCashAmount, Amount = 50m },
        new() { Identifier = "REB-RATE", Incentive = IncentiveType.FixedRateRebate, Percentage = 0.1m },
        new() { Identifier = "REB-UOM", Incentive = IncentiveType.AmountPerUom, Amount = 2m },
    ];

    public static Product[] Products =>
    [
        new()
        {
            Id = 1,
            Identifier = "PROD-ALL",
            Price = 100m,
            Uom = "kg",
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate
                | SupportedIncentiveType.AmountPerUom
                | SupportedIncentiveType.FixedCashAmount,
        },
        new()
        {
            Id = 2,
            Identifier = "PROD-RATE",
            Price = 25m,
            Uom = "unit",
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate,
        },
    ];
}
