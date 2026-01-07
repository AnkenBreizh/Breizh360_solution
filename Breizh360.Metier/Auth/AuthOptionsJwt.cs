namespace Breizh360.Metier.Auth;

public sealed class AuthOptionsJwt
{
    public string SigningKey { get; init; } = "";
    public string Issuer { get; init; } = "";
    public string Audience { get; init; } = "";

    /// <summary>Durée du JWT (minutes).</summary>
    public int AccessTokenMinutes { get; init; } = 15;

    /// <summary>Durée du refresh token (jours).</summary>
    public int RefreshTokenDays { get; init; } = 14;

    /// <summary>Clock skew (secondes) pour la validation côté API (valeur doc/contrat).</summary>
    public int ClockSkewSeconds { get; init; } = 30;
}
