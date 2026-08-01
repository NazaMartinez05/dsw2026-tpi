using System.Text.RegularExpressions;

namespace Dsw2026Tpi.CrossCutting.Helpers;

public static class ValidationsExtensions
{
    public const string EmailPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$";
    public static bool IsEmailValid(this string? email)
    {
        return !string.IsNullOrWhiteSpace(email) &&
            Regex.IsMatch(email, EmailPattern);
    }

    public static bool IsDniValid(this string? dni, int minLength = 7, int maxLength = 8)
    {
        return !string.IsNullOrWhiteSpace(dni) &&
            Regex.IsMatch(dni, $@"^\d{{{minLength},{maxLength}}}$");
    }
}
