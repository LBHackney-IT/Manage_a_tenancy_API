using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Models;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ManageATenancyAPI.Configuration;
using Xunit;
using ManageATenancyAPI.Repository;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Tests.Repository
{
    public class CitizenIndexRepositoryTest
    {
        [Fact]
        public void should_construct_generic_repository_on_initial_load()
        {

            var mockDbAccessRepository = new Mock<IDBAccessRepository>();

            mockDbAccessRepository.Setup(x => x.ExecuteReader(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<string>(), It.IsAny<SqlParameter>())).Returns(() => null);

            var repository = new Mock<ICitizenIndexRepository>();

            repository.Setup(x => x.SearchCitizenIndex(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            repository.Verify();
        }

        [Fact]
        public void should_buildcontactperson_method_transform_datareader_to_a_collection_of_ciperson_object()
        {
            var mockDbAccessRepository = new Mock<IDBAccessRepository>();

            List<TestDataModel> testData = GetTestData();

            var expected = new List<CIPerson>
            {
                new CIPerson
                {
                    HackneyhomesId = testData.Select(x => x.HACKNEYHOMES).SingleOrDefault(),
                    Title = testData.Select(x => x.NAME_TITLE).SingleOrDefault(),
                    FirstName = testData.Select(x => x.NAME_CUSTOM_1).SingleOrDefault(),
                    Surname = testData.Select(x => x.NAME_SURNAME).SingleOrDefault(),
                    DateOfBirth = testData.Select(x => x.DOB_VALUE).SingleOrDefault(),
                    FullAddressDisplay = testData.Select(x => x.FullAddressDisplay).SingleOrDefault(),
                    FullAddressSearch = testData.Select(x => x.FullAddressSearch).SingleOrDefault(),
                    Address = testData.Select(x => x.Address).SingleOrDefault(),
                    AddressLine1 = testData.Select(x => x.AddressLine1).SingleOrDefault(),
                    AddressLine2 = testData.Select(x => x.ADDR_STREET).SingleOrDefault(),
                    AddressLine3 = testData.Select(x => x.ADDR_LOCALITY).SingleOrDefault(),
                    AddressCity = testData.Select(x => x.ADDR_TOWN).SingleOrDefault(),
                    AddressCountry = testData.Select(x => x.ADDR_COUNTY).SingleOrDefault(),
                    PostCode = testData.Select(x => x.ADDR_POSTCODE).SingleOrDefault(),
                    SystemName = testData.Select(x => x.SystemName).SingleOrDefault() ,
                    USN = testData.Select(x => x.USN).SingleOrDefault(),
                    LARN = testData.Select(x => x.LARN).SingleOrDefault(),
                    UPRN = testData.Select(x => x.UPRN).SingleOrDefault(),
                    CrmContactId = testData.Select(x => x.CRM).SingleOrDefault(),
                    PropertyCautionaryAlert = testData.Select(x => x.PropertyCautionaryAlert).SingleOrDefault(),
                    CautionaryAlert = testData.Select(x => x.CautionaryAlert).SingleOrDefault(),
                }
            };

            IDataReader dataReader = ConvertToDataReader(testData);

            mockDbAccessRepository.Setup(x => x.ExecuteReader(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<string>(), It.IsAny<SqlParameter>())).Returns(dataReader);
            var mockAppConfig = new Mock<IOptions<AppConfiguration>>();
            mockAppConfig.SetupGet(x => x.Value).Returns(new AppConfiguration());
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            var repository = new CitizenIndexRepository(mockDbAccessRepository.Object, mockConfig.Object, mockAppConfig.Object);

            var actual = repository.BuildContactPerson(dataReader);

            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));

        }

        private static IDataReader ConvertToDataReader(List<TestDataModel> testData)
        {
            return testData.AsDataReader(x => new
            {
                ADDR_SUBBNAME = x.ADDR_SUBBNAME,
                ADDR_BNUM = x.ADDR_BNUM,
                ADDR_BUILDING = x.ADDR_BUILDING,
                ADDR_COUNTY = x.ADDR_COUNTY,
                ADDR_LOCALITY = x.ADDR_LOCALITY,
                ADDR_POSTCODE = x.ADDR_POSTCODE,
                ADDR_STREET = x.ADDR_STREET,
                ADDR_TOWN = x.ADDR_TOWN,
                DOB_VALUE = x.DOB_VALUE,
                HACKNEYHOMES = x.HACKNEYHOMES,
                NAME_TITLE = x.NAME_TITLE,
                NAME_CUSTOM_1 = x.NAME_CUSTOM_1,
                NAME_SURNAME = x.NAME_SURNAME,
                CRM = x.CRM,
                Id = x.ID,
                HackneyhomesId = x.HackneyhomesId,
                Title = x.Title,
                Surname = x.Surname,
                FirstName = x.FirstName,
                DateOfBirth = x.DateOfBirth,
                FullAddressDisplay = x.FullAddressDisplay,
                Address = x.Address,
                AddressLine1 = x.AddressLine1,
                AddressLine2 = x.AddressLine2,
                AddressLine3 = x.AddressLine3,
                AddressCity = x.AddressCity,
                AddressCountry = x.AddressCountry,
                PostCode = x.PostCode,
                SystemName = x.SystemName,
                LARN = x.LARN,
                UPRN = x.UPRN,
                USN = x.USN,
                FullAddressSearch = x.FullAddressSearch,
                CrMcontactId = x.CrMcontactId,
                FullName = x.FullName,
                CautionaryAlert = x.CautionaryAlert,
                PropertyCautionaryAlert = x.PropertyCautionaryAlert
            });
        }

        private static List<TestDataModel> GetTestData()
        {
            return new List<TestDataModel>
            {
                new TestDataModel{
                ADDR_SUBBNAME = "sub b name",
                ADDR_BNUM = "bnum",
                ADDR_BUILDING = "building",
                ADDR_COUNTY = "county",
                ADDR_LOCALITY = "locality",
                ADDR_POSTCODE = "postcode",
                ADDR_STREET = "street",
                ADDR_TOWN = "town",
                DOB_VALUE = "2018-02-23",
                HACKNEYHOMES= "",
                NAME_TITLE = "MISS",
                NAME_CUSTOM_1 = "ANJA",
                NAME_SURNAME = "NAME_SURNAME",
                CRM = new Guid("e683f257-5636-457d-a56f-6055170f0a51"),
                FirstName = "ANJA",
                Surname = "WALSER",
                DateOfBirth = "",
                Address = "sub b name bnum building street locality town county postcode",
                FullAddressDisplay = "sub b name bnum building street locality town county postcode",
                AddressLine1 = "sub b name bnum building",
                AddressLine2 = "ADELAIDE AVENUE",
                AddressLine3 = "LADYWELL",
                AddressCity = "LONDON",
                AddressCountry = "LEWISHAM",
                PostCode = "SE4 1LF",
                SystemName = "CitizenIndex",
                LARN = "LARN67134427",
                UPRN = "100021923691",
                USN = "855022",
                FullAddressSearch = "subbnamebnumbuildingstreetlocalitytowncountypostcode",
                PropertyCautionaryAlert = null, //Not from CI comes from CRM
                CautionaryAlert = null,//Not from CI comes from CRM
                CrMcontactId = Guid.NewGuid()
                }
            };
        }
    }

    public class TestDataModel
    {
        public string ID { get; set; }
        public string HackneyhomesId { get; set; }
        public string Title { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string DateOfBirth { get; set; }
        public string Address { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressCity { get; set; }
        public string AddressCountry { get; set; }
        public string PostCode { get; set; }
        public string SystemName { get; set; }
        public string LARN { get; set; }
        public string UPRN { get; set; }
        public string USN { get; set; }
        public string FullAddressSearch { get; set; }
        public string FullAddressDisplay { get; set; }
        public Guid CrMcontactId { get; set; }
        public string FullName
        {
            get
            {
                return (FirstName.Trim() + " " + Surname.Trim()).Trim();
            }
        }
        public string ADDR_SUBBNAME { get; set; }
        public string ADDR_BNUM { get; set; }
        public string ADDR_BUILDING { get; set; }
        public string ADDR_STREET { get; set; }
        public string ADDR_LOCALITY { get; set; }
        public string ADDR_TOWN { get; set; }
        public string ADDR_COUNTY { get; set; }
        public string ADDR_POSTCODE { get; set; }
        public string DOB_VALUE { get; set; }
        public string HACKNEYHOMES { get; set; }
        public string NAME_TITLE { get; set; }
        public string NAME_CUSTOM_1 { get; set; }
        public string NAME_SURNAME { get; set; }
        public Guid CRM { get; set; }
        public bool? PropertyCautionaryAlert { get; set; }
        public bool? CautionaryAlert { get; set; }
    }
}
