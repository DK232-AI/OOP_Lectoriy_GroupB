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



    // IsAvailable проверяет доступность позиции 
    // Сейчас конструктор CartItem уже гарантирует, что Quantity > 0, так что в сущности метод бесполезен
    // Но IsAvailable указан в методичке как обязательный, я только не знаю, как его сделать осмысленным
    // Возможно, у нас есть другие способы установить доступность товара для покупки? Например база склада или типа того?
    private bool IsAvailable(CartItem item)
    {
        return item.Quantity > 0;
    }

    private string PrepareLine(CartItem item)
    {
        return $"Товар #{item.ProductId} | Кол-во: {item.Quantity} | Цена за шт.: {item.PricePerUnit} УЕ | Итого: {item.Total} УЕ";
    }
}
