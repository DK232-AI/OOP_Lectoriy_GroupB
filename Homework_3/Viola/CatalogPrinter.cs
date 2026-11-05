using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Domain;

namespace Marketplace.Presentation;

public class CatalogPrinter
{
    public void PrintAll(IEnumerable<Product> products)
    {
        if (products == null) 
            throw new ArgumentNullException(nameof(products));

        Console.WriteLine("- Каталог товаров -");
        var list = products.ToList();

        if (list.Count == 0)
        {
            Console.WriteLine("Каталог пуст.");
            return;
        }

        foreach (var p in list)
        {
            Console.WriteLine(PrepareLine(p));
        }
    }

    public void PrintTop(IEnumerable<Product> products, int count)
    {
        if (products == null) 
            throw new ArgumentNullException(nameof(products));
        if (count < 0)
            throw new ArgumentException("Количество не может быть отрицательным", nameof(count));
        if (count == 0)
        {
            Console.WriteLine("Запрошен топ из 0 товаров.");
            return;
        }

        Console.WriteLine($"- Топ {count} самых дорогих товаров в каталоге -");

        var topProducts = products.OrderByDescending(p => p.Price).Take(count);

        foreach (var p in topProducts)
        {
            Console.WriteLine(PrepareLine(p));
        }
    }

    private string PrepareLine(Product product)
    {
        return $"[{product.Id}] {product.Name,-20} | Цена: {PriceFormatter.Format(product.Price)}";
    }
}

file static class PriceFormatter
{
    public static string Format(decimal price) => $"{price:F2} y.e.";
}
