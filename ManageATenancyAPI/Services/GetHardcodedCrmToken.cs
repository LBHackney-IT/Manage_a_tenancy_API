using System;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces;

namespace ManageATenancyAPI.Services
{
    public class GetHardcodedCrmToken : IHackneyGetCRM365Token
    {
        public Task<string> getCRM365AccessToken()
        {
            return Task.FromResult(Environment.GetEnvironmentVariable("CRMToken"));
        }
    }
}