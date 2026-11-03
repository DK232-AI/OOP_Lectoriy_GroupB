using System;
using System.Collections.Generic;

namespace Marketplace.Domain;

/// <summary>
/// Корзина покупателя: ID, ID пользователя и список товарных позиций.
/// </summary>
public class UserBasket
{
    private static int counter;
    private readonly List<ProductPosition> positions = new();

    public int Id { get; }
    public int UserId { get; }
    public int Count => positions.Count;
    public bool IsEmpty => positions.Count == 0;

    public UserBasket(int userId)
    {
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId), "ID пользователя должен быть больше нуля.");

        UserId = userId;
        Id = checked(++counter);
    }

    public void AddPosition(ProductPosition position)
    {
        if (position == null)
            throw new ArgumentNullException(nameof(position));

        if (positions.Exists(item => item.Id == position.Id))
            throw new ArgumentException("Позиция уже добавлена в корзину.", nameof(position));

        positions.Add(position.Copy());
    }

    /// <summary>
    /// Возвращает позиции корзины. Копируются и список, и позиции: историю покупок можно хранить независимо от корзины.
    /// </summary>
    public List<ProductPosition> GetPositions() => positions.ConvertAll(position => position.Copy());

    public bool SetQuantity(int positionId, int quantity)
    {
        if (positionId <= 0)
            return false;

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Количество должно быть больше нуля.");

        ProductPosition? position = positions.Find(item => item.Id == positionId);
        if (position == null)
            return false;

        position.Quantity = quantity;
        return true;
    }

    public bool RemovePosition(int positionId)
    {
        if (positionId <= 0)
            return false;

        return positions.RemoveAll(position => position.Id == positionId) > 0;
    }

    /// <summary>Вызывать после успешной оплаты. При отказе сервис не должен очищать корзину.</summary>
    public void Clear() => positions.Clear();

    public CalculatedPrice CalculatePrice(IEnumerable<Product> products, decimal discountMultiplier)
    {
        if (products == null)
            throw new ArgumentNullException(nameof(products));

        // Проверяем скидку даже для пустой корзины.
        CalculatedPrice.ValidateDiscountMultiplier(discountMultiplier);

        if (positions.Count == 0)
            return new CalculatedPrice(0m, discountMultiplier);

        var catalog = new Dictionary<int, Product>();
        foreach (Product product in products)
        {
            if (product == null)
                throw new ArgumentException("Каталог не должен содержать null.", nameof(products));

            if (!catalog.TryAdd(product.Id, product))
                throw new ArgumentException("В каталоге повторяется ID товара.", nameof(products));
        }

        decimal originalPrice = 0m;
        foreach (ProductPosition position in positions)
        {
            if (!catalog.TryGetValue(position.ProductId, out Product? product))
                throw new InvalidOperationException("В каталоге нет товара с ID " + position.ProductId + ".");

            originalPrice += product.Price * position.Quantity;
        }

        return new CalculatedPrice(originalPrice, discountMultiplier);
    }

    public override string ToString() => $"Корзина #{Id} пользователя {UserId}: позиций {Count}";
}
