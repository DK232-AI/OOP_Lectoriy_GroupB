using System;

namespace Marketplace.Domain;

public class Discount
{
    public required string Name { get; init; }
    public DateTime ValidUntil { get; init; }
    public bool IsActive => ValidUntil > DateTime.Now;

    private decimal _percent;
    public decimal Percent
    {
        get => _percent;
        set
        {
            EnsurePercent(value);
            _percent = value;
        }
    }

    private int _minSum;
    public int MinSum
    {
        get => _minSum;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(MinSum),"Минимальная сумма не может быть отрицательной.");
            _minSum = value;
        }
    }



    public int Calculate(int total)
    {
        return Calculate(total, Percent);
    }

    public int Calculate(int total, decimal percent)
    {
        if (total < 0)
            throw new ArgumentOutOfRangeException(nameof(total), "Итоговая сумма не может быть отрицательной.");

        EnsurePercent(percent);

        if (total < MinSum)
        {
            return total; 
        }

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

        return (int)finalPrice;
    }
}
