using System;
using System.Collections.Generic;

namespace Marketplace.Domain;

/// <summary>
/// Профиль пользователя: контакты, балансы, скидка и история покупок.
/// Денежные поля имеют тип double - по требованию ТЗ («с плавающей точкой»).
/// </summary>
public class UserProfile
{
    private readonly List<Contact> _contacts = new();
    private readonly List<ProductPosition> _purchases = new();

    public User User { get; }

    public IReadOnlyList<Contact> Contacts => _contacts;
    public IReadOnlyList<ProductPosition> Purchases => _purchases;

    /// <summary>Баланс в денежных единицах. Не может быть отрицательным.</summary>
    public double MoneyBalance { get; private set; }

    /// <summary>Баланс в бонусах. Не может быть отрицательным.</summary>
    public double BonusBalance { get; private set; }

    /// <summary>Персональная скидка: множитель от 0.01 до 1.00, где 1.00 - скидка 0%.</summary>
    public double PersonalDiscount { get; private set; }

    /// <summary>Разрешённая доля оплаты бонусами: от 0.0 до 0.5.</summary>
    public double AllowedBonusRatio { get; private set; }

    public UserProfile(User user)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));

        MoneyBalance = 0.0;
        BonusBalance = 0.0;
        PersonalDiscount = 1.00;   // 1.00 - это 0% скидки
        AllowedBonusRatio = 0.0;
    }

    public void AddContact(Contact contact)
    {
        if (contact == null)
            throw new ArgumentNullException(nameof(contact));

        if (_contacts.Contains(contact))
            throw new ArgumentException("Этот контакт уже добавлен в профиль.", nameof(contact));

        _contacts.Add(contact);
    }

    public bool RemoveContact(int contactId)
    {
        if (contactId <= 0)
            return false;

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

    /// <summary>Деконструктор для денежных вопросов.</summary>
    public void Deconstruct(out double money, out double bonuses, out double discountMultiplier, out int purchaseCount)
    {
        money = MoneyBalance;
        bonuses = BonusBalance;
        discountMultiplier = PersonalDiscount;
        purchaseCount = Purchases.Count;
    }

    public void AddMoney(double amount)
    {
        // Проверка через отрицание нужна, чтобы отсечь NaN: NaN не меньше нуля и не больше нуля.
        if (!double.IsFinite(amount) || amount <= 0.0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Сумма пополнения должна быть больше нуля.");

        MoneyBalance += amount;
    }

    public void AddBonuses(double amount)
    {
        if (!double.IsFinite(amount) || amount <= 0.0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Сумма бонусов для пополнения должна быть больше нуля.");

        BonusBalance += amount;
    }

    public void SetPersonalDiscount(double discount)
    {
        if (!double.IsFinite(discount) || discount < 0.01 || discount > 1.00)
            throw new ArgumentOutOfRangeException(nameof(discount), "Скидка должна быть в диапазоне от 0.01 до 1.00.");

        PersonalDiscount = discount;
    }

    public void SetAllowedBonusRatio(double ratio)
    {
        if (!double.IsFinite(ratio) || ratio < 0.0 || ratio > 0.5)
            throw new ArgumentOutOfRangeException(nameof(ratio), "Доля оплаты бонусами должна быть от 0.0 до 0.5.");

        AllowedBonusRatio = ratio;
    }

    /// <summary>
    /// Применяет новое состояние баланса после оплаты. Вызывается сервисом оплаты.
    /// Рассчитывает на корректность BalanceState - проверки выполняются в его конструкторе.
    /// </summary>
    public void ApplyNewBalance(BalanceState newState)
    {
        MoneyBalance = newState.Money;
        BonusBalance = newState.Bonuses;
        AllowedBonusRatio = newState.BonusPaymentRatio;
    }

    /// <summary>
    /// Добавляет купленные позиции в историю. Позиции копируются, чтобы история не зависела от корзины и не могла быть изменена извне.
    /// </summary>
    public void AddPurchases(IEnumerable<ProductPosition> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        var buffer = new List<ProductPosition>();

        foreach (var item in items)
        {
            if (item == null)
                throw new ArgumentException("Список покупок не должен содержать null.", nameof(items));

            buffer.Add(item.Copy());
        }

        _purchases.AddRange(buffer);
    }

    public override string ToString() =>
        $"{User.FullName}: {MoneyBalance:F2} УЕ, {BonusBalance:F2} бонусов, скидка {PersonalDiscount:P0}, покупок: {Purchases.Count}";
}