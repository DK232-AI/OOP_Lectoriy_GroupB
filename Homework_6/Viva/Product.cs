using System;
using System.Globalization;
using ProductId = System.Int32;

namespace Marketplace.Domain;

public class Product
{
    public const int MaxPrice = 1_000_000;

    private static int nextId;
    private static readonly string[] allowedStatuses = { "В наличии", "Нет в наличии", "Под заказ" };
    public static readonly DateTime CatalogStartedAt;

    private readonly DateTime createdAt;
    private string article = string.Empty;
    private int price;

    public static int CreatedCount => nextId;
    public ProductId Id { get; }
    public required string Title { get; init; }
    public string? Description { get; set; }

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
            if (value < 0 || value > MaxPrice)
                throw new ArgumentOutOfRangeException(nameof(value), $"Цена должна быть от 0 до {MaxPrice}.");

            price = value;
        }
    }

    public DateTime CreatedAt => createdAt;
    public bool IsBudget => Price < 25;
    public string Status { get; private set; } = "В наличии";

    static Product()
    {
        CatalogStartedAt = DateTime.UtcNow;
    }

    public Product()
    {
        Id = checked(++nextId);
        createdAt = DateTime.Now;
        Article = Id.ToString(CultureInfo.InvariantCulture);
    }

    public string GetDescription() => Description ?? "описание не указано";

    public void ChangeStatus(string newStatus)
    {
        EnsureStatus(newStatus);
        Status = newStatus;
    }

    public string GetInfo() => GetInfo(true);

    public string GetInfo(bool includeIntProperty) => includeIntProperty
        ? $"{Title} [{Article}], цена: {Price} УЕ, статус: {Status}"
        : $"{Title} [{Article}], статус: {Status}";

    private static void EnsureStatus(string value)
    {
        if (Array.IndexOf(allowedStatuses, value) < 0)
            throw new ArgumentException("Недопустимый статус товара.", nameof(value));
    }
}
