using Marketplace;
using Marketplace.Domain;

internal static class Program
{
    private static int checks;

    private static void Main()
    {
        CheckProductsAndBasket();
        CheckBalanceAndProfile();
        CheckPaymentIntegration();
        Console.WriteLine($"PASS: {checks} проверок.");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
        checks++;
    }

    private static void Reject<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { checks++; return; }
        throw new InvalidOperationException($"Ожидалось исключение {typeof(T).Name}.");
    }

    private static UserProfile CreateProfile(int id = 1) => new(new User(id, "Иван Иванов", "ivan", "test-hash"));

    private static void CheckProductsAndBasket()
    {
        var plate = new Product(" тарелка ", 1000m);
        var cutlery = new Product("приборы", 2000m);
        Product[] products = [plate, cutlery];
        var basket = new UserBasket(1);
        var position = new ProductPosition(plate.Id, 3);
        basket.AddPosition(position);
        basket.AddPosition(new ProductPosition(cutlery.Id, 2));
        var price = basket.CalculatePrice(products, 0.95m);
        Check(plate.Name == "Тарелка" && plate.Id != cutlery.Id, "Данные товара.");
        Check(price.OriginalPrice == 7000m && price.FinalPrice == 6650m && price.DiscountAmount == 350m, "Расчет задания.");
        Check(price.GetFinalPrice() == 6650m && price.GetDiscountAmount() == 350m, "Методы расчетной цены.");
        Check(basket.CalculatePrice(products, 1m).FinalPrice == 7000m, "Без скидки.");
        Check(basket.CalculatePrice(products, 0.01m).FinalPrice == 70m, "Скидка 99%.");
        position.Quantity = 20;
        var snapshot = basket.GetPositions();
        Check(snapshot[0].Quantity == 3 && snapshot[0].Id == position.Id, "Копирование при добавлении.");
        snapshot[0].Quantity = 30;
        snapshot.Clear();
        Check(basket.GetPositions().Count == 2 && basket.GetPositions()[0].Quantity == 3, "Независимость снимка.");
        Reject<ArgumentNullException>(() => basket.AddPosition(null!));
        Reject<ArgumentException>(() => basket.AddPosition(position));
        Reject<ArgumentNullException>(() => basket.CalculatePrice(null!, 1m));
        Reject<ArgumentException>(() => basket.CalculatePrice([plate, plate], 1m));
        Reject<ArgumentException>(() => basket.CalculatePrice([plate, null!], 1m));
        Reject<InvalidOperationException>(() => basket.CalculatePrice([plate], 1m));
        foreach (decimal invalid in new[] { -1m, 0m, 0.009m, 1.01m })
            Reject<ArgumentOutOfRangeException>(() => basket.CalculatePrice(products, invalid));
        Check(basket.CalculatePrice(products, 1m).OriginalPrice == 7000m, "Ошибки не меняют корзину.");
        foreach (string? invalid in new string?[] { null, "", " \t" })
            Reject<ArgumentException>(() => new Product(invalid!, 1m));
        Reject<ArgumentOutOfRangeException>(() => new Product("X", -1m));
        Reject<ArgumentOutOfRangeException>(() => plate.Price = -1m);
        Check(plate.Price == 1000m, "Неудачное изменение цены.");
        Check(new Product("X", 0m).Price == 0m, "Бесплатный товар.");
        Reject<ArgumentOutOfRangeException>(() => new UserBasket(0));
        Reject<ArgumentOutOfRangeException>(() => new ProductPosition(0, 1));
        Reject<ArgumentOutOfRangeException>(() => new ProductPosition(1, 0));
        Reject<ArgumentOutOfRangeException>(() => position.Quantity = 0);
        Check(position.Quantity == 20, "Неудачное изменение количества.");
        plate.Price = 1200m;
        Check(price.FinalPrice == 6650m && basket.CalculatePrice(products, 1m).OriginalPrice == 7600m, "Расчетная цена — снимок.");
        var priceCopy = price with { };
        Check(priceCopy.FinalPrice == price.FinalPrice, "Копирование расчетной цены.");
        Check(default(CalculatedPrice).FinalPrice == 0m && default(CalculatedPrice).DiscountMultiplier == 1m, "default цены.");
        Reject<ArgumentOutOfRangeException>(() => new CalculatedPrice(-1m, 1m));
        Check(new CalculatedPrice(0.1m + 0.2m, 0.95m).FinalPrice == 0.285m, "Точность decimal.");
        int positionId = basket.GetPositions()[0].Id;
        Check(basket.RemovePosition(positionId) && !basket.RemovePosition(positionId), "Удаление позиции.");
        var sameBasket = basket;
        basket.Clear();
        Check(sameBasket.GetPositions().Count == 0 && basket.CalculatePrice(products, 1m).FinalPrice == 0m, "Копирование ссылки.");
        Reject<ArgumentOutOfRangeException>(() => basket.CalculatePrice(products, 0m));
        var expensive = new Product("X", decimal.MaxValue);
        basket.AddPosition(new ProductPosition(expensive.Id, 2));
        Reject<OverflowException>(() => basket.CalculatePrice([expensive], 1m));
        Check(basket.GetPositions().Count == 1, "Переполнение не меняет корзину.");
    }

    private static void CheckBalanceAndProfile()
    {
        var profile = CreateProfile();
        Check(profile.User.Id == profile.Basket.UserId && profile.PersonalDiscount == 1m, "Связь профиля и корзины.");
        Check(profile.MoneyBalance == 0m && profile.BonusBalance == 0m && profile.AllowedBonusRatio == 0m, "Начальный баланс.");
        Reject<ArgumentNullException>(() => new UserProfile(null!));
        Reject<ArgumentOutOfRangeException>(() => CreateProfile(0));
        profile.AddMoney(0.1m); profile.AddMoney(0.2m); profile.AddBonuses(10m);
        Check(profile.MoneyBalance == 0.3m && profile.BonusBalance == 10m, "Пополнение.");
        foreach (decimal invalid in new[] { 0m, -1m })
        {
            Reject<ArgumentOutOfRangeException>(() => profile.AddMoney(invalid));
            Reject<ArgumentOutOfRangeException>(() => profile.AddBonuses(invalid));
        }
        foreach (decimal invalid in new[] { 0m, 0.009m, 1.01m })
            Reject<ArgumentOutOfRangeException>(() => profile.SetPersonalDiscount(invalid));
        profile.SetPersonalDiscount(0.01m); profile.SetPersonalDiscount(1m);
        Check(profile.PersonalDiscount == 1m, "Границы скидки.");
        foreach (decimal invalid in new[] { -0.01m, 0.51m })
        {
            Reject<ArgumentOutOfRangeException>(() => profile.SetAllowedBonusRatio(invalid));
            Reject<ArgumentOutOfRangeException>(() => new BalanceState(0m, 0m, invalid));
        }
        profile.SetAllowedBonusRatio(0.5m);
        Check(profile.AllowedBonusRatio == 0.5m, "Максимум бонусной доли.");
        Reject<ArgumentOutOfRangeException>(() => new BalanceState(-1m, 0m, 0m));
        Reject<ArgumentOutOfRangeException>(() => new BalanceState(0m, -1m, 0m));
        BalanceState before = profile.GetBalance();
        BalanceState after = before with { Money = 100m, Bonuses = 20m, BonusPaymentRatio = 0.1m };
        Check(before.Money == 0.3m && after.Money == 100m, "with не меняет оригинал.");
        Reject<ArgumentOutOfRangeException>(() => { _ = after with { Money = -1m }; });
        Reject<ArgumentOutOfRangeException>(() => { _ = after with { Bonuses = -1m }; });
        Reject<ArgumentOutOfRangeException>(() => { _ = after with { BonusPaymentRatio = 0.6m }; });
        profile.ApplyNewBalance(after);
        Check(profile.MoneyBalance == 100m && profile.BonusBalance == 20m && profile.AllowedBonusRatio == 0.1m, "Применение баланса.");
        Check(default(BalanceState).Money == 0m && default(BalanceState).Bonuses == 0m, "default баланса.");
        var contact = new Contact(1, 1, "mail@example.com");
        profile.AddContact(contact);
        Reject<ArgumentException>(() => profile.AddContact(contact));
        Reject<ArgumentNullException>(() => profile.AddContact(null!));
        Reject<NotSupportedException>(() => ((IList<Contact>)profile.Contacts).Clear());
        var (userId, name, info) = profile;
        Check(userId == 1 && name == "Иван Иванов" && info.Contains("mail@example.com"), "Деконструктор контактов.");
        Check(profile.RemoveContact(1) && !profile.RemoveContact(1), "Удаление контакта.");
        var item = new ProductPosition(1, 2);
        profile.AddPurchases([item]); item.Quantity = 10;
        Check(profile.Purchases[0].Quantity == 2, "История копирует входные данные.");
        profile.Purchases[0].Quantity = 50;
        Check(profile.Purchases[0].Quantity == 2, "Чтение истории не меняет оригинал.");
        Reject<NotSupportedException>(() => ((IList<ProductPosition>)profile.Purchases).Clear());
        Reject<ArgumentNullException>(() => profile.AddPurchases(null!));
        Reject<ArgumentException>(() => profile.AddPurchases([item, null!]));
        Check(profile.Purchases.Count == 1, "Невалидный набор не добавляется частично.");
        Reject<InvalidOperationException>(() => profile.AddPurchases(BrokenItems(item)));
        Check(profile.Purchases.Count == 1, "Ошибка перечисления не меняет историю.");
        var (money, bonuses, discount, purchaseCount) = profile;
        Check(money == 100m && bonuses == 20m && discount == 1m && purchaseCount == 1, "Деконструктор финансов.");
        profile.ApplyNewBalance(new BalanceState(0m, decimal.MaxValue, 0m));
        Reject<OverflowException>(() => profile.AddMoney(1m));
        Check(profile.MoneyBalance == decimal.MaxValue, "Переполнение не меняет баланс.");
    }

    private static IEnumerable<ProductPosition> BrokenItems(ProductPosition item)
    {
        yield return item;
        throw new InvalidOperationException("Ошибка источника.");
    }

    private static void CheckPaymentIntegration()
    {
        var profile = CreateProfile();
        var plate = new Product("Тарелка", 1000m);
        var cutlery = new Product("Приборы", 2000m);
        Product[] products = [plate, cutlery];
        profile.AddMoney(10000m); profile.AddBonuses(1000m);
        profile.SetPersonalDiscount(0.95m); profile.SetAllowedBonusRatio(0.1m);
        profile.Basket.AddPosition(new ProductPosition(plate.Id, 3));
        profile.Basket.AddPosition(new ProductPosition(cutlery.Id, 2));
        var price = profile.CalculateBasketPrice(products);
        var balance = profile.GetBalance();
        Check(TryPay(ref balance, price), "Успешная оплата.");
        Check(balance.Money == 4015m && balance.Bonuses == 335m, "Списано 5985 денег и 665 бонусов.");
        Check(profile.MoneyBalance == 10000m, "Копия баланса не меняет профиль до применения.");
        profile.AddPurchases(profile.Basket.GetPositions());
        profile.ApplyNewBalance(balance); profile.Basket.Clear();
        Check(profile.Purchases.Count == 2 && profile.CalculateBasketPrice(products).FinalPrice == 0m, "История и очистка после оплаты.");
        profile.Basket.AddPosition(new ProductPosition(cutlery.Id, 10));
        BalanceState before = profile.GetBalance(); balance = before;
        Check(!TryPay(ref balance, profile.CalculateBasketPrice(products)), "Отказ при нехватке денег.");
        Check(balance.Equals(before) && profile.GetBalance().Equals(before), "При отказе баланс не меняется.");
        Check(profile.Basket.GetPositions().Count == 1 && profile.Purchases.Count == 2, "Отказ сохраняет корзину и покупки.");
        profile.AddMoney(20000m); balance = profile.GetBalance();
        Check(TryPay(ref balance, profile.CalculateBasketPrice(products)), "Повтор после пополнения.");
        Check(balance.Money == 5350m && balance.Bonuses == 0m, "Недостающие бонусы доплачены деньгами.");
        Check(profile.Purchases[1].Quantity == 2, "Новая корзина не меняет прошлые покупки.");
        balance = new BalanceState(100m, 100m, 0m);
        Check(TryPay(ref balance, new CalculatedPrice(100m, 1m)) && balance.Money == 0m && balance.Bonuses == 100m, "Доля 0%.");
        balance = new BalanceState(50m, 50m, 0.5m);
        Check(TryPay(ref balance, new CalculatedPrice(100m, 1m)) && balance.Money == 0m && balance.Bonuses == 0m, "Доля 50% и точный баланс.");
    }

    // Тестовый пример контракта сервиса. Производственный сервис остается задачей его автора.
    private static bool TryPay(ref BalanceState balance, CalculatedPrice price)
    {
        decimal bonuses = Math.Min(balance.Bonuses, price.FinalPrice * balance.BonusPaymentRatio);
        decimal money = price.FinalPrice - bonuses;
        if (balance.Money < money)
            return false;
        balance = balance with { Bonuses = balance.Bonuses - bonuses, Money = balance.Money - money };
        return true;
    }
}
