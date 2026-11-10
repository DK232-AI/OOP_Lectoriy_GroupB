using System;
using System.Collections.Generic;
using Marketplace.DataAccess;
using Marketplace.Domain;
using Marketplace.Presentation;

namespace Marketplace;

/// <summary>
/// Точка входа компоновки: демонстрация инкапсуляции, DIP и SRP.
/// </summary>
public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Section("1. Создание товаров, в том числе с недопустимыми данными");

        var catalog = new Catalog();
        catalog.Add(new Product(1, "Кружка", 450));
        catalog.Add(new Product(2, "Термос", 1200));
        catalog.Add(new Product(3, "Набор стикеров", 90));

        Console.WriteLine($"В каталог добавлено товаров: {catalog.GetCount()}");

        // Целостность данных: недопустимые значения не попадают в объект.
        TryAdd(catalog, 4, "   ", -5, "пустое название и отрицательная цена");
        TryAdd(catalog, 4, "Товар", -5, "отрицательная цена");
        TryAdd(catalog, 0, "Товар", 100, "ID равен нулю");
        TryAdd(catalog, 5, "кружка", 300, "дубль названия в другом регистре");
        TryAdd(catalog, 1, "Другой товар", 300, "дубль ID");

        Console.WriteLine($"Товаров в каталоге после отклонённых попыток: {catalog.GetCount()}");

        Section("2. Поиск, удаление и попытка испортить коллекцию снаружи");

        var found = catalog.Find("  термос  ");
        Console.WriteLine(found == null ? "Термос не найден." : $"Найден: {found.GetInfo()}");

        Console.WriteLine($"Удаление \"Термос\": {catalog.Remove("ТЕРМОС")}");
        Console.WriteLine($"Повторное удаление \"Термос\": {catalog.Remove("Термос")}");
        Console.WriteLine($"Товаров в каталоге: {catalog.GetCount()}");

        // GetProducts возвращает копию: изменить внутренний список снаружи нельзя.
        IReadOnlyList<Product> first = catalog.GetProducts();
        IReadOnlyList<Product> second = catalog.GetProducts();

        Console.WriteLine($"Два вызова GetProducts дают разные объекты списка: {!ReferenceEquals(first, second)}");
        Console.WriteLine($"Элементов в копии: {first.Count}, в каталоге: {catalog.GetCount()}");
        Console.WriteLine("catalog.GetProducts().Clear() не компилируется: у IReadOnlyList<T> нет метода Clear.");

        Section("3. Сохранение и загрузка через интерфейс (DIP)");

        // DIP: работаем через интерфейс, а не через конкретный класс.
        IProductRepository repository = new FileProductRepository("products.csv");

        repository.Save(catalog.GetProducts());
        Console.WriteLine("Каталог сохранён в products.csv через IProductRepository.");

        List<Product> loaded = repository.Load();
        Console.WriteLine($"Загружено товаров: {loaded.Count}");

        Section("4. Вывод и отчёты (SRP)");

        var printer = new CatalogPrinter();
        printer.PrintAll(loaded);
        Console.WriteLine();
        printer.PrintTop(loaded, 2);

        Console.WriteLine();
        Console.WriteLine($"Суммарная стоимость каталога: {new CatalogReport().GetSummary(loaded):F2} УЕ");
        Console.WriteLine($"Средняя цена товара: {new CatalogReport().GetAveragePrice(loaded):F2} УЕ");

        Section("5. Проверка целостности файлового хранилища");

        CheckBrokenFile();

        Section("6. Что не компилируется (проверено удалением строк из кода)");

        Console.WriteLine("product.price = -100;      → CS0122: 'Product.price' недоступен из-за уровня защиты");
        Console.WriteLine("catalog.products.Clear();  → CS0122: 'Catalog.products' недоступен из-за уровня защиты");
        Console.WriteLine("catalog.GetProducts().Clear(); → CS1061: 'IReadOnlyList<Product>' не содержит 'Clear'");
        Console.WriteLine("PriceFormatter.Format(1) из CatalogReport.cs → CS0103: имя 'PriceFormatter' не существует в текущем контексте");

        Console.WriteLine();
        Console.WriteLine("Компоновка завершена.");
    }

    private static void TryAdd(Catalog catalog, int id, string title, int price, string caseName)
    {
        try
        {
            catalog.Add(new Product(id, title, price));
            Console.WriteLine($"[НЕ ОЖИДАЛОСЬ] {caseName}: товар добавлен.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"[Отклонено] {caseName}: {ex.Message}");
        }
    }

    private static void CheckBrokenFile()
    {
        const string brokenPath = "products_broken.csv";

        // Одна корректная строка, одна битая, одна с недопустимой ценой, одна с повтором ID.
        File.WriteAllLines(brokenPath, new[]
        {
            "1;Кружка;450",
            "мусор",
            "2;Термос;-100",
            "1;Дубль;99"
        });

        IProductRepository repository = new FileProductRepository(brokenPath);
        List<Product> loaded = repository.Load();

        Console.WriteLine($"Из файла с 4 строками (1 корректная, 3 битых) загружено: {loaded.Count}");
        foreach (var product in loaded)
            Console.WriteLine($"  {product.GetInfo()}");

        Console.WriteLine($"Файл существует и прочитан без исключения: {File.Exists(brokenPath)}");
    }

    private static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 72));
        Console.WriteLine(title);
        Console.WriteLine(new string('=', 72));
    }
}
