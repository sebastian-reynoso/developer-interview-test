using System;
using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner.Data;

/// <summary>In-memory stand-in for the database-backed ProductDataStore.</summary>
public class InMemoryProductDataStore : IProductDataStore
{
    private readonly Dictionary<string, Product> _products;

    public InMemoryProductDataStore(IEnumerable<Product> products)
    {
        _products = products.ToDictionary(product => product.Identifier, StringComparer.OrdinalIgnoreCase);
    }

    public IEnumerable<Product> Products => _products.Values;

    public Product GetProduct(string productIdentifier) => _products.GetValueOrDefault(productIdentifier);
}
