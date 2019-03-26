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
using Xunit;

namespace ManageATenancyAPI.Tests.Unit.Services
{
    public class TenancyServiceTests
    {
        private readonly Mock<IHackneyHousingAPICall> _mockHousingApiCall;
        private readonly TenancyService _service;

        public TenancyServiceTests()
        {
            _mockHousingApiCall = new Mock<IHackneyHousingAPICall>();
            var mockHousingApiCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();
            var mockCrmToken = new Mock<IHackneyGetCRM365Token>();
            _service = new TenancyService(mockHousingApiCallBuilder.Object, _mockHousingApiCall.Object, mockCrmToken.Object);
        }

        [Fact]
        public async Task GetNewTenancies_ThereAreNewTenancies_ReturnsListOfNewTenancies()
        {
            var dict = new Dictionary<string, object>
            {
                { "value", GetNewTenanciesResponse() }
            };
            var responseObject = JObject.FromObject(dict);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseObject.ToString(), System.Text.Encoding.UTF8, "application/json") };
            _mockHousingApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(responseMessage);

            var newTenancies = await _service.GetNewTenancies();

            Assert.IsAssignableFrom<IEnumerable<NewTenancyResponse>>(newTenancies);
            Assert.True(newTenancies.Count() == 1);
        }

        [Fact]
        public async Task GetNewTenanciesForHousingOfficer_ThereAreNoNewTenancies_ReturnsEmptyList()
        {
            var dict = new Dictionary<string, object>
            {
                { "value", new string[0] }
            };
            var responseObject = JObject.FromObject(dict);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseObject.ToString(), System.Text.Encoding.UTF8, "application/json") };
            _mockHousingApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(responseMessage);

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