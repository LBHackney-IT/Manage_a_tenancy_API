﻿using System;
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
    [Collection("Database collection")]
    public class TraRepositoryTests : BaseTest
    {
        [Fact]
        public async Task Exists_ReturnsTrue()
        {

            var options = new OptionsWrapper<ConnStringConfiguration>(GetConfiguration<ConnStringConfiguration>(Config, "ConnectionStrings"));
            ILoggerAdapter<TRARepository> logger = new Mock<ILoggerAdapter<TRARepository>>().Object;
            IDBAccessRepository genericRepository = new Mock<IDBAccessRepository>().Object;
            IOptions<AppConfiguration> config = new Mock<IOptions<AppConfiguration>>().Object;
            var traRepository = new TRARepository(logger, genericRepository, options, config);

            var result = await traRepository.Exists("TestTRA3");
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

            var result = await traRepository.Create($"Nad{id} Estate TRA", "Notes for x{id} Estate TRA", $"nad{id}.com", 1, Guid.Parse("f18b2363-8453-e811-8126-70106faaf8c1"));
            var exists = await traRepository.Exists($"Nad{id} Estate TRA");
            Assert.True(exists);

        }

        [Fact]
        public async Task Find()
        {
            var options = new OptionsWrapper<ConnStringConfiguration>(GetConfiguration<ConnStringConfiguration>(Config, "ConnectionStrings"));
            ILoggerAdapter<TRARepository> logger = new Mock<ILoggerAdapter<TRARepository>>().Object;
            IDBAccessRepository genericRepository = new Mock<IDBAccessRepository>().Object;
            IOptions<AppConfiguration> config = new Mock<IOptions<AppConfiguration>>().Object;
            var traRepository = new TRARepository(logger, genericRepository, options, config);
            var id = Guid.NewGuid().ToString().Substring(0, 8);

            var result = await traRepository.Create($"Nad{id} Estate TRA", "Notes for x{id} Estate TRA", $"nad{id}.com", 1, Guid.Parse("f18b2363-8453-e811-8126-70106faaf8c1"));
            var found = await traRepository.Find($"Nad{id} Estate TRA");

            Assert.NotNull(result);
            Assert.NotNull(found);
            Assert.Equal(result.Name, found.Name);

        }

        public async Task Create_UpdateNotes()
        {
            var options = new OptionsWrapper<ConnStringConfiguration>(GetConfiguration<ConnStringConfiguration>(Config, "ConnectionStrings"));
            ILoggerAdapter<TRARepository> logger = new Mock<ILoggerAdapter<TRARepository>>().Object;
            IDBAccessRepository genericRepository = new Mock<IDBAccessRepository>().Object;
            IOptions<AppConfiguration> config = new Mock<IOptions<AppConfiguration>>().Object;
            var traRepository = new TRARepository(logger, genericRepository, options, config);

            var id = Guid.NewGuid().ToString().Substring(0, 8);

            var result = await traRepository.Create($"Nad{id} Estate TRA", "Notes for x{id} Estate TRA", $"nad{id}.com", 1, Guid.Parse("f18b2363-8453-e811-8126-70106faaf8c1"));
            var exists = await traRepository.Exists($"Nad{id} Estate TRA");
            Assert.True(exists);

            traRepository.UpdateNotes(result.TRAId, "this is a new note");

        }
        public async Task Create_UpdateEmail()
        {
            var options = new OptionsWrapper<ConnStringConfiguration>(GetConfiguration<ConnStringConfiguration>(Config, "ConnectionStrings"));
            ILoggerAdapter<TRARepository> logger = new Mock<ILoggerAdapter<TRARepository>>().Object;
            IDBAccessRepository genericRepository = new Mock<IDBAccessRepository>().Object;
            IOptions<AppConfiguration> config = new Mock<IOptions<AppConfiguration>>().Object;
            var traRepository = new TRARepository(logger, genericRepository, options, config);

            var id = Guid.NewGuid().ToString().Substring(0, 8);

            var result = await traRepository.Create($"Nad{id} Estate TRA", "Notes for x{id} Estate TRA", $"nad{id}.com", 1, Guid.Parse("f18b2363-8453-e811-8126-70106faaf8c1"));
            var exists = await traRepository.Exists($"Nad{id} Estate TRA");
            Assert.True(exists);

            traRepository.UpdateEmail(result.TRAId, "updated@update.com");

        }

    }
}