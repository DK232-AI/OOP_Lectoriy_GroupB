using System;

namespace Marketplace;

public class Product
{
    private static int counter;
    private string name = string.Empty;
    private decimal price;

    public int Id { get; }

    public string Name
    {
        get => name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Название товара не может быть пустым.", nameof(value));
            }

            string text = value.Trim();
            name = char.ToUpper(text[0]) + text.Substring(1);
        }
    }

    public decimal Price
    {
        get => price;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Цена не может быть отрицательной.");
            }

            price = value;
        }
    }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
        Id = checked(++counter);
    }
}
