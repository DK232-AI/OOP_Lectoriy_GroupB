using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Linq;
using Marketplace.Domain;

namespace Marketplace.DataAccess;

internal class FileProductRepository : IProductRepository
{
    private readonly string _filePath;

    public FileProductRepository(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public void Save(IEnumerable<Product> products)
    {
        if (products == null) throw new ArgumentNullException(nameof(products));

        var lines = new List<string>();
        foreach (var p in products)
        {
            lines.Add(Serialize(p));
        }

        // сохраняем в файл в кодировке UTF-8, чтобы русские буквы сохранялись
        File.WriteAllLines(_filePath, lines, Encoding.UTF8);
    }

    // чтобы избежать коллизий с ID, при вызове Load() текущий Catalog должен
    // быть полностью очищен и заполнен заново из файла коллекцией result
    public List<Product> Load()
    {
        var result = new List<Product>();

        if (!File.Exists(_filePath)) return result;

        var lines = File.ReadAllLines(_filePath, Encoding.UTF8);
        foreach (var l in lines)
        {
            try
            {
                var product = Deserialize(l);

                // кроме нуль проверки ещё защита от дубликатов
                if (product != null && !result.Any(p => p.Id == product.Id))
                {
                    result.Add(product);
                }
            }
            catch (Exception)
            {
                continue;
            }
        }
        return result;
    }



    private static string Serialize(Product product)
    {
        // формат: ID;Название;Цена с точкой для дробей, чтобы не ломалось при разных языках ПК
        return $"{product.Id};{product.Name};{product.Price.ToString(CultureInfo.InvariantCulture)}";
    }

    private static Product Deserialize(string line)
    {
        var parts = line.Split(';');
        if (parts.Length != 3) return null;

        int id = int.Parse(parts[0]);
        string name = parts[1];
        decimal price = decimal.Parse(parts[2], CultureInfo.InvariantCulture);

        return new Product(id, name, price);
    }
}
