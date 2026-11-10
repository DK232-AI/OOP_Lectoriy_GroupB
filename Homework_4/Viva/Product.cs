using System;

namespace Marketplace.Domain;

/// <summary>
/// Товар каталога. Все поля закрытые, проверки собраны в одном месте —
/// в приватных методах EnsureTitle и EnsurePrice. Снаружи доступен только
/// контракт из задания: GetId, GetTitle, GetPrice, SetPrice, GetInfo.
/// </summary>
public class Product
{
    private readonly int id;
    private readonly string title;
    private int price;

    public Product(int id, string title, int price)
    {
        // ID проверяется прямо в конструкторе: изменять его методы класса не могут,
        // поэтому отдельный Ensure-метод для него не нужен.
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Идентификатор должен быть больше нуля.");

        EnsureTitle(title);
        EnsurePrice(price);

        this.id = id;
        this.title = title.Trim();
        this.price = price;
    }

    public int GetId() => id;
    public string GetTitle() => title;
    public int GetPrice() => price;

    public void SetPrice(int newPrice)
    {
        EnsurePrice(newPrice);
        price = newPrice;
    }

    public string GetInfo() => $"Товар #{id} \"{title}\", {price} УЕ";

    private static void EnsureTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Название товара не может быть пустым.", nameof(value));
    }

    private static void EnsurePrice(int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Цена не может быть отрицательной.");
    }
}
