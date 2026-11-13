using System;

namespace Marketplace.Domain;

/// <summary>
/// Скидка на заказ. Расчёт вынесен в перегруженный Calculate,
/// проверки — в приватные EnsurePercent и ApplyDiscount.
/// </summary>
public class Discount
{
    private decimal percent;
    private int minSum;

    // Сокращённая запись, обязательная при создании.
    public required string Name { get; init; }

    public DateTime ValidUntil { get; init; }

    // Вычисляемое свойство: сеттера нет, значение считается из ValidUntil.
    public bool IsActive => ValidUntil > DateTime.Now;

    // Полная запись: процент от 0 до 100.
    public decimal Percent
    {
        get => percent;
        set
        {
            EnsurePercent(value);
            percent = value;
        }
    }

    // Полная запись: минимальная сумма заказа, не может быть отрицательной.
    public int MinSum
    {
        get => minSum;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Минимальная сумма не может быть отрицательной.");

            minSum = value;
        }
    }

    /// <summary>Короткая версия всегда вызывает полную — дублирования кода нет.</summary>
    public int Calculate(int total) => Calculate(total, Percent);

    /// <summary>Итог после скидки с учётом минимальной суммы заказа.</summary>
    public int Calculate(int total, decimal percent)
    {
        if (total < 0)
            throw new ArgumentOutOfRangeException(nameof(total), "Итоговая сумма не может быть отрицательной.");

        EnsurePercent(percent);

        // Минимальная сумма не набрана — скидка не применяется.
        if (total < MinSum)
            return total;

        return ApplyDiscount(total, percent);
    }

    private static void EnsurePercent(decimal value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentOutOfRangeException(nameof(value), "Скидка должна быть в пределах от 0 до 100 процентов.");
    }

    private static int ApplyDiscount(int total, decimal percent)
    {
        decimal discountAmount = total * (percent / 100m);
        decimal finalPrice = total - discountAmount;

        // Округление до целых по правилам арифметики (0.5 — вверх).
        // В задании тип цены — int, поэтому результат приводится к целому явно.
        return (int)Math.Round(finalPrice, MidpointRounding.AwayFromZero);
    }
}
