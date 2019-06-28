using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ManageATenancyAPI.Tests.v2.AcceptanceTests
{
    public class AcceptanceTests
    {
        protected static ServiceProvider BuildServiceProvider()
        {
            IServiceCollection collection = new ServiceCollection();
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var startup = new Startup(config, new HostingEnvironment());
            startup.ConfigureServices(collection);
            var serviceProvider = collection.BuildServiceProvider();
            return serviceProvider;
        }

    }
}