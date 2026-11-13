using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marketplace.Domain;

namespace Marketplace.Services;

/// <summary>
/// Сервис оформления заказа. Без состояния: все данные приходят параметрами,
/// поэтому один экземпляр можно использовать повторно.
/// </summary>
public class CheckoutService
{
    public int GetTotal(IEnumerable<CartItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return items.Sum(item => item.Total);
    }

    /// <summary>
    /// Позиции дешевле заданной цены за единицу.
    /// Недоступные позиции (в том числе default(CartItem)) не попадают в результат:
    /// у значения по умолчанию цена 0, и формально оно «дешевле» любой границы.
    /// </summary>
    public IReadOnlyList<CartItem> FindCheaperThan(IEnumerable<CartItem> items, int maxPrice)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (maxPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(maxPrice), "Предельная цена не может быть отрицательной.");

        return items.Where(item => IsAvailable(item) && item.PricePerUnit < maxPrice).ToList();
    }

    /// <summary>Короткая версия всегда вызывает полную — дублирования кода нет.</summary>
    public string GetReport(IEnumerable<CartItem> items) => GetReport(items, false);

    public string GetReport(IEnumerable<CartItem> items, bool includeLines)
    {
        ArgumentNullException.ThrowIfNull(items);

        var itemsList = items.ToList();
        if (itemsList.Count == 0)
            return "Корзина пуста.";

        var availableItems = itemsList.Where(IsAvailable).ToList();
        int totalSum = GetTotal(availableItems);

        var report = new StringBuilder();
        report.Append($"Сводка заказа:{Environment.NewLine}");
        report.Append($"Доступно позиций {availableItems.Count} из {itemsList.Count}. Итоговая сумма: {totalSum} УЕ.");

        if (includeLines)
        {
            report.Append($"{Environment.NewLine}Детали заказа:");
            foreach (var item in availableItems)
                report.Append(Environment.NewLine).Append(PrepareLine(item));
        }

        return report.ToString();
    }

    /// <summary>
    /// default(CartItem) создаётся без вызова конструктора и не является позицией покупки.
    /// </summary>
    private static bool IsAvailable(CartItem item) =>
        item.ProductId > 0 && item.Quantity > 0 && item.PricePerUnit >= 0;

    private static string PrepareLine(CartItem item) =>
        $"Товар #{item.ProductId} | Кол-во: {item.Quantity} | Цена за шт.: {item.PricePerUnit} УЕ | Итого: {item.Total} УЕ";
}
