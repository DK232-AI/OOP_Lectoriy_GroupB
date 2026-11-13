using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Marketplace.Domain;

/// <summary>
/// Товар каталога. Содержит все четыре вида свойств из лабораторной:
/// сокращённое (Title), полную запись (Article, Price), вычисляемое (IsBudget)
/// и свойство с ограниченным сеттером (Status).
/// </summary>
public class Product
{
    private static readonly string[] allowedStatuses = { "В наличии", "Нет в наличии", "Под заказ" };
    private static int counter;

    private readonly int id;
    private string article = string.Empty;
    private int price;

    // 1. string — сокращённая (автоматическая) запись, обязательная при создании.
    public required string Title { get; init; }

    // 2. string — полная запись с проверкой на пустоту.
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

    // 3. int — полная запись, не допускает отрицательных значений.
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

    // 4. bool — вычисляемое свойство: сеттера нет, значение считается из Price.
    public bool IsBudget => Price < 25;

    // Статус меняется только внутри класса: сеттер закрыт.
    public string Status { get; private set; } = "В наличии";

    /// <summary>
    /// Создание через инициализатор: обязательное Title задаётся в объектном
    /// инициализаторе, ID и артикул присваиваются автоматически.
    /// </summary>
    public Product()
    {
        id = checked(++counter);

        // Артикул по умолчанию равен ID: объект не создаётся с пустым артикулом,
        // но установить пустой артикул вручную по-прежнему нельзя (проверка в сеттере).
        Article = id.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>Восстановление товара с известным ID (используется слоем хранения).</summary>
    [SetsRequiredMembers]
    public Product(int id, string title, int price)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "ID должен быть больше нуля.");
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Название товара обязательно.", nameof(title));

        this.id = id;
        Title = title.Trim();
        Article = id.ToString(CultureInfo.InvariantCulture);
        Price = price;
        counter = Math.Max(counter, id);
    }

    /// <summary>Смена статуса: единственный способ, проверка значения внутри класса.</summary>
    public void ChangeStatus(string newStatus)
    {
        EnsureStatus(newStatus);
        Status = newStatus;
    }

    /// <summary>Короткая версия всегда вызывает полную — перегрузки не дублируют код.</summary>
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
