using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Domain;

public class UserProfile
{
    private readonly List<Contact> contacts = new();
    private readonly List<ProductPosition> purchases = new();
    private BalanceState balance;

    public User User { get; }
    public UserBasket Basket { get; }
    public IReadOnlyList<Contact> Contacts => contacts.AsReadOnly();
    public IReadOnlyList<ProductPosition> Purchases => purchases.ConvertAll(item => item.Copy()).AsReadOnly();

    public decimal MoneyBalance => balance.Money;
    public decimal BonusBalance => balance.Bonuses;
    public decimal PersonalDiscount { get; private set; } = 1m;
    public decimal AllowedBonusRatio => balance.BonusPaymentRatio;

    public UserProfile(User user)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        Basket = new UserBasket(user.Id);
    }

    public void AddContact(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        if (contacts.Contains(contact))
            throw new ArgumentException("Контакт уже добавлен.", nameof(contact));
        contacts.Add(contact);
    }

    public bool RemoveContact(int contactId) => contacts.RemoveAll(contact => contact.Id == contactId) > 0;

    public void AddMoney(decimal amount)
    {
        if (amount <= 0m)
            throw new ArgumentOutOfRangeException(nameof(amount), "Сумма пополнения должна быть положительной.");
        balance = balance with { Money = balance.Money + amount };
    }

    public void AddBonuses(decimal amount)
    {
        if (amount <= 0m)
            throw new ArgumentOutOfRangeException(nameof(amount), "Сумма бонусов должна быть положительной.");
        balance = balance with { Bonuses = balance.Bonuses + amount };
    }

    public void SetPersonalDiscount(decimal discount)
    {
        if (discount < 0.01m || discount > 1m)
            throw new ArgumentOutOfRangeException(nameof(discount), "Множитель скидки должен быть от 0.01 до 1.");
        PersonalDiscount = discount;
    }

    public void SetAllowedBonusRatio(decimal ratio) => balance = balance with { BonusPaymentRatio = ratio };

    public BalanceState GetBalance() => balance;

    // Сервис передает сюда состояние только после успешной оплаты.
    public void ApplyNewBalance(BalanceState newState) => balance = newState;

    public CalculatedPrice CalculateBasketPrice(IEnumerable<Product> products) =>
        Basket.CalculatePrice(products, PersonalDiscount);

    public void AddPurchases(IEnumerable<ProductPosition> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var copies = new List<ProductPosition>();
        foreach (ProductPosition item in items)
        {
            if (item == null)
                throw new ArgumentException("Список покупок не должен содержать null.", nameof(items));
            copies.Add(item.Copy());
        }

        // Проверяем и копируем весь набор до изменения истории.
        purchases.AddRange(copies);
    }

    public void Deconstruct(out int userId, out string userFullName, out string contactsInfo)
    {
        userId = User.Id;
        userFullName = User.FullName;
        var text = new StringBuilder();
        foreach (Contact contact in contacts)
        {
            var (_, channelId, identifier) = contact;
            text.AppendLine($"Контакт канала {channelId}: {identifier}");
        }
        contactsInfo = contacts.Count == 0 ? "Контактов нет." : text.ToString();
    }

    public void Deconstruct(out decimal money, out decimal bonuses, out decimal discountMultiplier, out int purchaseCount)
    {
        money = MoneyBalance;
        bonuses = BonusBalance;
        discountMultiplier = PersonalDiscount;
        purchaseCount = purchases.Count;
    }
}
