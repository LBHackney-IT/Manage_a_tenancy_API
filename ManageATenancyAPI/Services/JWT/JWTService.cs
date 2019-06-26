using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using ManageATenancyAPI.Services.JWT.Models;
using Microsoft.IdentityModel.Tokens;

namespace ManageATenancyAPI.Services.JWT
{
    public class JWTService: IJWTService
    {
        /// <summary>
        /// Validates claims against signing key with secret and returns data in a nicely formatted manner.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public IManageATenancyClaims GetManageATenancyClaims(string token, string secret)
        {
            var key = Encoding.ASCII.GetBytes(secret);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            var claims = handler.ValidateToken(token, validations, out var tokenSecure);

            IManageATenancyClaims manageATenancyClaims = ManageATenancyClaims.FromJson(claims.Claims.ToList()[2].Value);

            return manageATenancyClaims;
        }
    }
}
