using System;
using System.Collections.Generic;
using Marketplace.Domain;
using Marketplace.Services;

namespace Marketplace;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        DemoProductStatics();
        DemoPriceRules();
        DemoMoney();
        DemoOrderNumberGenerator();
        DemoReceipt();
        DemoReadonly();
        DemoInvalidInputs();
    }

    private static void DemoProductStatics()
    {
        Section("1. Статические члены Product");

        Console.WriteLine($"Product.MaxPrice = {Product.MaxPrice} (const, с большой буквы)");
        Console.WriteLine($"Product.CatalogStartedAt = {Product.CatalogStartedAt} (static readonly DateTime)");
        Console.WriteLine();

        Console.WriteLine("Создаю товары:");
        var p1 = new Product { Title = "Кружка", Article = "A-001", Price = 450 };
        var p2 = new Product { Title = "Термос", Article = "A-002", Price = 1200 };
        var p3 = new Product { Title = "Стикеры", Article = "A-003", Price = 20 };

        Console.WriteLine($"  Товар 1: ID = {p1.Id}, CreatedAt = {p1.CreatedAt:O}");
        Console.WriteLine($"  Товар 2: ID = {p2.Id}");
        Console.WriteLine($"  Товар 3: ID = {p3.Id}");
        Console.WriteLine();

        Console.WriteLine($"Product.CreatedCount = {Product.CreatedCount} (static property)");
        Console.WriteLine($"Уникальные ID: {p1.Id != p2.Id && p2.Id != p3.Id && p1.Id != p3.Id}");
        Console.WriteLine();
    }

    private static void DemoPriceRules()
    {
        Section("2. Статический класс PriceRules");

        Console.WriteLine($"PriceRules.VatRate = {PriceRules.VatRate} (const decimal)");
        Console.WriteLine($"PriceRules.MaxDiscountPercent = {PriceRules.MaxDiscountPercent} (const int)");
        Console.WriteLine($"PriceRules.DefaultCurrency = \"{PriceRules.DefaultCurrency}\" (static readonly string)");
        Console.WriteLine();

        int price = 1000;
        int priceWithVat = PriceRules.WithVat(price);
        Console.WriteLine($"PriceRules.WithVat({price}) = {priceWithVat}");

        int discounted = PriceRules.ApplyDiscount(price, 15);
        Console.WriteLine($"PriceRules.ApplyDiscount({price}, 15) = {discounted}");
        Console.WriteLine();
    }

    private static void DemoMoney()
    {
        Section("3. Readonly struct Money");

        var m1 = new Money(1500m, "RUB");
        Console.WriteLine($"m1 = new Money(1500m, \"RUB\") → {m1}");

        var m2 = Money.FromRubles(2500m);
        Console.WriteLine($"m2 = Money.FromRubles(2500m) → {m2}");

        Console.WriteLine($"Money.Zero = {Money.Zero} (static readonly)");

        var m3 = m1.Add(m2);
        Console.WriteLine($"m1.Add(m2) → {m3}");
        Console.WriteLine();
    }

    private static void DemoOrderNumberGenerator()
    {
        Section("4. Статический класс OrderNumberGenerator");

        Console.WriteLine($"OrderNumberGenerator.LastNumber = {OrderNumberGenerator.LastNumber}");
        int order1 = OrderNumberGenerator.Next();
        int order2 = OrderNumberGenerator.Next();
        Console.WriteLine($"OrderNumberGenerator.Next() → {order1}");
        Console.WriteLine($"OrderNumberGenerator.Next() → {order2}");
        Console.WriteLine($"OrderNumberGenerator.LastNumber = {OrderNumberGenerator.LastNumber}");
        Console.WriteLine();
    }

    private static void DemoReceipt()
    {
        Section("5. Статический сервис ReceiptService");

        var cart = new Cart();
        cart.Add(new CartItem(1, 2, 450));
        cart.Add(new CartItem(2, 1, 1200));
        string receipt = ReceiptService.BuildReceipt(cart.Items);
        Console.WriteLine(receipt);
        Console.WriteLine($"Номер после чека: {OrderNumberGenerator.LastNumber}");
    }

    private static void DemoReadonly()
    {
        Section("6. Readonly поля и readonly struct");

        var product = new Product { Title = "Товар", Article = "TEST", Price = 100 };
        Console.WriteLine($"product.CreatedAt = {product.CreatedAt:O} (readonly поле)");

        var m = new Money(100m, "RUB");
        Console.WriteLine($"m = Money(100m, \"RUB\") — readonly struct");
        Console.WriteLine("  Все свойства get-only, изменение только через новую копию");
        // product.Id = 10;                 // CS0200: у свойства нет сеттера.
        // product.CreatedAt = DateTime.Now; // CS0200: у свойства нет сеттера.
        // product.Price = -1;               // ArgumentOutOfRangeException.
        // product.Price = 2_000_000;        // ArgumentOutOfRangeException.
        // Product.MaxPrice = 10;            // CS0131: присваивание константе.
        // Product.CatalogStartedAt = DateTime.UtcNow; // CS0198: static readonly.
        // m.Amount = 500m;                  // CS0200: свойство get-only в readonly struct.
        Console.WriteLine();
    }

    private static void DemoInvalidInputs()
    {
        Section("7. Проверки исключений и независимости значения Money");
        Try("Отрицательная цена", () => new Product { Title = "Ошибка", Price = -1 });
        Try("Цена выше MaxPrice", () => new Product { Title = "Ошибка", Price = Product.MaxPrice + 1 });
        Try("Скидка выше 70%", () => PriceRules.ApplyDiscount(1000, 71));
        Try("Отрицательная цена в PriceRules", () => PriceRules.WithVat(-1));
        Try("Разные валюты", () => new Money(10m, "RUB").Add(new Money(5m, "USD")));
        var original = Money.FromRubles(1000m);
        var copy = original.Add(Money.FromRubles(500m));
        Console.WriteLine($"Money: исходное {original}, новая сумма {copy}; исходное не меняется.");

        // Проверка ошибок компиляции (строки не должны быть активны):
        // product.Id = 42;                  // нет публичного сеттера.
        // Product.MaxPrice = 1;              // константе нельзя присваивать.
        // Product.CatalogStartedAt = DateTime.UtcNow; // static readonly.
        // original.Amount = 2m;              // свойство get-only.
    }

    private static void Try(string situation, Action action)
    {
        try { action(); Console.WriteLine($"ОШИБКА: {situation} принято"); }
        catch (ArgumentException ex) { Console.WriteLine($"Отклонено: {situation} — {ex.GetType().Name}"); }
        catch (InvalidOperationException ex) { Console.WriteLine($"Отклонено: {situation} — {ex.GetType().Name}"); }
    }

    private static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine(title);
        Console.WriteLine(new string('=', 70));
    }
}
