using System;
using System.Collections.Generic;

using Marketplace.Domain;
using Marketplace.Services;

namespace Marketplace;

/// <summary>
/// Точка входа: демонстрация сценариев Задания 3.
/// </summary>
public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Section("1. Подготовка каталога и покупателя");

        var plate = new Product("Тарелка", 1000m);
        var device = new Product("Прибор", 2000m);
        var catalog = new List<Product> { plate, device };

        Console.WriteLine($"Созданы товары: {plate}, {device}");

        var user = new User("Иванов Иван");
        var profile = new UserProfile(user);
        profile.AddContact(new Contact(1, "@ivanov"));

        // По ТЗ: пополнение 10000 УЕ, бонусы 10% от суммы, доля бонусов 10%, скидка 5%.
        profile.AddMoney(10_000.0);
        profile.AddBonuses(10_000.0 * 0.10);
        profile.SetAllowedBonusRatio(0.10);
        profile.SetPersonalDiscount(0.95);

        var (money, bonuses, discount, purchaseCount) = profile;
        Console.WriteLine($"Покупатель: {profile.User.FullName}");
        Console.WriteLine($"  Баланс: {money:F2} УЕ, бонусов {bonuses:F2}, множитель скидки {discount:F2}, покупок {purchaseCount}");

        Section("2. Заказ товаров и наполнение корзины");

        var basket = new UserBasket(profile.User.Id);

        // 3 товара "Тарелка" по 1000 УЕ и 2 товара "Прибор" по 2000 УЕ.
        basket.AddPosition(new ProductPosition(plate.Id, 3));
        basket.AddPosition(new ProductPosition(device.Id, 2));

        Console.WriteLine($"В корзину добавлено позиций: {basket.Count}");

        Section("3. Расчётная цена с учётом персональной скидки");

        CalculatedPrice price = basket.CalculatePrice(catalog, (decimal)profile.PersonalDiscount);
        Console.WriteLine($"Показать итоговую цену: {price.FinalPrice:F2} УЕ");
        Console.WriteLine($"Показать размер скидки в деньгах: {price.DiscountAmount:F2} УЕ");
        Console.WriteLine($"({price})");

        Section("4. Копирование ссылки и значения");

        // Класс - ссылочный тип: две переменные указывают на один объект.
        var positionA = new ProductPosition(plate.Id, 1);
        var positionB = positionA;
        positionB.Quantity = 5;
        Console.WriteLine($"Класс: positionA.Quantity = {positionA.Quantity}, positionB.Quantity = {positionB.Quantity} - объект один.");

        // Структура - значимый тип: копия независима, оригинал не меняется.
        var priceBefore = new CalculatedPrice(1000m, 0.90m);
        CalculatedPrice priceCopy = priceBefore;
        priceCopy = new CalculatedPrice(500m, 0.90m);
        Console.WriteLine($"Структура: оригинал {priceBefore.FinalPrice:F2} УЕ, копия {priceCopy.FinalPrice:F2} УЕ - оригинал не изменился.");

        // Копирование позиции внутри корзины: изменение копии не портит оригинал.
        // Используется отдельная корзина, чтобы не менять состав основной.
        var demoBasket = new UserBasket(profile.User.Id);
        var original = new ProductPosition(device.Id, 2);
        demoBasket.AddPosition(original);
        original.Quantity = 99;
        Console.WriteLine($"Корзина хранит копию: в корзине позиция с Quantity = "
                          + $"{demoBasket.GetPositions().Find(p => p.Id == original.Id)!.Quantity}, оригинал изменён на {original.Quantity}.");

        Section("5. Неуспешная оплата: нехватка средств");

        // Скидка убрана, дорогая корзина, денег заведомо не хватает.
        var poorUser = new User("Петров Пётр");
        var poorProfile = new UserProfile(poorUser);
        poorProfile.AddMoney(500.0);
        poorProfile.SetPersonalDiscount(1.00);

        var poorBasket = new UserBasket(poorProfile.User.Id);
        poorBasket.AddPosition(new ProductPosition(device.Id, 3));

        int positionsBefore = poorBasket.Count;
        var poorService = new OrderService();
        PaymentResult failed = poorService.Checkout(poorProfile, poorBasket, catalog);

        Console.WriteLine($"Успех: {failed.Success}");
        Console.WriteLine($"Сообщение: {failed.Message}");
        Console.WriteLine($"Корзина сохранена: {poorBasket.Count} позиций (было {positionsBefore}); баланс: {poorProfile.MoneyBalance:F2} УЕ");

        Section("6. Пополнение баланса и успешная оплата");

        decimal missing = failed.Price.FinalPrice - (decimal)poorProfile.MoneyBalance;
        Console.WriteLine($"Не хватало {missing:F2} УЕ - пополняем.");
        poorProfile.AddMoney((double)missing);

        PaymentResult success = poorService.Checkout(poorProfile, poorBasket, catalog);
        Console.WriteLine($"Успех: {success.Success}");
        Console.WriteLine($"Сообщение: {success.Message}");
        Console.WriteLine($"Корзина опустошена: {poorBasket.IsEmpty}, итоговая цена пустой корзины: "
                          + $"{poorBasket.CalculatePrice(catalog, 1.00m).FinalPrice:F2} УЕ");
        Console.WriteLine($"Баланс после оплаты: {poorProfile.MoneyBalance:F2} УЕ, бонусов {poorProfile.BonusBalance:F2}");
        Console.WriteLine($"Покупок в истории: {poorProfile.Purchases.Count}");

        Section("7. Демонстрация with для Состояния Баланса");

        BalanceState stateBefore = new BalanceState(bonuses: 500.0, money: 2000.0, bonusPaymentRatio: 0.10);
        BalanceState stateAfter = stateBefore with { Money = 1500.0 };

        Console.WriteLine($"До:    {stateBefore}");
        Console.WriteLine($"После: {stateAfter}");
        Console.WriteLine($"Исходное состояние не изменилось: {stateBefore.Money:F2} УЕ");

        Section("8. Оплата основной корзиной покупателя");

        PaymentResult mainPayment = new OrderService().Checkout(profile, basket, catalog);
        Console.WriteLine($"Успех: {mainPayment.Success}");
        Console.WriteLine($"Сообщение: {mainPayment.Message}");
        Console.WriteLine($"Баланс: {profile.MoneyBalance:F2} УЕ, бонусов {profile.BonusBalance:F2}");
        Console.WriteLine($"Корзина: {basket.Count} позиций, пуста: {basket.IsEmpty}");
        Console.WriteLine($"Покупок в истории: {profile.Purchases.Count}");

        Section("9. Отчёт об операциях оплаты");

        PrintReport(new[] { failed, success, mainPayment });

        Section("10. Проверка валидации (ожидаем исключения)");

        ExpectThrow("Пустое название товара", () => new Product("   ", 100m));
        ExpectThrow("Отрицательная цена", () => new Product("Товар", -1m));
        ExpectThrow("Нулевое количество", () => new ProductPosition(plate.Id, 0));
        ExpectThrow("Скидка 0.00", () => profile.SetPersonalDiscount(0.00));
        ExpectThrow("Доля бонусов 0.75", () => profile.SetAllowedBonusRatio(0.75));
        ExpectThrow("Пополнение на NaN", () => profile.AddMoney(double.NaN));
        ExpectThrow("Множитель скидки 1.5", () => new CalculatedPrice(100m, 1.5m));
        ExpectThrow("Отрицательный баланс бонусов", () => new BalanceState(-1.0, 0.0, 0.0));
        ExpectThrow("Нет товара в каталоге", () =>
        {
            var isolated = new UserBasket(profile.User.Id);
            isolated.AddPosition(new ProductPosition(device.Id, 1));
            isolated.CalculatePrice(new List<Product> { plate }, 1.00m);
        });
    }

    private static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine(title);
        Console.WriteLine(new string('=', 70));
    }

    private static void PrintReport(IEnumerable<PaymentResult> results)
    {
        Console.WriteLine($"{"Успех",-8}{"Итоговая цена",15}{"Бонусы",12}{"Деньги",12}  Сообщение");
        Console.WriteLine(new string('-', 100));

        foreach (var result in results)
        {
            Console.WriteLine($"{result.Success,-8}{result.Price.FinalPrice,15:F2}{result.BonusesSpent,12:F2}"
                              + $"{result.MoneySpent,12:F2}  {result.Message}");
        }
    }

    private static void ExpectThrow(string title, Action action)
    {
        try
        {
            action();
            Console.WriteLine($"[НЕ ОЖИДАЛОСЬ] {title}: исключения не было");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OK] {title}: {ex.GetType().Name} - {ex.Message}");
        }
    }
}
