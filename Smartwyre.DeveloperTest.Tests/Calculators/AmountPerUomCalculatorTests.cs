using Smartwyre.DeveloperTest.Calculators;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Calculators;

public class AmountPerUomCalculatorTests
{
    private readonly AmountPerUomCalculator _calculator = new();

    [Fact]
    public void HandlesAmountPerUomIncentive()
    {
        Assert.Equal(IncentiveType.AmountPerUom, _calculator.IncentiveType);
        Assert.Equal(SupportedIncentiveType.AmountPerUom, _calculator.RequiredProductSupport);
    }

    [Fact]
    public void TryCalculate_ReturnsAmountTimesVolume()
    {
        var rebate = new Rebate { Amount = 2.5m };
        var request = new CalculateRebateRequest { Volume = 4m };

        var success = _calculator.TryCalculate(rebate, new Product(), request, out var rebateAmount);

        Assert.True(success);
        Assert.Equal(10m, rebateAmount); // 2.5 * 4
    }

    [Theory]
    [InlineData(0, 4)] // no amount
    [InlineData(2.5, 0)] // no volume
    public void TryCalculate_Fails_WhenAmountOrVolumeIsZero(decimal amount, decimal volume)
    {
        var rebate = new Rebate { Amount = amount };
        var request = new CalculateRebateRequest { Volume = volume };

        var success = _calculator.TryCalculate(rebate, new Product(), request, out var rebateAmount);

        Assert.False(success);
        Assert.Equal(0m, rebateAmount);
    }
}
