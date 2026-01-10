using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Breizh360.Api.Metier.Contracts.Auth;
using Breizh360.Domaine.Auth.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Breizh360.Api.Services
{
    /// <summary>
    /// Service chargé d’émettre des paires de tokens (access + refresh).
    /// Cette implémentation signe les access tokens en utilisant le schéma JWT standard
    /// et génère des refresh tokens aléatoires. Elle n’implémente pas encore la
    /// persistance des refresh tokens mais prépare le terrain pour une future
    /// implémentation en calculant un hash (HMACSHA256) à partir de la clé de
    /// signature. La durée de vie des tokens est configurable via <see cref="JwtOptions"/>.
    /// </summary>
    public sealed class ApiTokenService
    {
        private readonly JwtOptions _options;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        /// <summary>
        /// Construit le service d’émission des tokens.
        /// </summary>
        /// <param name="optionsAccessor">Accès à la configuration des JWT.</param>
        public ApiTokenService(IOptions<JwtOptions> optionsAccessor)
        {
            _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        /// <summary>
        /// Émet un nouveau couple access/refresh pour l’utilisateur spécifié.
        /// L’access token est un JWT signé et contient au minimum les claims
        /// subject (sub) et unique name (unique_name). Un refresh token aléatoire
        /// est également généré. La persistance du refresh token n’est pas encore
        /// effectuée.
        /// </summary>
        /// <param name="user">Utilisateur pour lequel émettre les tokens.</param>
        /// <returns>Un objet <see cref="AuthContractsTokenPairResponse"/> contenant le couple de tokens.</returns>
        public Task<AuthContractsTokenPairResponse> IssueAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            // Construction des claims standard
            var now = DateTimeOffset.UtcNow;
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.Login)
            };

            // Clé de signature symétrique
            var keyBytes = Encoding.UTF8.GetBytes(_options.SigningKey);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Génération du JWT
            var expires = now.AddMinutes(_options.AccessTokenExpiryMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires.UtcDateTime,
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                SigningCredentials = credentials
            };
            var securityToken = _tokenHandler.CreateToken(tokenDescriptor);
            string accessToken = _tokenHandler.WriteToken(securityToken);

            // Génération d’un refresh token aléatoire (32 octets -> base64url).
            var refreshBytes = new byte[32];
            RandomNumberGenerator.Fill(refreshBytes);
            string refreshToken = Base64UrlEncoder.Encode(refreshBytes);

            // Calcul d’un hash HMAC du refresh token pour une future persistance.
            using var hmac = new HMACSHA256(keyBytes);
            var macBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
            string tokenHash = $"HMACSHA256$kid=default${Base64UrlEncoder.Encode(macBytes)}";
            // À terme, persister {tokenHash, user.Id, expires + RefreshTokenExpiryDays} via IRefreshTokenRepository.

            var response = new AuthContractsTokenPairResponse
            {
                TokenType = "Bearer",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresInSeconds = _options.AccessTokenExpiryMinutes * 60
            };
            return Task.FromResult(response);
        }

        /// <summary>
        /// Rotate un refresh token et renvoie un nouveau couple. Cette implémentation
        /// ne valide pas le refresh token fourni mais délivre un nouveau couple
        /// de tokens pour des tests de bout en bout. La persistance et la
        /// validation des refresh tokens seront ajoutées ultérieurement.
        /// </summary>
        /// <param name="refreshToken">Refresh token transmis par le client.</param>
        /// <returns>Un nouveau couple de tokens.</returns>
        public Task<AuthContractsTokenPairResponse> RefreshAsync(string refreshToken)
        {
            // Pour l’instant, on ignore le token fourni et on émet un nouveau couple.
            // Cela permet de tester l’endpoint /auth/refresh en attendant une implémentation complète.
            // Une future version devra :
            // 1. Calculer le hash HMAC du refreshToken reçu.
            // 2. Rechercher un enregistrement actif dans le store (IRefreshTokenRepository).
            // 3. Révoquer l’ancien refresh token et créer un nouveau refresh token.
            // 4. Émettre un nouveau JWT avec les claims du user associé.
            var dummyUser = new User("dummy", "dummy@example.com", new string('x', 32));
            return IssueAsync(dummyUser);
        }
    }
}