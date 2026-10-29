using System;

namespace Marketplace.Domain;  // рекомендованное пространство имён

// стуктура состояния баланса покупателя
public readonly struct BalanceState
{
    public double Bonuses { get; }
    public double Money { get; }
    public double BonusPaymentRatio { get; }    // доля итоговой цены, которую можно оплатить бонусами, от 0.0 до 0.5

    public BalanceState(double bonuses, double money, double bonusPaymentRatio)
    {
        if (bonuses < 0.0)
            throw new ArgumentException("Количество бонусов не может быть меньше нуля", nameof(bonuses));

        if (money < 0.0)
            throw new ArgumentException("Количество денежных единиц не может быть меньше нуля", nameof(money));

        if (bonusPaymentRatio < 0.0 || bonusPaymentRatio > 0.5)
            throw new ArgumentException("Доля оплаты бонусами должна быть от 0.0 до 0.5", nameof(bonusPaymentRatio));

        Bonuses = bonuses;
        Money = money;
        BonusPaymentRatio = bonusPaymentRatio;
    }

    public override string ToString()
    {
        return $"Баланс: {Money:F2} УЕ, {Bonuses:F2} бонусов. Возможная доля оплаты бонусами: {BonusPaymentRatio:P0}";
    }
}
