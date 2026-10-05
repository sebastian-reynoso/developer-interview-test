using System;
using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Calculators;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

/// <summary>
/// Orchestrates a rebate calculation: looks up the rebate and product, runs the
/// checks common to every incentive type, delegates the incentive-specific
/// calculation to the matching <see cref="IIncentiveCalculator"/> and stores the result.
/// </summary>
public class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;
    private readonly IReadOnlyDictionary<IncentiveType, IIncentiveCalculator> _calculators;

    public RebateService(
        IRebateDataStore rebateDataStore,
        IProductDataStore productDataStore,
        IEnumerable<IIncentiveCalculator> calculators)
    {
        _rebateDataStore = rebateDataStore;
        _productDataStore = productDataStore;
        // Throws if two calculators claim the same incentive type, surfacing misconfiguration at startup.
        _calculators = calculators.ToDictionary(calculator => calculator.IncentiveType);
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        Rebate rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        Product product = _productDataStore.GetProduct(request.ProductIdentifier);

        if (rebate == null || product == null)
        {
            return Failed();
        }

        if (!_calculators.TryGetValue(rebate.Incentive, out var calculator))
        {
            return Failed();
        }

        if (!product.SupportedIncentives.HasFlag(calculator.RequiredProductSupport))
        {
            return Failed();
        }

        if (!calculator.TryCalculate(rebate, product, request, out var rebateAmount))
        {
            return Failed();
        }

        _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);

        return new CalculateRebateResult { Success = true };
    }

    private static CalculateRebateResult Failed() => new() { Success = false };
}
