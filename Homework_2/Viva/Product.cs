using System;

namespace Marketplace.Domain;

/// <summary>
/// Товар каталога: ID, название, цена за единицу.
/// </summary>
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
                throw new ArgumentException("Название товара не может быть пустым.", nameof(Name));

            string text = value.Trim();
            name = char.ToUpperInvariant(text[0]) + text.Substring(1);
        }
    }

    public decimal Price
    {
        get => price;
        set
        {
            if (value < 0m)
                throw new ArgumentOutOfRangeException(nameof(Price), "Цена не может быть отрицательной.");

            price = value;
        }
    }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
        Id = checked(++counter);
    }

    public override string ToString() => $"Товар #{Id} \"{Name}\", {Price:F2} УЕ";
}
