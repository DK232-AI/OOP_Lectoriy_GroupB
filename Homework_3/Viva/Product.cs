using System;

namespace Marketplace.Domain;

public class Product
{
    private static int counter;
    private readonly int id;
    private readonly string title;
    private int price;

    public int Id => id;
    public string Name => title;
    public decimal Price => price;

    public Product(int id, string title, int price) : this(id, title, (decimal)price)
    {
    }

    public Product(string name, decimal price) : this(checked(counter + 1), name, price)
    {
    }

    public Product(int id, string name, decimal price)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "ID должен быть больше нуля.");

        this.id = id;
        title = EnsureTitle(name);
        this.price = EnsurePrice(price);
        counter = Math.Max(counter, id);
    }

    public int GetId() => id;
    public string GetTitle() => title;
    public int GetPrice() => price;
    public void SetPrice(int newPrice) => price = EnsurePrice(newPrice);
    public string GetInfo() => $"Товар #{id} \"{title}\", {price} УЕ";
    public override string ToString() => GetInfo();

    private static string EnsureTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Название товара не может быть пустым.", nameof(title));

        return title.Trim();
    }

    private static int EnsurePrice(decimal price)
    {
        if (price < 0 || price > int.MaxValue || decimal.Truncate(price) != price)
            throw new ArgumentOutOfRangeException(nameof(price), "Цена должна быть целым неотрицательным числом.");

        return (int)price;
    }
}
