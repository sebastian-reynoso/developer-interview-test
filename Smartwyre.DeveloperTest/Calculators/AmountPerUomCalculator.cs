using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Calculators;

/// <summary>Pays the rebate's fixed amount for each unit of measure (UOM) of volume.</summary>
public class AmountPerUomCalculator : IIncentiveCalculator
{
    public IncentiveType IncentiveType => IncentiveType.AmountPerUom;

    public SupportedIncentiveType RequiredProductSupport => SupportedIncentiveType.AmountPerUom;

    public bool TryCalculate(Rebate rebate, Product product, CalculateRebateRequest request, out decimal rebateAmount)
    {
        rebateAmount = 0m;

        if (rebate.Amount == 0 || request.Volume == 0)
        {
            return false;
        }

        rebateAmount = rebate.Amount * request.Volume;
        return true;
    }
}
