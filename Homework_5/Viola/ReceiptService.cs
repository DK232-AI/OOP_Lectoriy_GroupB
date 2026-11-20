using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marketplace.Domain;

namespace Marketplace.Services;

/// <summary>
/// Статический сервис для формирования текстового чека.
/// Демонстрирует: static класс без состояния, работа с другими сервисами.
/// </summary>
public static class ReceiptService
{
    public static string BuildReceipt(IEnumerable<CartItem> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        // Материализуем один раз: IEnumerable может быть одноразовым перечислением.
        var lines = items.ToList();
        var checkoutService = new CheckoutService();
        int totalWithoutVat = checkoutService.GetTotal(lines);
        int totalWithVat = PriceRules.WithVat(totalWithoutVat);

        var sb = new StringBuilder();
        int orderNumber = OrderNumberGenerator.Next();
        sb.AppendLine($"- Ваш чек по заказу №{orderNumber} -");
        foreach (var item in lines)
            sb.AppendLine(PrepareLine(item));

        sb.AppendLine(new string('-', 30));
        sb.AppendLine($"Итого с НДС: {FormatMoney(totalWithVat)}");
        sb.AppendLine($"В том числе НДС ({(PriceRules.VatRate * 100)}%): {FormatMoney(totalWithVat - totalWithoutVat)}");

        return sb.ToString();
    }

    private static string PrepareLine(CartItem item)
    {
        return $"- Товар #{item.ProductId} | {item.Quantity} шт. x {FormatMoney(item.PricePerUnit)} = {FormatMoney(item.Total)}";
    }

    private static string FormatMoney(int amount)
    {
        return $"{amount} {PriceRules.DefaultCurrency}";
    }
}
