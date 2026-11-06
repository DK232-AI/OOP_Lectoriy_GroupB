using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Marketplace.Domain;

namespace Marketplace.DataAccess;

/// <summary>
/// Файловое хранилище товаров. Внутренняя деталь реализации, наружу сборки не нужна.
/// Формат строки: ID;Название;Цена — один товар на строку.
/// </summary>
internal class FileProductRepository : IProductRepository
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    private readonly string filePath;

    public FileProductRepository(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));

        this.filePath = filePath;
    }

    public void Save(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);

        var lines = new List<string>();
        foreach (var product in products)
        {
            if (product == null)
                throw new ArgumentException("Список товаров не должен содержать null.", nameof(products));

            lines.Add(Serialize(product));
        }

        // UTF-8 без BOM: файл читается и нашим кодом, и сторонними инструментами.
        File.WriteAllLines(filePath, lines, Utf8NoBom);
    }

    /// <summary>
    /// Читает товары из файла. Отсутствующий файл — это пустой список.
    /// Битые строки пропускаются: одна испорченная запись не роняет загрузку.
    /// ВНИМАНИЕ: чтобы не было коллизий по ID, перед заполнением создайте новый Catalog.
    /// </summary>
    public List<Product> Load()
    {
        var result = new List<Product>();

        if (!File.Exists(filePath))
            return result;

        var lines = File.ReadAllLines(filePath, Utf8NoBom);
        foreach (var line in lines)
        {
            Product? product = Deserialize(line);

            // null — битая строка; повтор ID — тоже пропускаем.
            if (product == null || result.Exists(existing => existing.GetId() == product.GetId()))
                continue;

            result.Add(product);
        }

        return result;
    }

    private static string Serialize(Product product)
    {
        string title = product.GetTitle();

        // Разделитель и перевод строки внутри названия сломали бы формат.
        if (title.Contains(';') || title.Contains('\n') || title.Contains('\r'))
            throw new ArgumentException("Название товара не должно содержать символ ';' или перевод строки.", nameof(product));

        return string.Join(';',
            product.GetId().ToString(CultureInfo.InvariantCulture),
            title,
            product.GetPrice().ToString(CultureInfo.InvariantCulture));
    }

    private static Product? Deserialize(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;

        var parts = line.Split(';');
        if (parts.Length != 3)
            return null;

        if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
            return null;

        if (!int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int price))
            return null;

        try
        {
            return new Product(id, parts[1], price);
        }
        catch (ArgumentException)
        {
            // Некорректные данные из файла — пропускаем строку, а не роняем загрузку.
            return null;
        }
    }
}
