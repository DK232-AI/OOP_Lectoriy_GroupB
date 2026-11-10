using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Domain;

namespace Marketplace.Presentation;

/// <summary>
/// Отвечает только за печать каталога (SRP). Ничего не считает и не хранит.
/// </summary>
public class CatalogPrinter
{
    public void PrintAll(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);

        Console.WriteLine("— Каталог товаров —");

        var list = products.ToList();
        if (list.Count == 0)
        {
            Console.WriteLine("Каталог пуст.");
            return;
        }

        foreach (var product in list)
            Console.WriteLine(PrepareLine(product));
    }

    public void PrintTop(IEnumerable<Product> products, int count)
    {
        ArgumentNullException.ThrowIfNull(products);

        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), "Количество не может быть отрицательным.");

        var list = products.ToList();

        if (list.Count == 0)
        {
            Console.WriteLine("Каталог пуст.");
            return;
        }

        if (count == 0)
        {
            Console.WriteLine("Запрошен топ из 0 товаров.");
            return;
        }

        // Если просят больше, чем есть, честно сообщаем, а не молчим.
        if (count > list.Count)
            Console.WriteLine($"Запрошен топ {count}, но в каталоге только {list.Count} товаров.");

        Console.WriteLine($"— Топ {Math.Min(count, list.Count)} самых дорогих товаров —");

        foreach (var product in list.OrderByDescending(p => p.GetPrice()).Take(count))
            Console.WriteLine(PrepareLine(product));
    }

    private string PrepareLine(Product product) =>
        $"[{product.GetId()}] {product.GetTitle(),-20} | Цена: {PriceFormatter.Format(product.GetPrice())}";
}

// Виден только в этом файле.
file static class PriceFormatter
{
    public static string Format(int price) => $"{price} УЕ";
}
