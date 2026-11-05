using System;
using System.Collections.Generic;

namespace Marketplace.Domain;

public class Catalog
{
    private readonly List<Product> products = new();

    public void Add(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (Find(product.GetTitle()) != null)
            throw new ArgumentException("Товар с таким названием уже есть в каталоге.", nameof(product));

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

    public IReadOnlyList<Product> GetProducts() => Array.AsReadOnly(products.ToArray());
    public int GetCount() => products.Count;

    private int FindIndex(string title)
    {
        ArgumentNullException.ThrowIfNull(title);
        string key = title.Trim();
        return products.FindIndex(product =>
            string.Equals(product.GetTitle(), key, StringComparison.OrdinalIgnoreCase));
    }
}
