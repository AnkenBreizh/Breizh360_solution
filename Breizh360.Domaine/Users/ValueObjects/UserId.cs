namespace Breizh360.Domaine.Users.ValueObjects;

public readonly record struct UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());

    public static UserId From(Guid value)
        => value == Guid.Empty ? throw new ArgumentException("UserId ne peut pas être vide.", nameof(value)) : new(value);

    public override string ToString() => Value.ToString();
}
