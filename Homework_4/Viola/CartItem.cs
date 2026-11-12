using System;

namespace Marketplace.Domain;

public readonly struct CartItem
{
    public int ProductId { get; }
    public int Quantity { get; }
    public int PricePerUnit { get; }    //опять же, почему для денег int а не decimal?

    public int Total => PricePerUnit * Quantity;

    public CartItem(int productId, int quantity, int pricePerUnit)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId), "ID товара должен быть больше нуля.");

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Количество товаров должно быть больше нуля.");

        if (pricePerUnit < 0)
            throw new ArgumentOutOfRangeException(nameof(pricePerUnit), "Цена товара не может быть меньше нуля.");

        ProductId = productId;
        Quantity = quantity;
        PricePerUnit = pricePerUnit;
    }
}
