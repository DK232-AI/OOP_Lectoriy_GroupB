using System;
using System.Diagnostics.CodeAnalysis;

namespace Marketplace.Domain;

public class Product
{
    private static readonly string[] allowedStatuses = { "В наличии", "Нет в наличии", "Под заказ" };
    private static int counter;
    private readonly int id;
    private string article = string.Empty;
    private int price;

    public required string Title { get; init; }

    public string Article
    {
        get => article;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Артикул обязателен.", nameof(value));

            article = value.Trim();
        }
    }

    public int Price
    {
        get => price;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Цена не может быть отрицательной.");

            price = value;
        }
    }

    public bool IsBudget => Price < 25;
    public string Status { get; private set; } = "В наличии";

    public Product()
    {
        id = checked(++counter);
    }

    [SetsRequiredMembers]
    public Product(int id, string title, int price)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "ID должен быть больше нуля.");
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Название товара обязательно.", nameof(title));

        this.id = id;
        Title = title.Trim();
        Article = id.ToString(System.Globalization.CultureInfo.InvariantCulture);
        Price = price;
        counter = Math.Max(counter, id);
    }

    public void ChangeStatus(string newStatus)
    {
        EnsureStatus(newStatus);
        Status = newStatus;
    }

    public string GetInfo() => GetInfo(true);

    public string GetInfo(bool includeIntProperty) => includeIntProperty
        ? $"{Title} [{Article}], цена: {Price} УЕ, статус: {Status}"
        : $"{Title} [{Article}], статус: {Status}";

    public int GetId() => id;
    public string GetTitle() => Title;
    public int GetPrice() => Price;
    public void SetPrice(int newPrice) => Price = newPrice;

    private static void EnsureStatus(string value)
    {
        if (Array.IndexOf(allowedStatuses, value) < 0)
            throw new ArgumentException("Недопустимый статус товара.", nameof(value));
    }
}
