using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Services;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Services.Interfaces;
using Xunit;

namespace ManageATenancyAPI.Tests.Unit.Services
{
    public class TenancyServiceTests
    {
        private readonly Mock<IClock> _mockClock;
        private readonly Mock<IHackneyHousingAPICall> _mockHousingApiCall;
        private readonly Mock<INewTenancyService> _mockLastRetrieved;
        private readonly TenancyService _service;

        public TenancyServiceTests()
        {
            _mockClock = new Mock<IClock>();
            _mockHousingApiCall = new Mock<IHackneyHousingAPICall>();
            _mockLastRetrieved = new Mock<INewTenancyService>();
            var mockHousingApiCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();
            var mockCrmToken = new Mock<IHackneyGetCRM365Token>();
            _service = new TenancyService(mockHousingApiCallBuilder.Object, _mockHousingApiCall.Object, mockCrmToken.Object, _mockLastRetrieved.Object, _mockClock.Object);
        }

        private static HttpResponseMessage CreateResponseMessage(Dictionary<string, object> response)
        {
            var responseObject = JObject.FromObject(response);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseObject.ToString(), System.Text.Encoding.UTF8, "application/json") };
            return responseMessage;
        }
        
        private void SetupHousingApiResponse(Dictionary<string, object> response)
        {
            _mockHousingApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(CreateResponseMessage(response));
        }

        [Fact]
        public async Task GetNewTenancies_UpdateLastRunDate()
        {
            SetupHousingApiResponse(new Dictionary<string, object>());
            _mockLastRetrieved.Setup(m => m.UpdateLastRetrieved(It.IsAny<DateTime>())).Verifiable();

            await _service.GetNewTenancies();
            
            _mockLastRetrieved.Verify(m => m.UpdateLastRetrieved(It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task GetNewTenancies_UpdateLastRunDate_WithCorrectDateTime()
        {
            var dateTime = new DateTime(2003, 08, 27, 14, 24, 52, 33);
            
            SetupHousingApiResponse(new Dictionary<string, object>());
            _mockLastRetrieved.Setup(m => m.UpdateLastRetrieved(It.IsAny<DateTime>())).Verifiable();
            _mockClock.Setup(m => m.Now).Returns(dateTime);

            await _service.GetNewTenancies();
            
            _mockLastRetrieved.Verify(m => m.UpdateLastRetrieved(dateTime), Times.Once);
        }

        [Fact]
        public async Task GetNewTenancies_RetrievesPreviousLastRunDate()
        {
            var now = new DateTime(2019, 05, 14, 11, 00, 00);
            var lastRunTime = new DateTime(1995, 03, 15, 22, 55, 33, 21);
            
            SetupHousingApiResponse(new Dictionary<string, object>());
            _mockClock.Setup(m => m.Now).Returns(now);
            _mockLastRetrieved.Setup(m => m.GetLastRetrieved()).Returns(lastRunTime);

            await _service.GetNewTenancies();
            
            _mockLastRetrieved.Verify(m => m.GetLastRetrieved(), Times.Once);
        }

        [Fact]
        public async Task GetNewTenancies_UsesLastRunDate()
        {
            var lastRunTime = new DateTime(1995, 03, 15, 22, 55, 33, 21);
            var queryDate = lastRunTime.ToCrmQueryFormat();

            var calledQuery = "";
            _mockHousingApiCall.Setup(x =>
                    x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((HttpClient client, string query, string parameter) => { calledQuery = query; })
                .ReturnsAsync(CreateResponseMessage(new Dictionary<string, object>()));
            
            _mockLastRetrieved.Setup(m => m.GetLastRetrieved()).Returns(lastRunTime);

            await _service.GetNewTenancies();

            Assert.Contains(queryDate, calledQuery);
        }

        [Fact]
        public async Task GetNewTenancies_UpdatesLastRun_AfterQuery()
        {
            var calledQuery = "";
            _mockHousingApiCall.Setup(x =>
                    x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((HttpClient client, string query, string parameter) => { calledQuery = query; })
                .ReturnsAsync(CreateResponseMessage(new Dictionary<string, object>()));
            
            var newLastRunTime = new DateTime(2018, 07, 5);
            var oldLastRunTime = new DateTime(2018, 06, 10);

            var currentLastRuntime = oldLastRunTime;
            _mockLastRetrieved.Setup(m => m.UpdateLastRetrieved(It.IsAny<DateTime>())).Callback((DateTime date) =>
            {
                currentLastRuntime = date; 
            });
            _mockLastRetrieved.Setup(m => m.GetLastRetrieved()).Returns(() => currentLastRuntime);
            _mockClock.Setup(m => m.Now).Returns(newLastRunTime);
            
            await _service.GetNewTenancies();

            Assert.Contains(oldLastRunTime.ToCrmQueryFormat(), calledQuery);
        }
        
        [Fact]
        public async Task GetNewTenancies_ThereAreNewTenancies_ReturnsListOfNewTenancies()
        {
            var dict = new Dictionary<string, object>
            {
                { "value", GetNewTenanciesResponse() }
            };
            SetupHousingApiResponse(dict);

            var newTenancies = await _service.GetNewTenancies();

            Assert.IsAssignableFrom<IEnumerable<NewTenancyResponse>>(newTenancies);
            Assert.True(newTenancies.Count() == 1);
        }

        [Fact]
        public async Task GetNewTenancies_ThereAreNoNewTenancies_ReturnsEmptyList()
        {
            var dict = new Dictionary<string, object>
            {
                { "value", new string[0] }
            };
            SetupHousingApiResponse(dict);

            var newTenancies = await _service.GetNewTenancies();

            Assert.IsAssignableFrom<IEnumerable<NewTenancyResponse>>(newTenancies);
            Assert.True(newTenancies.Count() == 0);
        }

        private static IList<dynamic> GetNewTenanciesResponse()
        {
            return new List<dynamic>
            {
                new {
                    housing_tenure= "NON",
                    housing_tag_ref= "0121098/01",
                    _hackney_household_accountid_value= "7adfe426-394a-e911-a976-002248072fe8",
                    housing_house_ref= "0121098",
                    accountid= "7edfe426-394a-e911-a976-002248072fe8",
                    createdon= DateTime.Parse("2019-03-19T11:21:37Z"),
                    hackney_propertyareapatch2_x002e_hackney_estateaddress= "Woodberry Down Estate Seven Sisters Road",
                    contact1_x002e_hackney_responsible= true,
                    contact1_x002e_address1_postalcode= "N4 1QU",
                    estateOfficerId= "d8a364c2-0d68-e811-8129-70106faa6a31",
                    contact1_x002e_hackney_title= "Mr",
                    OfficerFullName= "Jillian Inniss",
                    contact1_x002e_contactid= "9d8fc054-3837-e911-a972-002248072fe8",
                    contact1_x002e_firstname= "Charles",
                    contact1_x002e_address1_composite= "FLAT 57 SAVERNAKE HOUSE\r\nWOODBERRY DOWN ESTATE\r\nHACKNEY\r\nLONDON N4 1QU",
                    hackney_estateofficerpatch3_x002e_hackney_patchid= "d8a364c2-0d68-e811-8129-70106faa6a31",
                    contact1_x002e_lastname= "Gordon",
                    OfficerLastName= "Inniss",
                    contact1_x002e_fullname= "Charles Gordon",
                    OfficerFirstName= "Jillian",
                    OfficerEmailAddress= "jillian.inniss@hackney.gov.uk",
                    hackney_propertyareapatch2_x002e_hackney_neighbourhoodofficedesc= "North East Neighbourhood",
                    contact1_x002e_address1_line3= "HACKNEY",
                    contact1_x002e_address1_line2= "WOODBERRY DOWN ESTATE",
                    contact1_x002e_address1_line1= "FLAT 57 SAVERNAKE HOUSE",
                    contact1_x002e_hackney_personno= "1",
                    hackney_propertyareapatch2_x002e_hackney_areaname= 6,
                    hackney_propertyareapatch2_x002e_hackney_managerpropertypatchid= "5512c473-9953-e811-8126-70106faaf8c1"
                }
            };
        }
    }
}