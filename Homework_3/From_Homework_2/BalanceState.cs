using System;

namespace Marketplace.Domain;

/// <summary>
/// Состояние баланса покупателя: бонусы, деньги и разрешённая доля оплаты бонусами.
/// Неизменяемая структура (readonly record struct), поэтому поддерживает выражение with и при этом не позволяет изменить состояние на месте.
/// </summary>
public readonly record struct BalanceState
{
    private readonly double bonuses;
    private readonly double money;
    private readonly double bonusPaymentRatio;

    public double Bonuses
    {
        get => bonuses;
        init
        {
            if (!double.IsFinite(value) || value < 0.0)
                throw new ArgumentOutOfRangeException(nameof(Bonuses), "Количество бонусов не может быть отрицательным.");

            bonuses = value;
        }
    }

    public double Money
    {
        get => money;
        init
        {
            if (!double.IsFinite(value) || value < 0.0)
                throw new ArgumentOutOfRangeException(nameof(Money), "Количество денежных единиц не может быть отрицательным.");

            money = value;
        }
    }

    /// <summary>Доля итоговой цены, которую можно оплатить бонусами: от 0.0 до 0.5.</summary>
    public double BonusPaymentRatio
    {
        get => bonusPaymentRatio;
        init
        {
            if (!double.IsFinite(value) || value < 0.0 || value > 0.5)
                throw new ArgumentOutOfRangeException(nameof(BonusPaymentRatio),
                    "Доля оплаты бонусами должна быть от 0.0 до 0.5.");

            bonusPaymentRatio = value;
        }
    }

    public BalanceState(double bonuses, double money, double bonusPaymentRatio)
    {
        Bonuses = bonuses;
        Money = money;
        BonusPaymentRatio = bonusPaymentRatio;
    }

    public override string ToString() =>
        $"Баланс: {Money:F2} УЕ, {Bonuses:F2} бонусов. Возможная доля оплаты бонусами: {BonusPaymentRatio:P0}";
}
