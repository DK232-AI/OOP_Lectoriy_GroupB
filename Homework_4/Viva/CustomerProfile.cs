using System;

namespace Marketplace.Domain;

public class CustomerProfile
{
    private string email = string.Empty;
    private int rating;

    public required string FullName { get; init; }
    public string Phone { get; set; } = string.Empty;

    public string Email
    {
        get => email;
        set
        {
            EnsureValidEmail(value);
            email = value.Trim();
        }
    }

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

    public bool IsTrusted => Rating >= 4;

    public string GetInfo() => GetInfo(true);

    public string GetInfo(bool includeRating) => includeRating
        ? $"{FullName}, рейтинг: {Rating}"
        : FullName;

    public string GetContactInfo(bool includePhone) => includePhone
        ? $"Email: {Email}, телефон: {NormalizePhone(Phone)}"
        : $"Email: {Email}";

    private static void EnsureValidEmail(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
            throw new ArgumentException("Email должен содержать @.", nameof(value));
    }

    private static string NormalizePhone(string value) => value?.Trim() ?? string.Empty;
}
