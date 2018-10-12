using ManageATenancyAPI.Tests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ManageATenancyAPI
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration,IHostingEnvironment env) : base(configuration,env)
        {
            TestStatus.IsRunningInTests = true;
        }
    }
}
