using System;

namespace Marketplace.Domain;

/// <summary>
/// Профиль покупателя. Демонстрирует те же четыре вида свойств, что и Product,
/// плюс автоматическое свойство с инициализатором и перегрузки GetInfo.
/// </summary>
public class CustomerProfile
{
    private string email = string.Empty;
    private int rating;

    // Автоматическое свойство: required + init.
    public required string FullName { get; init; }

    // Автоматическое свойство с инициализатором.
    public string Phone { get; set; } = string.Empty;

    // Полная запись: сеттер проверяет наличие '@'.
    public string Email
    {
        get => email;
        set
        {
            EnsureValidEmail(value);
            email = value.Trim();
        }
    }

    // Полная запись с проверкой диапазона, резервное поле rating.
    public int Rating
    {
        get => rating;
        set
        {
            if (value < 0 || value > 5)
                throw new ArgumentOutOfRangeException(nameof(value), "Рейтинг должен быть от 0 до 5.");

            rating = value;
        }
    }

    // Вычисляемое свойство: сеттера нет.
    public bool IsTrusted => Rating >= 4;

    /// <summary>Короткая версия всегда вызывает полную.</summary>
    public string GetInfo() => GetInfo(true);

    public string GetInfo(bool includeRating) => includeRating
        ? $"{FullName}, рейтинг: {Rating}"
        : FullName;

    public string GetContactInfo(bool includePhone) => includePhone
        ? $"Email: {Email}, телефон: {NormalizePhone(Phone)}"
        : $"Email: {Email}";

    private static void EnsureValidEmail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
            throw new ArgumentException("Email должен содержать @.", nameof(value));
    }

    private static string NormalizePhone(string? value) =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
}
