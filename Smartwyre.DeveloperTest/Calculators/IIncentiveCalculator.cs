using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Calculators;

/// <summary>
/// Strategy for calculating a rebate amount for a single incentive type.
/// To support a new incentive type, add an implementation of this interface
/// and register it; RebateService does not need to change.
/// </summary>
public interface IIncentiveCalculator
{
    /// <summary>The rebate incentive type this calculator handles.</summary>
    IncentiveType IncentiveType { get; }

    /// <summary>The flag a product must have in SupportedIncentives to be eligible.</summary>
    SupportedIncentiveType RequiredProductSupport { get; }

    /// <summary>
    /// Validates the incentive-specific inputs and calculates the rebate amount.
    /// Returns false when the inputs are not valid for this incentive type.
    /// </summary>
    bool TryCalculate(Rebate rebate, Product product, CalculateRebateRequest request, out decimal rebateAmount);
}
