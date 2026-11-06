using System.Collections.Generic;
using Marketplace.Domain;

namespace Marketplace.DataAccess;

/// <summary>
/// Абстракция хранилища товаров. Зависимости строятся на ней, а не на конкретном классе (DIP).
/// </summary>
public interface IProductRepository
{
    void Save(IEnumerable<Product> products);
    List<Product> Load();
}
