using System;
using System.Collections.Generic;

namespace Marketplace.Domain;   // рекомендованное файловое пространство имён

// список зависимостей: классы `User`, `Contact`, `CartItem` (класс товарной позиции)

public class UserProfile
{
    public User User { get; }
    private readonly List<Contact> _contacts;
    public IReadOnlyList<Contact> Contacts => _contacts;

    // новые поля (баланс в денежных еденицах, баланс в бонусах, персональная скидка, разрешённая доля оплаты бонусами, список совершённых покупок)
    public double MoneyBalance { get; private set; }
    public double BonusBalance { get; private set; }
    public double PersonalDiscount { get; private set; } // от 0.01 до 1.00
    public double AllowedBonusRatio { get; private set; } // от 0.0 до 0.5

    private readonly List<CartItem> _purchases;
    public IReadOnlyList<CartItem> Purchases => _purchases;

    public UserProfile(User user)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        _contacts = new List<Contact>();

        // инициализация новых полей
        _purchases = new List<CartItem>();
        MoneyBalance = 0.0;
        BonusBalance = 0.0;
        PersonalDiscount = 1.00; // важно: 1.00 - это 0% скидки
        AllowedBonusRatio = 0.0;
    }

    public void AddContact(Contact contact)
    {
        if (contact == null) throw new ArgumentNullException(nameof(contact));
        if (_contacts.Contains(contact)) throw new ArgumentException("Контакт уже добавлен.");
        _contacts.Add(contact);
    }

    public bool RemoveContact(int contactId)
    {
        if (contactId <= 0) return false;
        var contactToRemove = _contacts.Find(c => c.Id == contactId);
        return contactToRemove != null && _contacts.Remove(contactToRemove);
    }

    public void Deconstruct(out int userId, out string userFullName, out string contactsInfo)
    {
        userId = User.Id;
        userFullName = User.FullName;

        var sb = new System.Text.StringBuilder();
        if (Contacts.Count == 0)
        {
            sb.Append("Контактов нет.");
        }
        else
        {
            foreach (var contact in Contacts)
            {
                var (_, channelId, identifier) = contact;
                sb.AppendLine($"Контакт канала {channelId}: {identifier}");
            }
        }
        contactsInfo = sb.ToString();
    }

    // деконструктор для денежных вопросов
    public void Deconstruct(out double money, out double bonuses, out double discountMultiplier, out int purchaseCount)
    {
        money = MoneyBalance;
        bonuses = BonusBalance;
        discountMultiplier = PersonalDiscount;
        purchaseCount = Purchases.Count;
    }



    // новые методы
    public void AddMoney(double amount)
    {
        if (amount <= 0.0) 
            throw new ArgumentException("Сумма пополнения должна быть больше нуля");
        MoneyBalance += amount;
    }

    public void AddBonuses(double amount)
    {
        if (amount <= 0.0) 
            throw new ArgumentException("Сумма бонусов для пополнения должна быть больше нуля");
        BonusBalance += amount;
    }

    public void SetPersonalDiscount(double discount)
    {
        if (discount < 0.01 || discount > 1.00)
            throw new ArgumentException("Скидка должна быть в диапазоне от 0.01 до 1.00");
        PersonalDiscount = discount;
    }

    public void SetAllowedBonusRatio(double ratio)
    {
        if (ratio < 0.0 || ratio > 0.5)
            throw new ArgumentException("Доля оплаты бонусами должна быть от 0.0 до 0.5");
        AllowedBonusRatio = ratio;
    }

    // применение структуры BalanceState после оплаты (вызывается сервисом оплаты)
    public void ApplyNewBalance(BalanceState newState)
    {
        MoneyBalance = newState.Money;
        BonusBalance = newState.Bonuses;
        AllowedBonusRatio = newState.BonusPaymentRatio;
    }

    // добавление купленных товаров в историю
    public void AddPurchases(IEnumerable<CartItem> items)
    {
        if (items == null) 
            throw new ArgumentNullException(nameof(items));
        _purchases.AddRange(items);
    }
}
