using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Domain;

namespace Marketplace.Presentation;

/// <summary>
/// Отвечает только за расчёты по каталогу (SRP). Ничего не печатает и не работает с файлами.
/// </summary>
public class CatalogReport
{
    public decimal GetAveragePrice(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);

        var prices = products.Select(product => (decimal)product.GetPrice()).ToList();
        if (prices.Count == 0)
            return 0m;

        return prices.Average();
    }

    public decimal GetSummary(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);

        return products.Sum(product => (decimal)product.GetPrice());
    }
}
