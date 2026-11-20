using System;

namespace Marketplace.Domain;

/// <summary>
/// Неизменяемая структура для суммы денег с валютой.
/// Readonly struct с const, static readonly, конструктором с проверками.
/// </summary>
public readonly struct Money
{
    public const decimal MaxAmount = 10_000_000m;
    public static readonly Money Zero = new(0m, "RUB");

    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        EnsureAmount(amount);
        EnsureCurrency(currency);

        Amount = amount;
        Currency = currency;
    }

    public static Money FromRubles(decimal amount) => new(amount, "RUB");

    public Money Add(Money other)
    {
        if (!string.Equals(Currency, other.Currency, StringComparison.Ordinal))
            throw new InvalidOperationException($"Нельзя складывать суммы в разных валютах: {Currency} и {other.Currency}.");

        return new Money(checked(Amount + other.Amount), Currency);
    }

    public override string ToString() => $"{Amount:0.##} {Currency}";

    private static void EnsureAmount(decimal amount)
    {
        if (amount < 0m)
            throw new ArgumentOutOfRangeException(nameof(amount), "Сумма не может быть отрицательной.");

        if (amount > MaxAmount)
            throw new ArgumentOutOfRangeException(nameof(amount), $"Сумма не может превышать {MaxAmount}.");
    }

    private static void EnsureCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Валюта не может быть пустой.", nameof(currency));

        if (currency.Length != 3)
            throw new ArgumentException("Код валюты должен состоять из трёх заглавных символов.", nameof(currency));

        foreach (char c in currency)
        {
            if (c < 'A' || c > 'Z')
                throw new ArgumentException("Код валюты должен содержать только заглавные латинские буквы.", nameof(currency));
        }
    }
}
