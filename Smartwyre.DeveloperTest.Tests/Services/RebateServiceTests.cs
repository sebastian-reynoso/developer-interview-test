using System;
using Moq;
using Smartwyre.DeveloperTest.Calculators;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Services;

/// <summary>
/// Tests the orchestration in RebateService. The calculator is mocked so these tests only
/// cover lookup, shared validation, delegation and storage; each calculator's own rules
/// are covered by its own test class.
/// </summary>
public class RebateServiceTests
{
    private const string RebateId = "REB-1";
    private const string ProductId = "PROD-1";

    private readonly Mock<IRebateDataStore> _rebateDataStore = new();
    private readonly Mock<IProductDataStore> _productDataStore = new();
    private readonly Mock<IIncentiveCalculator> _calculator = new();

    private readonly Rebate _rebate = new() { Identifier = RebateId, Incentive = IncentiveType.FixedCashAmount };
    private readonly Product _product = new() { Identifier = ProductId, SupportedIncentives = SupportedIncentiveType.FixedCashAmount };
    private readonly CalculateRebateRequest _request = new() { RebateIdentifier = RebateId, ProductIdentifier = ProductId, Volume = 1m };

    public RebateServiceTests()
    {
        _rebateDataStore.Setup(store => store.GetRebate(RebateId)).Returns(_rebate);
        _productDataStore.Setup(store => store.GetProduct(ProductId)).Returns(_product);

        _calculator.Setup(calculator => calculator.IncentiveType).Returns(IncentiveType.FixedCashAmount);
        _calculator.Setup(calculator => calculator.RequiredProductSupport).Returns(SupportedIncentiveType.FixedCashAmount);
    }

    private RebateService CreateService(params IIncentiveCalculator[] calculators) =>
        new(_rebateDataStore.Object, _productDataStore.Object, calculators);

    private void CalculatorReturns(bool success, decimal amount)
    {
        _calculator
            .Setup(calculator => calculator.TryCalculate(_rebate, _product, _request, out amount))
            .Returns(success);
    }

    private void VerifyNothingStored() =>
        _rebateDataStore.Verify(
            store => store.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()),
            Times.Never);

    [Fact]
    public void Calculate_StoresAmountAndSucceeds_WhenRequestIsValid()
    {
        CalculatorReturns(success: true, amount: 42m);

        var result = CreateService(_calculator.Object).Calculate(_request);

        Assert.True(result.Success);
        _rebateDataStore.Verify(store => store.StoreCalculationResult(_rebate, 42m), Times.Once);
    }

    [Fact]
    public void Calculate_FailsWithoutStoring_WhenRebateIsNotFound()
    {
        // The original code threw a NullReferenceException in this case.
        _rebateDataStore.Setup(store => store.GetRebate(RebateId)).Returns((Rebate)null);

        var result = CreateService(_calculator.Object).Calculate(_request);

        Assert.False(result.Success);
        VerifyNothingStored();
    }

    [Fact]
    public void Calculate_FailsWithoutStoring_WhenProductIsNotFound()
    {
        // The original FixedCashAmount branch threw a NullReferenceException in this case.
        _productDataStore.Setup(store => store.GetProduct(ProductId)).Returns((Product)null);

        var result = CreateService(_calculator.Object).Calculate(_request);

        Assert.False(result.Success);
        VerifyNothingStored();
    }

    [Fact]
    public void Calculate_FailsWithoutStoring_WhenNoCalculatorHandlesTheIncentive()
    {
        _rebate.Incentive = IncentiveType.AmountPerUom;

        var result = CreateService(_calculator.Object).Calculate(_request);

        Assert.False(result.Success);
        VerifyNothingStored();
    }

    [Fact]
    public void Calculate_FailsWithoutCallingCalculator_WhenProductDoesNotSupportTheIncentive()
    {
        _product.SupportedIncentives = SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom;

        var result = CreateService(_calculator.Object).Calculate(_request);

        Assert.False(result.Success);
        decimal anyAmount;
        _calculator.Verify(
            calculator => calculator.TryCalculate(It.IsAny<Rebate>(), It.IsAny<Product>(), It.IsAny<CalculateRebateRequest>(), out anyAmount),
            Times.Never);
        VerifyNothingStored();
    }

    [Fact]
    public void Calculate_FailsWithoutStoring_WhenCalculatorRejectsTheInputs()
    {
        CalculatorReturns(success: false, amount: 0m);

        var result = CreateService(_calculator.Object).Calculate(_request);

        Assert.False(result.Success);
        VerifyNothingStored();
    }

    [Fact]
    public void Calculate_Throws_WhenRequestIsNull()
    {
        var service = CreateService(_calculator.Object);

        Assert.Throws<ArgumentNullException>(() => service.Calculate(null));
    }

    [Fact]
    public void Constructor_Throws_WhenTwoCalculatorsHandleTheSameIncentive()
    {
        Assert.Throws<ArgumentException>(() => CreateService(new FixedCashAmountCalculator(), new FixedCashAmountCalculator()));
    }

    [Fact]
    public void Calculate_UsesTheCalculatorMatchingTheRebateIncentive()
    {
        // Wires the real calculators to check the service picks the right strategy end to end.
        _rebate.Incentive = IncentiveType.FixedRateRebate;
        _rebate.Percentage = 0.1m;
        _product.Price = 200m;
        _product.SupportedIncentives = SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom;
        _request.Volume = 3m;

        var service = CreateService(new FixedCashAmountCalculator(), new FixedRateRebateCalculator(), new AmountPerUomCalculator());

        var result = service.Calculate(_request);

        Assert.True(result.Success);
        _rebateDataStore.Verify(store => store.StoreCalculationResult(_rebate, 60m), Times.Once);
    }
}
