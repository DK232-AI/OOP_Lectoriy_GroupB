using System;

namespace Marketplace.Domain;

/// <summary>
/// Товарная позиция в корзине: ID позиции, ID товара, количество единиц.
/// </summary>
public class ProductPosition
{
    private static int counter;
    private int quantity;

    public int Id { get; }
    public int ProductId { get; }

    public int Quantity
    {
        get => quantity;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(Quantity), "Количество должно быть больше нуля.");

            quantity = value;
        }
    }

    public ProductPosition(int productId, int quantity)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId), "ID товара должен быть больше нуля.");

        ProductId = productId;
        Quantity = quantity;
        Id = checked(++counter);
    }

    // Все поля имеют типы значений: копия независима и сохраняет ID позиции.
    internal ProductPosition Copy() => (ProductPosition)MemberwiseClone();

    public override string ToString() => $"Позиция #{Id}: товар {ProductId} x {Quantity}";
}
