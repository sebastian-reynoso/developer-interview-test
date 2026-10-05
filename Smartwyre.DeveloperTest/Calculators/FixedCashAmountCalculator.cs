using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Calculators;

/// <summary>Pays the rebate's fixed amount, regardless of price or volume.</summary>
public class FixedCashAmountCalculator : IIncentiveCalculator
{
    public IncentiveType IncentiveType => IncentiveType.FixedCashAmount;

    public SupportedIncentiveType RequiredProductSupport => SupportedIncentiveType.FixedCashAmount;

    public bool TryCalculate(Rebate rebate, Product product, CalculateRebateRequest request, out decimal rebateAmount)
    {
        rebateAmount = 0m;

        if (rebate.Amount == 0)
        {
            return false;
        }

        rebateAmount = rebate.Amount;
        return true;
    }
}
