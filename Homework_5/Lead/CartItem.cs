using System;

namespace Marketplace.Domain;

/// <summary>Независимая позиция корзины для текущего задания.</summary>
public readonly struct CartItem
{
    public int ProductId { get; }
    public int Quantity { get; }
    public int PricePerUnit { get; }
    public int Total => checked(PricePerUnit * Quantity);

    public CartItem(int productId, int quantity, int pricePerUnit)
    {
        if (productId <= 0) throw new ArgumentOutOfRangeException(nameof(productId));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (pricePerUnit < 0) throw new ArgumentOutOfRangeException(nameof(pricePerUnit));
        ProductId = productId;
        Quantity = quantity;
        PricePerUnit = pricePerUnit;
    }
}
