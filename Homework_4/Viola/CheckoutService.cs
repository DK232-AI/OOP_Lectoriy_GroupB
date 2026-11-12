using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Domain;

namespace Marketplace.Services;

public class CheckoutService
{
    public int GetTotal(IEnumerable<CartItem> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        return items.Sum(item => item.Total);
    }

    public IReadOnlyList<CartItem> FindCheaperThan(IEnumerable<CartItem> items, int maxPrice)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));
        if (maxPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(maxPrice));

        return items.Where(item => item.PricePerUnit < maxPrice).ToList();
    }

    public string GetReport(IEnumerable<CartItem> items)
    {
        return GetReport(items, false);
    }

    public string GetReport(IEnumerable<CartItem> items, bool includeLines)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        var itemsList = items.ToList();
        if (itemsList.Count == 0)
            return "Корзина пуста.";

        var availableItems = itemsList.Where(IsAvailable).ToList();
        int totalSum = GetTotal(availableItems);

        string report = $"Сводка заказа: \nДоступно позиций {availableItems.Count} из {itemsList.Count}. Итоговая сумма: {totalSum} УЕ.";

        if (includeLines)
        {
            report += "\nДетали заказа:";
            foreach (var item in availableItems)
            {
                report += "\n" + PrepareLine(item);
            }
        }

        return report;
    }



    // default(CartItem) создается без вызова конструктора и не является позицией покупки.
    private bool IsAvailable(CartItem item)
    {
        return item.ProductId > 0 && item.Quantity > 0 && item.PricePerUnit >= 0;
    }

    private string PrepareLine(CartItem item)
    {
        return $"Товар #{item.ProductId} | Кол-во: {item.Quantity} | Цена за шт.: {item.PricePerUnit} УЕ | Итого: {item.Total} УЕ";
    }
}
