using System;

namespace Marketplace.Domain;

public readonly struct BalanceState
{
    private readonly decimal bonuses;
    private readonly decimal money;
    private readonly decimal bonusPaymentRatio;

    public decimal Bonuses
    {
        get => bonuses;
        init
        {
            if (value < 0m)
                throw new ArgumentOutOfRangeException(nameof(value), "Бонусы не могут быть отрицательными.");
            bonuses = value;
        }
    }

    public decimal Money
    {
        get => money;
        init
        {
            if (value < 0m)
                throw new ArgumentOutOfRangeException(nameof(value), "Баланс не может быть отрицательным.");
            money = value;
        }
    }

    public decimal BonusPaymentRatio
    {
        get => bonusPaymentRatio;
        init
        {
            if (value < 0m || value > 0.5m)
                throw new ArgumentOutOfRangeException(nameof(value), "Доля оплаты бонусами должна быть от 0 до 0.5.");
            bonusPaymentRatio = value;
        }
    }

    // init-проверки действуют и в конструкторе, и при копировании через with.
    public BalanceState(decimal bonuses, decimal money, decimal bonusPaymentRatio)
    {
        Bonuses = bonuses;
        Money = money;
        BonusPaymentRatio = bonusPaymentRatio;
    }

    public override string ToString() =>
        $"Баланс: {Money:F2} УЕ, {Bonuses:F2} бонусов. Доля оплаты бонусами: {BonusPaymentRatio:P0}";
}
