using System;
using System.Collections.Generic;
using System.Text;
using Marketplace.Domain;

namespace Marketplace.Services;

// Статический сервис для формирования текстового чека.
public static class ReceiptService
{
    public static string BuildReceipt(IEnumerable<CartItem> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        var sb = new StringBuilder();

        int orderNumber = OrderNumberGenerator.Next();
        sb.AppendLine($"- Ваш чек по заказу №{orderNumber} -");

        // создаем экземпляр CheckoutService, так как его метод GetTotal не статический
        var checkoutService = new CheckoutService();

        foreach (var item in items)
        {
            sb.AppendLine(PrepareLine(item));
        }

        int totalWithoutVat = checkoutService.GetTotal(items);
        int totalWithVat = PriceRules.WithVat(totalWithoutVat);

        sb.AppendLine(new string('-', 30));
        sb.AppendLine($"Итого с НДС: {FormatMoney(totalWithVat)}");
        sb.AppendLine($"В том числе НДС ({(PriceRules.VatRate * 100)}%): {FormatMoney(totalWithVat - totalWithoutVat)}");

        return sb.ToString();
    }

    private static string PrepareLine(CartItem item)
    {
        // использую свойства из CartItem для сборки строчки: ProductId, Quantity, Total
        return $"- Товар #{item.ProductId} | {item.Quantity} шт. x {FormatMoney(item.PricePerUnit)} = {FormatMoney(item.Total)}";
    }

    private static string FormatMoney(int amount)
    {
        return $"{amount} {PriceRules.DefaultCurrency}";
    }
}
