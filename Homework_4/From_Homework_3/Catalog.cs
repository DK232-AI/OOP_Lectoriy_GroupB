using System;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Domain;

/// <summary>
/// Каталог товаров: закрытая коллекция и публичный API для просмотра и изменения состава.
/// </summary>
public class Catalog
{
    private readonly List<Product> products = new();

    public void Add(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        // Сравнение по названию без учёта регистра и пробелов по краям.
        if (products.Any(existing => SameTitle(existing.GetTitle(), product.GetTitle())))
            throw new ArgumentException("Товар с таким названием уже есть в каталоге.", nameof(product));

        // Дополнительная защита: одинаковый ID ломает сохранение и расчёт корзины.
        if (products.Any(existing => existing.GetId() == product.GetId()))
            throw new ArgumentException("Товар с таким ID уже есть в каталоге.", nameof(product));

        products.Add(product);
    }

    public bool Remove(string title)
    {
        int index = FindIndex(title);
        if (index < 0)
            return false;

        products.RemoveAt(index);
        return true;
    }

    public Product? Find(string title)
    {
        int index = FindIndex(title);
        return index < 0 ? null : products[index];
    }

    /// <summary>
    /// Возвращает копию: снаружи нельзя изменить внутренний список.
    /// </summary>
    public IReadOnlyList<Product> GetProducts() => products.ToList();

    public int GetCount() => products.Count;

    private int FindIndex(string title)
    {
        ArgumentNullException.ThrowIfNull(title);
        return products.FindIndex(product => SameTitle(product.GetTitle(), title));
    }

    private static string Normalize(string title) => title.Trim();

    private static bool SameTitle(string left, string right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);
}
