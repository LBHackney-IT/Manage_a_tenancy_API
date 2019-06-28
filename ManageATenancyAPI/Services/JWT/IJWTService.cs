using ManageATenancyAPI.Services.JWT.Models;
using System;

namespace ManageATenancyAPI.Services.JWT
{
    /// <summary>
    /// Service to Decrypt JWT tokens sent from Outsystems
    /// </summary>
    public interface IJWTService
    {

        /// <summary>
        /// Validates claims against signing key with secret and returns data in a nicely formatted manner.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        IManageATenancyClaims GetManageATenancyClaims(string token, string secret);

        string CreateManageATenancySingleMeetingToken(Guid traMeetingId, string secret);
    }
}