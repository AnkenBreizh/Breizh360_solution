namespace Breizh360.Domaine.Notifications.ValueObjects;

/// <summary>
/// Clé d'idempotence fonctionnelle (anti-doublons).
/// La contrainte d'unicité est imposée côté Data (index unique).
/// </summary>
public readonly record struct IdempotencyKey(string Value)
{
    public const int MaxLength = 128;

    public static IdempotencyKey From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("IdempotencyKey est requis.", nameof(value));

        value = value.Trim();

        if (value.Length > MaxLength)
            throw new ArgumentException($"IdempotencyKey est trop long (max {MaxLength}).", nameof(value));

        return new IdempotencyKey(value);
    }

    public override string ToString() => Value;
}
