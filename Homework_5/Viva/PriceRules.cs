using System;

namespace Marketplace.Domain;

public static class PriceRules
{
    public const decimal VatRate = 0.2m;
    public const int MaxDiscountPercent = 70;
    public static readonly string DefaultCurrency = "RUB";

    public static int WithVat(int price)
    {
        EnsurePrice(price);
        return checked((int)(price * (1m + VatRate)));
    }

    public static int ApplyDiscount(int price, int percent)
    {
        EnsurePrice(price);
        EnsurePercent(percent);
        return (int)(price * (100m - percent) / 100m);
    }

    private static void EnsurePrice(int price)
    {
        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Цена не может быть отрицательной.");
    }

    private static void EnsurePercent(int percent)
    {
        if (percent < 0 || percent > MaxDiscountPercent)
            throw new ArgumentOutOfRangeException(nameof(percent), "Скидка должна быть от 0 до 70 процентов.");
    }
}
