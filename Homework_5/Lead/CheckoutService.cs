using System;
using System.Collections.Generic;

using Marketplace.Domain;

namespace Marketplace.Services;

/// <summary>Сервис без состояния, используемый при формировании чека.</summary>
public sealed class CheckoutService
{
    public int GetTotal(IEnumerable<CartItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        int total = 0;
        foreach (CartItem item in items)
        {
            if (item.ProductId <= 0 || item.Quantity <= 0 || item.PricePerUnit < 0)
                throw new ArgumentException("Недопустимая позиция корзины.", nameof(items));
            total = checked(total + item.Total);
        }
        return total;
    }
}
