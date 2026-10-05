using System;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Smartwyre.DeveloperTest.Calculators;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Runner.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        var rebateDataStore = new InMemoryRebateDataStore(SampleData.Rebates);
        var productDataStore = new InMemoryProductDataStore(SampleData.Products);

        // Composition root: the only place that knows the concrete types.
        // A new incentive type only needs one more AddSingleton<IIncentiveCalculator, ...>() line.
        using var serviceProvider = new ServiceCollection()
            .AddSingleton<IRebateDataStore>(rebateDataStore)
            .AddSingleton<IProductDataStore>(productDataStore)
            .AddSingleton<IIncentiveCalculator, FixedCashAmountCalculator>()
            .AddSingleton<IIncentiveCalculator, FixedRateRebateCalculator>()
            .AddSingleton<IIncentiveCalculator, AmountPerUomCalculator>()
            .AddSingleton<IRebateService, RebateService>()
            .BuildServiceProvider();

        var rebateService = serviceProvider.GetRequiredService<IRebateService>();

        PrintSampleData(rebateDataStore, productDataStore);

        while (true)
        {
            var request = PromptRequest();
            if (request is null)
            {
                return; // Input ended.
            }

            var result = rebateService.Calculate(request);

            Console.WriteLine();
            if (result.Success)
            {
                var calculation = rebateDataStore.Calculations[^1];
                Console.WriteLine($"Success: rebate of {calculation.Amount} stored for {calculation.RebateIdentifier} ({calculation.IncentiveType}).");
            }
            else
            {
                Console.WriteLine("Failed: the rebate could not be calculated for this request.");
            }
            Console.WriteLine();

            var again = Prompt("Calculate another rebate? (y/n)");
            if (!string.Equals(again, "y", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        }
    }

    /// <summary>Asks for the request fields. Returns null if input ends.</summary>
    private static CalculateRebateRequest PromptRequest()
    {
        var rebateIdentifier = Prompt("Rebate identifier");
        if (rebateIdentifier is null)
        {
            return null;
        }

        var productIdentifier = Prompt("Product identifier");
        if (productIdentifier is null)
        {
            return null;
        }

        var volume = PromptDecimal("Volume");
        if (volume is null)
        {
            return null;
        }

        return new CalculateRebateRequest
        {
            RebateIdentifier = rebateIdentifier,
            ProductIdentifier = productIdentifier,
            Volume = volume.Value,
        };
    }

    private static void PrintSampleData(InMemoryRebateDataStore rebateDataStore, InMemoryProductDataStore productDataStore)
    {
        Console.WriteLine("Smartwyre rebate calculator");
        Console.WriteLine();
        Console.WriteLine("Available rebates:");
        foreach (var rebate in rebateDataStore.Rebates)
        {
            Console.WriteLine($"  {rebate.Identifier,-10} {rebate.Incentive,-16} Amount: {rebate.Amount,-6} Percentage: {rebate.Percentage}");
        }

        Console.WriteLine();
        Console.WriteLine("Available products:");
        foreach (var product in productDataStore.Products)
        {
            Console.WriteLine($"  {product.Identifier,-10} Price: {product.Price} per {product.Uom,-5} Supports: {product.SupportedIncentives}");
        }
        Console.WriteLine();
    }

    /// <summary>Asks until a non-empty value is entered. Returns null if input ends.</summary>
    private static string Prompt(string label)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            var input = Console.ReadLine();
            if (input is null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }
        }
    }

    /// <summary>Asks until a valid decimal is entered (using '.' as decimal separator). Returns null if input ends.</summary>
    private static decimal? PromptDecimal(string label)
    {
        while (true)
        {
            var input = Prompt(label);
            if (input is null)
            {
                return null;
            }

            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            Console.WriteLine("Please enter a number, e.g. 10 or 2.5");
        }
    }
}
