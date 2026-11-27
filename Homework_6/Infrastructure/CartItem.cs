namespace Marketplace.Domain;

public readonly struct CartItem
{
    public int ProductId { get; }
    public int Quantity { get; }
    public int PricePerUnit { get; }
    public int Total => checked(Quantity * PricePerUnit);

    public CartItem(int productId, int quantity, int pricePerUnit)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));
        if (pricePerUnit < 0)
            throw new ArgumentOutOfRangeException(nameof(pricePerUnit));

        ProductId = productId;
        Quantity = quantity;
        PricePerUnit = pricePerUnit;
    }
}
