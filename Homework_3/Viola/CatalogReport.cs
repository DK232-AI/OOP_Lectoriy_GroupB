using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Domain;

namespace Marketplace.Presentation;

public class CatalogReport
{
    public decimal GetAveragePrice(IEnumerable<Product> products)
    {
        if (products == null) 
            throw new ArgumentNullException(nameof(products));

        var list = products.ToList();
        if (list.Count == 0) return 0m;

        return list.Average(p => p.Price);
    }

    public decimal GetSummary(IEnumerable<Product> products)
    {
        if (products == null) 
            throw new ArgumentNullException(nameof(products));

        return products.Sum(p => p.Price);
    }
}
