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
        #region GetTRAsForPatch
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

            List<TRA> fakeData = generateFakeDataForGettingTRAforPatch();

            IDataReader dataReader = GettingTRAsForPatchConvertToDataReader(fakeData);
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

            IDataReader dataReader = GettingTRAsForPatchConvertToDataReader(fakeData);
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

        public List<TRA> generateFakeDataForGettingTRAforPatch()
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

        private static IDataReader GettingTRAsForPatchConvertToDataReader(List<TRA> fakeData)
        {
            return fakeData.AsDataReader(x => new
            {
              TRAId =x.TRAId,
              Name = x.Name,
              AreaId = x.AreaId
            });
        }

        #endregion

        #region Get TRA information
        [Fact]
        public void should_construct_generic_repository_on_initial_load_get_tra_information()
        {
            var mockDbAccessRepository = new Mock<IDBAccessRepository>();

            mockDbAccessRepository.Setup(x => x.ExecuteReader(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<string>(), It.IsAny<SqlParameter>())).Returns(() => null);

            var repository = new Mock<ITRARepository>();

            repository.Setup(x => x.FindTRAInformation(It.IsAny<int>()));

            repository.Verify();
        }

        [Fact]
        public void repository_should_return_object_with_data_if_there_is_a_match_for_getting_tra_information()
        {
            var mockDbAccessRepository = new Mock<IDBAccessRepository>();       
            List<TestTRADataModel> fakeList = generateFakeDataForGettingTRAInformation();            
            IDataReader dataReader = GettingTRAInformationConvertToDataReader(fakeList);
            var expectedResponse = generateFakeResponseGettingTRAInformation(fakeList);
            mockDbAccessRepository.Setup(x => x.ExecuteReader(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<string>(), It.IsAny<SqlParameter>())).Returns(dataReader);
            var mockAppConfig = new Mock<IOptions<AppConfiguration>>();
            mockAppConfig.SetupGet(x => x.Value).Returns(new AppConfiguration());
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            var mockLogger = new Mock<ILoggerAdapter<TRARepository>>();
            var repository = new TRARepository(mockLogger.Object, mockDbAccessRepository.Object, mockConfig.Object, mockAppConfig.Object);

            var actualResponse = repository.BuildTRAInformation(dataReader);
            Assert.Equal(JsonConvert.SerializeObject(expectedResponse), JsonConvert.SerializeObject(actualResponse));
        }

        [Fact]
        public void repository_should_return_empty_object_if_there_is_no_match_for_getting_tra_information()
        {
            var mockDbAccessRepository = new Mock<IDBAccessRepository>();
            List<TestTRADataModel> fakeList = new List<TestTRADataModel>();
            IDataReader dataReader = GettingTRAInformationConvertToDataReader(fakeList);
            var expectedResponse = generateFakeResponseGettingTRAInformation(fakeList);
            mockDbAccessRepository.Setup(x => x.ExecuteReader(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<string>(), It.IsAny<SqlParameter>())).Returns(dataReader);
            var mockAppConfig = new Mock<IOptions<AppConfiguration>>();
            mockAppConfig.SetupGet(x => x.Value).Returns(new AppConfiguration());
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            var mockLogger = new Mock<ILoggerAdapter<TRARepository>>();
            var repository = new TRARepository(mockLogger.Object, mockDbAccessRepository.Object, mockConfig.Object, mockAppConfig.Object);

            var actualResponse = repository.BuildTRAInformation(dataReader);
            Assert.Equal(JsonConvert.SerializeObject(expectedResponse), JsonConvert.SerializeObject(actualResponse));
        }

        public List<TestTRADataModel> generateFakeDataForGettingTRAInformation()
        {
            var fakeData = new Faker();
            var fakeResult = new List<TestTRADataModel>();

            fakeResult.Add(new TestTRADataModel
            {
                AreaId = fakeData.Random.Int(),
                Email = fakeData.Random.String(),
                EstateName = fakeData.Random.String(),
                EstateUHRef = fakeData.Random.String(),
                Name = fakeData.Random.String(),
                Notes = fakeData.Random.String(),
                PatchCRMId = fakeData.Random.String(),
                Role = fakeData.Random.String(),
                TRAId=fakeData.Random.Int(),
                PersonName = fakeData.Random.String()
            });
            fakeResult.Add(new TestTRADataModel
            {
                AreaId = fakeData.Random.Int(),
                Email = fakeData.Random.String(),
                EstateName = fakeData.Random.String(),
                EstateUHRef = fakeData.Random.String(),
                Name = fakeData.Random.String(),
                Notes = fakeData.Random.String(),
                PatchCRMId = fakeData.Random.String(),
                Role = fakeData.Random.String(),
                TRAId = fakeData.Random.Int(),
                PersonName = fakeData.Random.String()
            });                     
            return fakeResult;
        }

        public TRAInformation generateFakeResponseGettingTRAInformation(List<TestTRADataModel> fakeList)
        {
            var fakeData = new Faker();
            var fakeResult = new TRAInformation();
            var fakeTRA = new TRA();
            var fakeListOfEstates = new List<TRAEstate>();
            var fakeListOfRoles = new List<TRARolesAssignment>();

            foreach (var element in fakeList)
            {
                //set patch id
                fakeResult.PatchId = element.PatchCRMId;
                //set TRA
                fakeResult.TRAId = element.TRAId;
                fakeResult.Email = element.Email;
                fakeResult.Name = element.Name;
                fakeResult.AreaId = element.AreaId;
                //set estates
                fakeListOfEstates.Add(new TRAEstate()
                {
                    EstateName = element.EstateName,
                    EstateUHReference = element.EstateUHRef
                });
                //set roles
                fakeListOfRoles.Add(new TRARolesAssignment()
                {
                    PersonName = element.PersonName,
                    Role = element.Role
               });
            }
            fakeResult.ListOfEstates = fakeListOfEstates;
            fakeResult.ListOfRoles = fakeListOfRoles;         
            return fakeResult;
        }
        private static IDataReader GettingTRAInformationConvertToDataReader(List<TestTRADataModel> fakeData)
        {
            return fakeData.AsDataReader(x => new
            {
                AreaId = x.AreaId,
                Email = x.Email,
                EstateName = x.EstateName,
                EstateUHRef = x.EstateUHRef,
                Name = x.Name,
                Notes = x.Notes,
                PatchCRMId = x.PatchCRMId,
                Role = x.Role,
                TRAId = x.TRAId,
                PersonName = x.PersonName
            }); 
        }
        #endregion
    }

    public class TestTRADataModel
    {
        public int TRAId { get; set; }
        public string Name { get; set; }
        public int AreaId { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public string Role { get; set; }
        public string PersonName { get; set; }
        public string PatchCRMId { get; set; }
        public string EstateUHRef { get; set; }
        public string EstateName { get; set; }
        public string RoleName { get; set; }
    }
}
