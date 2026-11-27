namespace Marketplace.Domain;

public class PremiumCustomer : CustomerProfile
{
    public int DiscountPercent { get; }

    public PremiumCustomer(int discountPercent) : base()
    {
        if (discountPercent < 0 || discountPercent > 50)
            throw new ArgumentOutOfRangeException(nameof(discountPercent), "Скидка должна быть в диапазоне от 0% до 50%.");

        DiscountPercent = discountPercent;
    }

    public string GetPremiumInfo() =>
        $"{base.GetDisplayName()} (Скидка: {DiscountPercent}%)";
}
