namespace Breizh360.Domaine.Users.ValueObjects;

public readonly record struct DisplayName(string Value)
{
    public static DisplayName From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("DisplayName est requis.", nameof(value));

        value = value.Trim();

        if (value.Length < 2)
            throw new ArgumentException("DisplayName doit contenir au moins 2 caractères.", nameof(value));

        if (value.Length > 80)
            throw new ArgumentException("DisplayName ne doit pas dépasser 80 caractères.", nameof(value));

        return new DisplayName(value);
    }

    public override string ToString() => Value;
}
