using System;
using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner.Data;

/// <summary>
/// In-memory stand-in for the database-backed RebateDataStore, so the runner can
/// demonstrate real calculations. RebateService is unaware of the difference.
/// </summary>
public class InMemoryRebateDataStore : IRebateDataStore
{
    private readonly Dictionary<string, Rebate> _rebates;
    private readonly List<RebateCalculation> _calculations = new();

    public InMemoryRebateDataStore(IEnumerable<Rebate> rebates)
    {
        _rebates = rebates.ToDictionary(rebate => rebate.Identifier, StringComparer.OrdinalIgnoreCase);
    }

    public IEnumerable<Rebate> Rebates => _rebates.Values;

    public IReadOnlyList<RebateCalculation> Calculations => _calculations;

    public Rebate GetRebate(string rebateIdentifier) => _rebates.GetValueOrDefault(rebateIdentifier);

    public void StoreCalculationResult(Rebate rebate, decimal rebateAmount)
    {
        _calculations.Add(new RebateCalculation
        {
            Id = _calculations.Count + 1,
            Identifier = Guid.NewGuid().ToString(),
            RebateIdentifier = rebate.Identifier,
            IncentiveType = rebate.Incentive,
            Amount = rebateAmount,
        });
    }
}
