using System;

namespace Smartwyre.DeveloperTest.Types;

// Each value is a distinct bit, so a product can support several incentives at once
// (e.g. FixedRateRebate | AmountPerUom). [Flags] makes ToString() print the combined names.
[Flags]
public enum SupportedIncentiveType
{
    FixedRateRebate = 1 << 0,
    AmountPerUom = 1 << 1,
    FixedCashAmount = 1 << 2,
}
