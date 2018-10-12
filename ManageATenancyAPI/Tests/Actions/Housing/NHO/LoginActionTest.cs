using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Tests.Helpers;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Hackney.InterfaceStubs;
using ManageATenancyAPI.Actions;


namespace ManageATenancyAPI.Tests.Actions.Housing.NHO
{
    public class LoginActionTest
    {

        [Fact]
        public async Task get_user_when_username_and_password_match_when_user_is_an_officer()
        {
            var loginResponse = new
            {
                hackney_estateofficerloginid = "login8f6a9cba",
                _hackney_officerloginid_value = "OfficerId70106faa6a31",
                hackney_password = "Password",
                hackney_username = "UserName",
                hackney_estateofficer1_x002e_hackney_estateofficerid = "EstateOfficerId454345",
                hackney_estateofficer1_x002e_hackney_lastname = "Smith",
                hackney_estateofficer1_x002e_hackney_firstname = "Test",
                officerPatchId = "PatchId7yo983o01",
                hackney_estateofficer1_x002e_hackney_name = "Test Smith",
                managerId = null as object,
                OfficermanagerId = "OfficermanagerId",
                OfficerAreaId="AreaId1",
                AreaId = null as object,
            };
            List<dynamic> listJObject = new List<dynamic>();

            listJObject.Add(loginResponse);

            var loginDictionary = new Dictionary<string, object>();
            loginDictionary.Add("value", listJObject);
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            new Mock<ILoggerAdapter<LoginActions>>();

            var mockApiContractSecurity = new Mock<ICryptoMethods>();

            mockApiContractSecurity.Setup(x => x.Encrypt(It.IsAny<string>())).Returns("9FAL6fC3FHBTONmicPWB3w==");

            var query = HousingAPIQueryBuilder.getAuthenticatedUserQuery("UserName", "9FAL6fC3FHBTONmicPWB3w==");
            var mockApiCall = new Mock<IHackneyHousingAPICall>();

            var jsonString = JsonConvert.SerializeObject(loginDictionary);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, "UserName")).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<LoginActions>>();
           

            var loginActions = new LoginActions(mockILoggerAdapter.Object, mockApiCall.Object, mockCallBuilder.Object, mockApiContractSecurity.Object, mockAccessToken.Object);

            var actualresponse = await loginActions.GetAuthenticatedUser("UserName", "HackneyTestPassword");

            var jsonResponse = new
            {
                estateOfficerLoginId = "login8f6a9cba",
                officerId = "OfficerId70106faa6a31",
                firstName = "Test",
                surName = "Smith",
                username = "UserName",
                fullName= "Test Smith",
                isManager = false,
                areamanagerId = "OfficermanagerId",
                officerPatchId = "PatchId7yo983o01",
                areaId = "AreaId1"
            };

            var json = new
            {
                result = jsonResponse
            };

            Assert.Equal(JsonConvert.SerializeObject(json), JsonConvert.SerializeObject(actualresponse));
        }


        [Fact]
        public async Task get_user_when_username_and_password_match_when_user_is_a_manager()
        {
            var loginResponse = new
            {
                hackney_estateofficerloginid = "login8f6a9cba",
                _hackney_officerloginid_value = "OfficerId70106faa6a31",
                hackney_password = "Password",
                hackney_username = "UserName",
                hackney_estateofficer1_x002e_hackney_estateofficerid = "EstateOfficerId454345",
                hackney_estateofficer1_x002e_hackney_lastname = "Smith",
                hackney_estateofficer1_x002e_hackney_firstname = "Test",
                officerPatchId = null as object,
                hackney_estateofficer1_x002e_hackney_name = "Test Smith",
                managerId = "managerId",
                OfficermanagerId = null as object,
                OfficerAreaId = null as object,
                AreaId = "Areaid4",
            };
            List<dynamic> listJObject = new List<dynamic>();

            listJObject.Add(loginResponse);

            var loginDictionary = new Dictionary<string, object>();
            loginDictionary.Add("value", listJObject);

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            new Mock<ILoggerAdapter<LoginActions>>();

            var mockApiContractSecurity = new Mock<ICryptoMethods>();

            mockApiContractSecurity.Setup(x => x.Encrypt(It.IsAny<string>())).Returns("9FAL6fC3FHBTONmicPWB3w==");

            var query = HousingAPIQueryBuilder.getAuthenticatedUserQuery("UserName", "9FAL6fC3FHBTONmicPWB3w==");
            var mockApiCall = new Mock<IHackneyHousingAPICall>();

            var jsonString = JsonConvert.SerializeObject(loginDictionary);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, "UserName")).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<LoginActions>>();
            var loginActions = new LoginActions(mockILoggerAdapter.Object, mockApiCall.Object, mockCallBuilder.Object, mockApiContractSecurity.Object, mockAccessToken.Object);


            var actualresponse = await loginActions.GetAuthenticatedUser("UserName", "HackneyTestPassword");

            var jsonResponse = new
            {
                estateOfficerLoginId = "login8f6a9cba",
                officerId = "OfficerId70106faa6a31",
                firstName = "Test",
                surName = "Smith",
                username = "UserName",
                fullName = "Test Smith",
                isManager = true,
                areamanagerId = "managerId",
                officerPatchId = null as object,
                areaId = "Areaid4"
            };

            var json = new
            {
                result = jsonResponse
            };

            Assert.Equal(JsonConvert.SerializeObject(json), JsonConvert.SerializeObject(actualresponse));
        }


        [Fact]
        public async Task get_user_throws_login_service_exception_result_exception_if_the_service_responds_with_an_error()
        {

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);


            new Mock<ILoggerAdapter<LoginActions>>();

            var mockApiContractSecurity = new Mock<ICryptoMethods>();

            mockApiContractSecurity.Setup(x => x.Encrypt(It.IsAny<string>())).Returns("9FAL6fC3FHBTONmicPWB3w==");

            var query = HousingAPIQueryBuilder.getAuthenticatedUserQuery("UserName", "9FAL6fC3FHBTONmicPWB3w==");
            var mockApiCall = new Mock<IHackneyHousingAPICall>();


            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, "UserName")).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<LoginActions>>();

            var loginActions = new LoginActions(mockILoggerAdapter.Object, mockApiCall.Object, mockCallBuilder.Object, mockApiContractSecurity.Object, mockAccessToken.Object);


            await Assert.ThrowsAsync<LoginServiceException>(async () => await loginActions.GetAuthenticatedUser("UserName", "HackneyTestPassword"));
        }

        [Fact]
        public async Task get_user_throws_missing_result_exception_if_the_service_responds_with_an_error()
        {

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);
            
            new Mock<ILoggerAdapter<LoginActions>>();

            var query = HousingAPIQueryBuilder.getAuthenticatedUserQuery("UserName", "HackneyTestPassword");
            var mockApiCall = new Mock<IHackneyHousingAPICall>();

            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, "UserName")).ReturnsAsync((HttpResponseMessage)null);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<LoginActions>>();

            var mockApiContractSecurity = new Mock<ICryptoMethods>();

            mockApiContractSecurity.Setup(x => x.Encrypt(It.IsAny<string>())).Returns("9FAL6fC3FHBTONmicPWB3w==");
            var loginActions = new LoginActions(mockILoggerAdapter.Object, mockApiCall.Object, mockCallBuilder.Object, mockApiContractSecurity.Object, mockAccessToken.Object);

            await Assert.ThrowsAsync<MissingLoginResultException>(async () => await loginActions.GetAuthenticatedUser("UserName", "HackneyTestPassword"));
        }

        [Fact]
        public async Task return_empty_result_when_username_password_donot_match()
        {

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);


            new Mock<ILoggerAdapter<LoginActions>>();

            var mockApiContractSecurity = new Mock<ICryptoMethods>();

            mockApiContractSecurity.Setup(x => x.Encrypt(It.IsAny<string>())).Returns("9FAL6fC3FHBTONmicPWB3w==");

            var query = HousingAPIQueryBuilder.getAuthenticatedUserQuery("User", "9FAL6fC3FHBTONmicPWB3w==");

            var mockApiCall = new Mock<IHackneyHousingAPICall>();

            var loginDictionary = new Dictionary<string, object>();
            loginDictionary.Add("value", "");
            var jsonString = JsonConvert.SerializeObject(loginDictionary);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, "User")).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<LoginActions>>();
            var loginActions = new LoginActions(mockILoggerAdapter.Object, mockApiCall.Object, mockCallBuilder.Object, mockApiContractSecurity.Object, mockAccessToken.Object);

            var actualresponse = await loginActions.GetAuthenticatedUser("User", "HackneyTestPassword");

            var json = new
            {
                result = new object()
            };
            Assert.Equal(JsonConvert.SerializeObject(json), JsonConvert.SerializeObject(actualresponse));
        }
    }
}
