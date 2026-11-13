using System;

namespace Marketplace.Domain;

/// <summary>
/// Позиция корзины. На занятии 2 была классом; теперь readonly struct:
/// данные позиции после создания не меняются, стоимость считается свойством.
/// </summary>
public readonly struct CartItem
{
    public int ProductId { get; }
    public int Quantity { get; }
    public int PricePerUnit { get; }

    /// <summary>Вычисляемое свойство: стоимость позиции.</summary>
    public int Total => checked(PricePerUnit * Quantity);

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
