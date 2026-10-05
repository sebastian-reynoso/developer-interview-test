using Smartwyre.DeveloperTest.Calculators;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Calculators;

public class FixedRateRebateCalculatorTests
{
    private readonly FixedRateRebateCalculator _calculator = new();

    [Fact]
    public void HandlesFixedRateRebateIncentive()
    {
        Assert.Equal(IncentiveType.FixedRateRebate, _calculator.IncentiveType);
        Assert.Equal(SupportedIncentiveType.FixedRateRebate, _calculator.RequiredProductSupport);
    }

    [Fact]
    public void TryCalculate_ReturnsPriceTimesPercentageTimesVolume()
    {
        var rebate = new Rebate { Percentage = 0.1m };
        var product = new Product { Price = 200m };
        var request = new CalculateRebateRequest { Volume = 3m };

        var success = _calculator.TryCalculate(rebate, product, request, out var rebateAmount);

        Assert.True(success);
        Assert.Equal(60m, rebateAmount); // 200 * 0.1 * 3
    }

    [Theory]
    [InlineData(0, 200, 3)] // no percentage
    [InlineData(0.1, 0, 3)] // no price
    [InlineData(0.1, 200, 0)] // no volume
    public void TryCalculate_Fails_WhenAnyInputIsZero(decimal percentage, decimal price, decimal volume)
    {
        var rebate = new Rebate { Percentage = percentage };
        var product = new Product { Price = price };
        var request = new CalculateRebateRequest { Volume = volume };

        var success = _calculator.TryCalculate(rebate, product, request, out var rebateAmount);

        Assert.False(success);
        Assert.Equal(0m, rebateAmount);
    }
}
