namespace Marketplace.Domain;

public static class ContactRules
{
    public const string UnknownEmail = "почта не указана";

    public static string NormalizeEmail(string? value) =>
        value?.Trim().ToLowerInvariant() ?? UnknownEmail;

    public static bool LooksLikeEmail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        int atIndex = value.IndexOf('@');
        return atIndex > 0 && atIndex == value.LastIndexOf('@') && atIndex < value.Length - 1;
    }
}
