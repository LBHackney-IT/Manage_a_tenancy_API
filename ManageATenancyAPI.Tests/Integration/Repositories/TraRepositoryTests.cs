using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Repository;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.Integration.Repositories
{
    public class TraRepositoryTests:BaseTest
    {
        [Fact]
        public async Task Exists_ReturnsTrue()
        {

            var options = new OptionsWrapper<ConnStringConfiguration>(GetConfiguration<ConnStringConfiguration>(Config, "ConnectionStrings"));
            ILoggerAdapter<TRARepository> logger = new Mock<ILoggerAdapter<TRARepository>>().Object;
            IDBAccessRepository genericRepository = new Mock<IDBAccessRepository>().Object;
            IOptions<AppConfiguration> config = new Mock<IOptions<AppConfiguration>> ().Object;
            var traRepository = new TRARepository(logger, genericRepository,options, config);

            var result = traRepository.Exists("Blackstone Estate TRA");
            Assert.True(result);
        }

        [Fact]
        public async Task Create()
        {
            var options = new OptionsWrapper<ConnStringConfiguration>(GetConfiguration<ConnStringConfiguration>(Config, "ConnectionStrings"));
            ILoggerAdapter<TRARepository> logger = new Mock<ILoggerAdapter<TRARepository>>().Object;
            IDBAccessRepository genericRepository = new Mock<IDBAccessRepository>().Object;
            IOptions<AppConfiguration> config = new Mock<IOptions<AppConfiguration>>().Object;
            var traRepository = new TRARepository(logger, genericRepository, options, config);

            var id = Guid.NewGuid().ToString().Substring(0, 8);

            var result = traRepository.Create($"Nad{id} Estate TRA", "Notes for x{id} Estate TRA",$"nad{id}.com",1,Guid.Parse("f18b2363-8453-e811-8126-70106faaf8c1"));
            var exists = traRepository.Exists($"Nad{id} Estate TRA");
            Assert.True(exists);

        }
    }
}