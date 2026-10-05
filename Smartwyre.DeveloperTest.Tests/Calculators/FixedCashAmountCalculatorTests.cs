using Smartwyre.DeveloperTest.Calculators;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Calculators;

public class FixedCashAmountCalculatorTests
{
    private readonly FixedCashAmountCalculator _calculator = new();

    [Fact]
    public void HandlesFixedCashAmountIncentive()
    {
        Assert.Equal(IncentiveType.FixedCashAmount, _calculator.IncentiveType);
        Assert.Equal(SupportedIncentiveType.FixedCashAmount, _calculator.RequiredProductSupport);
    }

    [Fact]
    public void TryCalculate_ReturnsRebateAmount_RegardlessOfPriceAndVolume()
    {
        var rebate = new Rebate { Amount = 50m };
        var product = new Product { Price = 999m };
        var request = new CalculateRebateRequest { Volume = 7m };

        var success = _calculator.TryCalculate(rebate, product, request, out var rebateAmount);

        Assert.True(success);
        Assert.Equal(50m, rebateAmount);
    }

    [Fact]
    public void TryCalculate_Fails_WhenRebateAmountIsZero()
    {
        var rebate = new Rebate { Amount = 0m };

        var success = _calculator.TryCalculate(rebate, new Product(), new CalculateRebateRequest(), out var rebateAmount);

        Assert.False(success);
        Assert.Equal(0m, rebateAmount);
    }
}
