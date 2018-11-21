using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Repository;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace ManageATenancyAPI.Tests.Repository
{
    public class TRARepositoryTest
    {
        [Fact]
        public void should_construct_generic_repository_on_initial_load()
        {
            var mockDbAccessRepository = new Mock<IDBAccessRepository>();

            mockDbAccessRepository.Setup(x => x.ExecuteReader(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<string>(), It.IsAny<SqlParameter>())).Returns(() => null);

            var repository = new Mock<ITRARepository>();

            repository.Setup(x => x.FindTRAsForPatch(It.IsAny<string>()));

            repository.Verify();
        }
        
        [Fact]
        public void repository_should_return_object_with_data_if_there_is_a_match()
        {
            var mockDbAccessRepository = new Mock<IDBAccessRepository>();

            List<TRA> fakeData = generateFakeData();

            IDataReader dataReader = ConvertToDataReader(fakeData);
            mockDbAccessRepository.Setup(x => x.ExecuteReader(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<string>(), It.IsAny<SqlParameter>())).Returns(dataReader);

            var mockAppConfig = new Mock<IOptions<AppConfiguration>>();
            mockAppConfig.SetupGet(x => x.Value).Returns(new AppConfiguration());
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            var mockLogger = new Mock<ILoggerAdapter<TRARepository>>();
            var repository = new TRARepository(mockLogger.Object,mockDbAccessRepository.Object, mockConfig.Object, mockAppConfig.Object);

            var actualResponse = repository.BuildListOfTRAs(dataReader);
            Assert.Equal(JsonConvert.SerializeObject(fakeData), JsonConvert.SerializeObject(actualResponse));
        }

        [Fact]
        public void repository_should_return_empty_object_with_data_if_there_is_no_match()
        {
            var mockDbAccessRepository = new Mock<IDBAccessRepository>();

            List<TRA> fakeData = new List<TRA>();

            IDataReader dataReader = ConvertToDataReader(fakeData);
            mockDbAccessRepository.Setup(x => x.ExecuteReader(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<string>(), It.IsAny<SqlParameter>())).Returns(dataReader);

            var mockAppConfig = new Mock<IOptions<AppConfiguration>>();
            mockAppConfig.SetupGet(x => x.Value).Returns(new AppConfiguration());
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            var mockLogger = new Mock<ILoggerAdapter<TRARepository>>();
            var repository = new TRARepository(mockLogger.Object, mockDbAccessRepository.Object, mockConfig.Object, mockAppConfig.Object);

            var actualResponse = repository.BuildListOfTRAs(dataReader);
            Assert.Equal(JsonConvert.SerializeObject(fakeData), JsonConvert.SerializeObject(actualResponse));
        }

        public List<TRA> generateFakeData()
        {
            var fakeData = new Faker();

            List<TRA> fakeListOfTRA = new List<TRA>();
            fakeListOfTRA.Add(new TRA()
            {
                AreaId = fakeData.Random.Int(),
                Name = fakeData.Random.String(),
                TRAId = fakeData.Random.Int()
            });
            fakeListOfTRA.Add(new TRA()
            {
                AreaId = fakeData.Random.Int(),
                Name= fakeData.Random.String(),
                TRAId = fakeData.Random.Int()
            });

            return fakeListOfTRA;
        }

        private static IDataReader ConvertToDataReader(List<TRA> fakeData)
        {
            return fakeData.AsDataReader(x => new
            {
              TRAId =x.TRAId,
              Name = x.Name,
              AreaId = x.AreaId
            });
        }
    }
}
