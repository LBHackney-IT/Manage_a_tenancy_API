using System.Security.Claims;
using ManageATenancyAPI.Services.JWT.Models;

namespace ManageATenancyAPI.Services.JWT
{
    /// <summary>
    /// Service to Decrypt JWT tokens sent from Outsystems
    /// </summary>
    public interface IJWTService
    {
        /// <summary>
        /// validates claims against signing key with secret
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        ClaimsPrincipal GetClaims(string token, string secret);

        /// <summary>
        /// Validates claims against signing key with secret and returns data in a nicely formatted manner.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        IManageATenancyClaims GetManageATenancyClaims(string token, string secret);
    }
}