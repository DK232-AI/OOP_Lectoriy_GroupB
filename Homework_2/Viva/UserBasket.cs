using System;
using System.Collections.Generic;

namespace Marketplace;

public class UserBasket
{
    private static int counter;
    private readonly List<ProductPosition> positions = new();

    public int Id { get; }
    public int UserId { get; }

    public UserBasket(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId), "ID пользователя должен быть больше нуля.");
        }

        UserId = userId;
        Id = checked(++counter);
    }

    public void AddPosition(ProductPosition position)
    {
        if (position == null)
        {
            throw new ArgumentNullException(nameof(position));
        }

        if (positions.Exists(item => item.Id == position.Id))
        {
            throw new ArgumentException("Позиция уже добавлена в корзину.", nameof(position));
        }

        positions.Add(position.Copy());
    }

    // Копируются и список, и позиции: историю покупок можно хранить независимо от корзины.
    public List<ProductPosition> GetPositions() => positions.ConvertAll(position => position.Copy());

    public bool RemovePosition(int positionId) => positions.RemoveAll(position => position.Id == positionId) > 0;

    // Вызывать после успешной оплаты. При отказе сервис не должен очищать корзину.
    public void Clear() => positions.Clear();

    public CalculatedPrice CalculatePrice(IEnumerable<Product> products, decimal discountMultiplier)
    {
        if (products == null)
        {
            throw new ArgumentNullException(nameof(products));
        }

        // Проверяем скидку даже для пустой корзины.
        var emptyPrice = new CalculatedPrice(0m, discountMultiplier);
        if (positions.Count == 0)
        {
            return emptyPrice;
        }

        var catalog = new Dictionary<int, Product>();
        foreach (Product product in products)
        {
            if (product == null)
            {
                throw new ArgumentException("Каталог не должен содержать null.", nameof(products));
            }

            if (!catalog.TryAdd(product.Id, product))
            {
                throw new ArgumentException("В каталоге повторяется ID товара.", nameof(products));
            }
        }

        decimal originalPrice = 0m;
        foreach (ProductPosition position in positions)
        {
            if (!catalog.TryGetValue(position.ProductId, out Product? product))
            {
                throw new InvalidOperationException("В каталоге нет товара с ID " + position.ProductId + ".");
            }

            originalPrice += product.Price * position.Quantity;
        }

        return new CalculatedPrice(originalPrice, discountMultiplier);
    }
}
