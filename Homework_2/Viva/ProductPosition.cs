using System;

namespace Marketplace;

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
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Количество должно быть больше нуля.");
            }

            quantity = value;
        }
    }

    public ProductPosition(int productId, int quantity)
    {
        if (productId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(productId), "ID товара должен быть больше нуля.");
        }

        ProductId = productId;
        Quantity = quantity;
        Id = checked(++counter);
    }

    // Все поля имеют типы значений: копия независима и сохраняет ID позиции.
    internal ProductPosition Copy() => (ProductPosition)MemberwiseClone();
}
