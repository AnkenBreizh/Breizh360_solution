using System;

namespace Breizh360.Api.Services
{
    /// <summary>
    /// Options de configuration pour l’émission et la validation des JWT.
    /// Ces paramètres sont généralement fournis via la section "Jwt" du fichier appsettings.json.
    /// </summary>
    public sealed class JwtOptions
    {
        /// <summary>
        /// Clé secrète utilisée pour signer les JWT. Doit contenir au moins 32 caractères.
        /// </summary>
        public string SigningKey { get; init; } = string.Empty;

        /// <summary>
        /// Émetteur des tokens (claim iss). Typiquement le nom ou l’URL de l’API.
        /// </summary>
        public string Issuer { get; init; } = string.Empty;

        /// <summary>
        /// Audience des tokens (claim aud). Utilisée pour limiter la portée du token.
        /// </summary>
        public string Audience { get; init; } = string.Empty;

        /// <summary>
        /// Durée de vie (en minutes) des access tokens. Par défaut : 60 minutes.
        /// </summary>
        public int AccessTokenExpiryMinutes { get; init; } = 60;

        /// <summary>
        /// Durée de vie (en jours) des refresh tokens. Par défaut : 7 jours.
        /// </summary>
        public int RefreshTokenExpiryDays { get; init; } = 7;
    }
}