using System;

namespace Marketplace.Services;

using Marketplace.Domain;

/// <summary>
/// Результат попытки оплаты. Неизменяемая структура.
/// </summary>
public readonly record struct PaymentResult
{
    /// <summary>Признак успешной оплаты.</summary>
    public bool Success { get; init; }

    /// <summary>Сообщение для вывода покупателю.</summary>
    public string Message { get; init; }

    /// <summary>Сколько бонусов списано.</summary>
    public decimal BonusesSpent { get; init; }

    /// <summary>Сколько денежных единиц списано.</summary>
    public decimal MoneySpent { get; init; }

    /// <summary>Состояние баланса после операции. При отказе совпадает с исходным.</summary>
    public BalanceState NewBalance { get; init; }

    /// <summary>Расчётная цена, по которой совершалась попытка оплаты.</summary>
    public CalculatedPrice Price { get; init; }
}
