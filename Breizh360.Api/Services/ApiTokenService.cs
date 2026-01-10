using System;
using System.Threading.Tasks;
using Breizh360.Api.Metier.Contracts.Auth;
using Breizh360.Domaine.Auth.Users;

namespace Breizh360.Api.Services
{
    /// <summary>
    /// Service chargé d’émettre des paires de tokens (access + refresh). Cette implémentation
    /// minimaliste génère des valeurs aléatoires. Une implémentation de production
    /// devrait signer les JWTs, gérer la rotation des clés et stocker de manière
    /// sécurisée les refresh tokens.
    /// </summary>
    public sealed class ApiTokenService
    {
        /// <summary>
        /// Émet un nouveau couple access/refresh pour l’utilisateur spécifié.
        /// </summary>
        public Task<AuthContractsTokenPairResponse> IssueAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            string accessToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            string refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var response = new AuthContractsTokenPairResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresInSeconds = 3600
            };
            return Task.FromResult(response);
        }

        /// <summary>
        /// Rotate un refresh token et renvoie un nouveau couple. Aucun contrôle
        /// n’est encore effectué sur le token fourni.
        /// </summary>
        public Task<AuthContractsTokenPairResponse> RefreshAsync(string refreshToken)
        {
            string newAccessToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            string newRefreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var response = new AuthContractsTokenPairResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresInSeconds = 3600
            };
            return Task.FromResult(response);
        }
    }
}