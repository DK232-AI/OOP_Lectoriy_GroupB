using System;

namespace Marketplace.Domain;

public readonly struct CalculatedPrice
{
    public decimal OriginalPrice { get; }
    public decimal DiscountMultiplier { get; }
    public decimal FinalPrice => Math.Round(OriginalPrice * DiscountMultiplier, 2, MidpointRounding.AwayFromZero);
    public decimal DiscountAmount => OriginalPrice - FinalPrice;

    public CalculatedPrice(decimal originalPrice, decimal discountMultiplier)
    {
        if (originalPrice < 0m)
            throw new ArgumentOutOfRangeException(nameof(originalPrice), "Исходная цена не может быть отрицательной.");

        ValidateDiscountMultiplier(discountMultiplier);
        OriginalPrice = Math.Round(originalPrice, 2, MidpointRounding.AwayFromZero);
        DiscountMultiplier = discountMultiplier;
    }

    internal static void ValidateDiscountMultiplier(decimal discountMultiplier)
    {
        if (discountMultiplier < 0.01m || discountMultiplier > 1m)
            throw new ArgumentOutOfRangeException(nameof(discountMultiplier), "Множитель скидки должен быть от 0.01 до 1.");
    }
}
