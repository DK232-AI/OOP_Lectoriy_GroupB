using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Domain;
using Marketplace.Services;

namespace Marketplace;

/// <summary>
/// Точка входа занятия 4: демонстрация видов свойств, перегрузок и защиты данных.
/// </summary>
public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        DemoProducts();
        DemoCustomers();
        DemoCartAndReports();
        DemoDiscount();
        DemoInvalidValues();
        DemoImmutability();
    }

    // ---------- 1. Товары: четыре вида свойств ----------

    private static void DemoProducts()
    {
        Section("1. Товары: обязательные свойства заданы в инициализаторах");

        // Создание через конструктор без параметров: required Title задаётся
        // в объектном инициализаторе — иначе код не скомпилируется.
        var cup = new Product { Title = "Кружка", Price = 450 };
        cup.Article = "A-001";

        var thermos = new Product { Title = "Термос", Price = 1200 };
        var stickers = new Product { Title = "Набор стикеров", Price = 20 };

        var products = new List<Product> { cup, thermos, stickers };

        foreach (var product in products)
            Console.WriteLine($"  {product.GetInfo()}");

        Section("2. Вычисляемое свойство IsBudget и оба варианта GetInfo");

        foreach (var product in products)
        {
            Console.WriteLine($"{product.Title}: IsBudget = {product.IsBudget} (цена {product.Price})");
            Console.WriteLine($"    GetInfo()      → {product.GetInfo()}");
            Console.WriteLine($"    GetInfo(false) → {product.GetInfo(false)}");
        }

        Console.WriteLine();
        Console.WriteLine("IsBudget — вычисляемое: сеттера нет, значение пересчитывается из Price.");
        Console.WriteLine("Изменение Price меняет IsBudget без дополнительного кода:");
        Console.WriteLine($"  до:    Price={stickers.Price}, IsBudget={stickers.IsBudget}");
        stickers.Price = 120;
        Console.WriteLine($"  после: Price={stickers.Price}, IsBudget={stickers.IsBudget}");

        Section("3. Свойство Status меняется только через ChangeStatus");

        Console.WriteLine($"Статус кружки: {cup.Status}");
        cup.ChangeStatus("Под заказ");
        Console.WriteLine($"После ChangeStatus(\"Под заказ\"): {cup.Status}");
        Console.WriteLine("Присваивание cup.Status = \"Продан\" не компилируется — сеттер private set.");
    }

    // ---------- 2. Покупатели ----------

    private static void DemoCustomers()
    {
        Section("4. Покупатели: required FullName, инициализатор Phone, проверки в сеттерах");

        var first = new CustomerProfile
        {
            FullName = "Иванов Иван",
            Email = "ivanov@example.com",
            Phone = "  +7 900 123-45-67  ",
            Rating = 5
        };

        var second = new CustomerProfile
        {
            FullName = "Петрова Мария",
            Email = "petrova@example.com",
            Rating = 2
        };

        foreach (var customer in new[] { first, second })
        {
            Console.WriteLine($"{customer.GetInfo()}");
            Console.WriteLine($"    GetInfo()      → {customer.GetInfo()}");
            Console.WriteLine($"    GetInfo(false) → {customer.GetInfo(false)}");
            Console.WriteLine($"    Контакты с телефоном: {customer.GetContactInfo(true)}");
            Console.WriteLine($"    Контакты без телефона: {customer.GetContactInfo(false)}");
            Console.WriteLine($"    IsTrusted = {customer.IsTrusted}");
            Console.WriteLine();
        }
    }

    // ---------- 3. Корзина и отчёты ----------

    private static void DemoCartAndReports()
    {
        Section("5. Корзина и отчёты через CheckoutService");

        var items = new List<CartItem>
        {
            new(1, 2, 450),      // 900
            new(2, 1, 1200),     // 1200
            new(3, 5, 120),      // 600
            default              // значение по умолчанию: не является позицией покупки
        };

        var checkout = new CheckoutService();

        Console.WriteLine($"Итоговая сумма всех позиций: {checkout.GetTotal(items)} УЕ");
        Console.WriteLine("default(CartItem) создан без конструктора и в сумму не даёт вклада: его Total = 0.");
        Console.WriteLine();

        Console.WriteLine("Корзина считается пустой? Нет — 3 корректные позиции из 4.");
        Console.WriteLine();

        Console.WriteLine("--- Краткий отчёт GetReport(items) ---");
        Console.WriteLine(checkout.GetReport(items));
        Console.WriteLine();

        Console.WriteLine("--- Полный отчёт GetReport(items, true) ---");
        Console.WriteLine(checkout.GetReport(items, true));
        Console.WriteLine();

        var cheaper = checkout.FindCheaperThan(items, 500);
        Console.WriteLine($"Позиции дешевле 500 УЕ за единицу: {cheaper.Count} шт.");
        foreach (var item in cheaper)
            Console.WriteLine($"  Товар #{item.ProductId}, цена {item.PricePerUnit} УЕ");
        Console.WriteLine();

        Console.WriteLine("--- Пустая корзина ---");
        Console.WriteLine(checkout.GetReport(new List<CartItem>()));
    }

    // ---------- 4. Скидки ----------

    private static void DemoDiscount()
    {
        Section("6. Скидка: обе перегрузки Calculate");

        var discount = new Discount
        {
            Name = "Осенняя распродажа",
            Percent = 15m,
            MinSum = 1000,
            ValidUntil = DateTime.Now.AddDays(7)
        };

        Console.WriteLine($"Скидка: {discount.Name}");
        Console.WriteLine($"  Percent = {discount.Percent}%, MinSum = {discount.MinSum} УЕ");
        Console.WriteLine($"  ValidUntil = {discount.ValidUntil:dd.MM.yyyy HH:mm}, IsActive = {discount.IsActive}");
        Console.WriteLine();

        // Короткая перегрузка использует собственный Percent.
        int total = 5000;
        Console.WriteLine($"Calculate({total})              → {discount.Calculate(total)} УЕ (Percent = {discount.Percent}%)");

        // Полная перегрузка принимает процент параметром.
        Console.WriteLine($"Calculate({total}, 30)          → {discount.Calculate(total, 30m)} УЕ");
        Console.WriteLine($"Calculate({total}, 0)           → {discount.Calculate(total, 0m)} УЕ");
        Console.WriteLine();

        // Минимальная сумма не набрана — скидка не применяется.
        int smallTotal = 800;
        Console.WriteLine($"Calculate({smallTotal})            → {discount.Calculate(smallTotal)} УЕ — скидка не применена, сумма меньше MinSum ({discount.MinSum}).");
        Console.WriteLine();

        Console.WriteLine("Проверка арифметики полной перегрузки:");
        Console.WriteLine($"  {total} × 15% = {total * 15m / 100m:F2} — скидка, к оплате {total - total * 15m / 100m:F0} УЕ");
    }

    // ---------- 5. Недопустимые значения ----------

    private static void DemoInvalidValues()
    {
        Section("7. Недопустимые значения отклоняются сеттерами");

        var product = new Product { Title = "Проверочный товар", Price = 100 };
        var customer = new CustomerProfile { FullName = "Сидоров Сидор", Email = "sidorov@example.com", Rating = 3 };

        Try("запись отрицательной цены", () => product.Price = -10);
        Console.WriteLine($"    Цена осталась: {product.Price} УЕ");
        Console.WriteLine();

        Try("запись пустого артикула", () => product.Article = "   ");
        Console.WriteLine("    Артикул остался прежним.");
        Console.WriteLine();

        Try("запись недопустимого рейтинга", () => customer.Rating = 7);
        Console.WriteLine($"    Рейтинг остался: {customer.Rating}");
        Console.WriteLine();

        Try("запись недопустимого статуса", () => product.ChangeStatus("Продан"));
        Console.WriteLine($"    Статус остался: {product.Status}");
        Console.WriteLine();

        Try("запись email без символа @", () => customer.Email = "неверный-email");
        Console.WriteLine($"    Email остался: {customer.Email}");
        Console.WriteLine();

        Try("скидка больше 100 процентов", () => new Discount { Name = "Ошибка", Percent = 150m });
        Console.WriteLine();

        Try("отрицательный процент в полной перегрузке", () => new Discount { Name = "Норм", Percent = 10m }.Calculate(1000, -5m));
    }

    private static void Try(string situation, Action action)
    {
        try
        {
            action();
            Console.WriteLine($"[НЕ ОЖИДАЛОСЬ] {situation}: значение принято.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Отклонено] {situation}: {ex.GetType().Name} — {ex.Message}");
        }
    }

    // ---------- 6. Неизменяемость ----------

    private static void DemoImmutability()
    {
        Section("8. Неизменяемые свойства после создания объекта");

        var product = new Product { Title = "Кружка", Price = 450 };
        Console.WriteLine($"Товар создан: {product.GetInfo()}");
        Console.WriteLine();

        // Иллюстрация попытки вызвать сеттер для неизменяемого свойства.
        // product.Title = "Другая кружка";
        // ↑ CS0200: Property or indexer 'Product.Title' cannot be assigned to --
        //           it is read only (свойство объявлено с init)

        // Иллюстрация попытки вызвать сеттер для неизменяемого свойства.
        // product.Status = "Продан";
        // ↑ CS0200: Property or indexer 'Product.Status' cannot be assigned to --
        //           it is read only (у свойства закрытый сеттер)

        // Иллюстрация попытки создать товар без обязательного свойства.
        // var broken = new Product { Price = 100 };
        // ↑ CS9035: Required member 'Product.Title' must be set in the object initializer
        //           or attribute constructor

        // Иллюстрация попытки изменить свойство структуры после создания.
        // var item = new CartItem(1, 2, 450);
        // item.Quantity = 5;
        // ↑ CS0200: Property or indexer 'CartItem.Quantity' cannot be assigned to --
        //           it is read only (readonly struct, свойства только для чтения)

        Console.WriteLine("Попытки изменить Title, Status, Quantity и создать объект без Title");
        Console.WriteLine("закомментированы в коде — все четыре строки не компилируются.");
        Console.WriteLine("Тексты ошибок компилятора приведены в отчёте.");

        // Значение структуры можно заменить целиком — это создание новой копии.
        var item2 = new CartItem(1, 2, 450);
        Console.WriteLine($"Копия структуры CartItem: ProductId={item2.ProductId}, Quantity={item2.Quantity}, Total={item2.Total} УЕ");
        Console.WriteLine("Полю структуры присвоить нельзя, но переменную можно заменить новым значением.");
    }

    private static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 74));
        Console.WriteLine(title);
        Console.WriteLine(new string('=', 74));
    }
}
