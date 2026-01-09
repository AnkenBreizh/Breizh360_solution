using System.Text.RegularExpressions;

namespace Breizh360.Domaine.Users.ValueObjects;

public readonly record struct Email(string Value)
{
    private static readonly Regex SimpleEmailRegex =
        new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static Email From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email est requis.", nameof(value));

        value = value.Trim();

        if (value.Length > 320) // limite généralement admise (RFC + pratiques)
            throw new ArgumentException("Email est trop long.", nameof(value));

        if (!SimpleEmailRegex.IsMatch(value))
            throw new ArgumentException("Email invalide.", nameof(value));

        return new Email(value);
    }

    public override string ToString() => Value;
}
