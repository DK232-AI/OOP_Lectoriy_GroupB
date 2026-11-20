using System;
using System.Collections.Generic;

namespace Marketplace.Domain;

/// <summary>Собственная корзина текущего задания;</summary>
public sealed class Cart
{
    private readonly List<CartItem> items = new();
    public IReadOnlyList<CartItem> Items => items.AsReadOnly();

    public void Add(CartItem item)
    {
        if (item.ProductId <= 0 || item.Quantity <= 0 || item.PricePerUnit < 0)
            throw new ArgumentException("Недопустимая позиция корзины.", nameof(item));
        items.Add(item);
    }
}
