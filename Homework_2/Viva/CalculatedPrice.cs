using System;

namespace Marketplace;

public readonly struct CalculatedPrice
{
    private readonly decimal discountMultiplier;

    public decimal OriginalPrice { get; }

    // default(CalculatedPrice) представляет пустую корзину без скидки.
    public decimal DiscountMultiplier => discountMultiplier == 0m ? 1m : discountMultiplier;
    public decimal FinalPrice => OriginalPrice * DiscountMultiplier;
    public decimal DiscountAmount => OriginalPrice - FinalPrice;

    public CalculatedPrice(decimal originalPrice, decimal discountMultiplier)
    {
        if (originalPrice < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(originalPrice), "Исходная цена не может быть отрицательной.");
        }

        if (discountMultiplier < 0.01m || discountMultiplier > 1m)
        {
            throw new ArgumentOutOfRangeException(nameof(discountMultiplier), "Множитель скидки должен быть от 0.01 до 1.00.");
        }

        OriginalPrice = originalPrice;
        this.discountMultiplier = discountMultiplier;
    }

    public decimal GetFinalPrice() => FinalPrice;

    public decimal GetDiscountAmount() => DiscountAmount;
}
