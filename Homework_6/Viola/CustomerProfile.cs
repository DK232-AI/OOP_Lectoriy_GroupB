using CustomerId = System.Int32;
using static Marketplace.Domain.ContactRules;

namespace Marketplace.Domain;

public class CustomerProfile
{
    public CustomerId Id { get; init; }
    public required string FullName { get; init; }
    public string? MiddleName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    private int rating;
    public int Rating
    {
        get => rating;
        set
        {
            EnsureRating(value);
            rating = value;
        }
    }

    public string GetDisplayName() => $"{FullName} {MiddleName ?? string.Empty}".Trim();

    public string GetEmail() => Email ?? UnknownEmail;

    public void EnsureEmail()
    {
        Email ??= UnknownEmail;
    }

    public string TryNormalizeEmail() => NormalizeEmail(Email);

    private static void EnsureRating(int value)
    {
        if (value < 0 || value > 5)
            throw new ArgumentOutOfRangeException(nameof(value), "Рейтинг должен быть в диапазоне от 0 до 5.");
    }
}
