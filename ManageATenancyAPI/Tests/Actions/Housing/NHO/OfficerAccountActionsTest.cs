using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Tests.Helpers;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using Xunit;
using ManageATenancyAPI.Models.Housing.NHO;
using System.Threading.Tasks;
using Hackney.InterfaceStubs;

namespace ManageATenancyAPI.Tests.Actions.Housing.NHO
{
    public class OfficerAccountActionsTest
    {
        [Fact]
        [Trait("OfficerAccountActionsTest", "Unit")]
        public void should_handle_the_exception_when_there_is_an_api_post_body_object_is_null()
        {
         
            var mockLoggerAdapter = new Mock<ILoggerAdapter<OfficerAccountActions>>();
            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            var mockApiContractSecurity = new Mock<ICryptoMethods>();
            
            var sut = new OfficerAccountActions(mockLoggerAdapter.Object, mockApiCall.Object, mockCallBuilder.Object, mockApiContractSecurity.Object,mockAccessToken.Object);

            var actual = Assert.ThrowsAsync<AccountServiceException>(() => sut.CreateOfficerAccount(It.IsAny<EstateOfficerAccount>()));

            Assert.Equal(null, actual.Exception.Source);
         
        }

        [Fact]
        [Trait("OfficerAccountActionsTest", "Unit")]
        public void should_handle_the_exception_gracefully_if_there_is_an_exception_throws_when_persisting_PersistofficerAccount_method_call()
        {
            var estateOfficerAccount = new EstateOfficerAccount()
            {
                OfficerAccount = new OfficerAccount() { HackneyFirstname = "Test First", HackneyLastname = "Test_Last", HackneyEmailaddress = "flast@test.com" },
                OfficerLoginAccount = new OfficerLoginAccount() { HackneyUsername = "Test user Name", HackneyPassword = "HackneyTestPassword" }
            };

       
            var mockLoggerAdapter = new Mock<ILoggerAdapter<OfficerAccountActions>>();
            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);
            var mockApiContractSecurity = new Mock<ICryptoMethods>();
            
            var sut = new Mock<OfficerAccountActions>(mockLoggerAdapter.Object, mockApiCall.Object, mockCallBuilder.Object, mockApiContractSecurity.Object, mockAccessToken.Object);

            sut.Protected().Setup<Task<JObject>>("PersistofficerAccount", estateOfficerAccount).ThrowsAsync(new Exception() { Source = "Exception was thrown when data persits to the estate officer entity in CRM" });

            var expectedException = Assert.ThrowsAsync<Exception>(() => sut.Object.CreateOfficerAccount(estateOfficerAccount));

            Assert.Equal("Exception was thrown when data persits to the estate officer entity in CRM", expectedException.Result.Source);
        }

        [Fact]
        [Trait("OfficerAccountActionsTest", "Unit")]
        public void should_return_valid_result_when_there_is_a_successfull_persistent_to_the_officeraccount_and_officerloginaccount_entites_in_crm()
        {
            var estateOfficerAccount = new EstateOfficerAccount()
            {
                OfficerAccount = new OfficerAccount() { HackneyFirstname = "Test First", HackneyLastname = "Test_Last", HackneyEmailaddress = "flast@test.com" },
                OfficerLoginAccount = new OfficerLoginAccount() { HackneyUsername = "Test user Name", HackneyPassword = "HackneyTestPassword" }
            };


            var mockLoggerAdapter = new Mock<ILoggerAdapter<OfficerAccountActions>>();
            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);
            var mockApiContractSecurity = new Mock<ICryptoMethods>();
            
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<JObject>())).Returns(Task.FromResult(responsMessage));

            var sut = new Mock<OfficerAccountActions>(mockLoggerAdapter.Object, mockApiCall.Object, mockCallBuilder.Object, mockApiContractSecurity.Object, mockAccessToken.Object);

            JObject result = JObject.Parse($"{{\"EstateOfficerid\": \"fa164f0b-a031-e811-811a-70106faaf8c1\",  \"Name\": \"TestFL\",  \"FirstName\": \"TestF\",  \"LastName\": \"TestL\", \"OfficerAccountStatus\": \"Active\", \"EstateOfficerLoginId\": \"fb164f0b-a031-e811-811a-70106faaf8c1\", \"UserName\": \"TestUser\", \"LoginAccountStatus\": \"Active\"}}");

            sut.Protected().Setup<Task<JObject>>("PersistofficerAccount", ItExpr.IsAny<EstateOfficerAccount>()).ReturnsAsync(result).Verifiable();

            var actual = new { result = result };

            var expected = sut.Object.CreateOfficerAccount(estateOfficerAccount).Result;

            sut.Verify();

            Assert.Equal(expected, actual);
        }

        [Fact]
        [Trait("OfficerAccountActionsTest", "Unit")]
        public void should_handle_the_exception_gracefully_if_there_is_an_exception_throws_when_persisting_persistofficerloginAccount_method_call()
        {
            var estateOfficerAccount = new EstateOfficerAccount()
            {
                OfficerAccount = new OfficerAccount() { HackneyFirstname = "Test First", HackneyLastname = "Test_Last", HackneyEmailaddress = "flast@test.com" },
                OfficerLoginAccount = new OfficerLoginAccount() { HackneyUsername = "Test user Name", HackneyPassword = "HackneyTestPassword" }
            };
            
            var mockLoggerAdapter = new Mock<ILoggerAdapter<OfficerAccountActions>>();
            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);
           var mockApiContractSecurity = new Mock<ICryptoMethods>();

            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<JObject>())).Returns(Task.FromResult(responsMessage));

            var sut = new OfficerAccountActions(mockLoggerAdapter.Object, mockApiCall.Object, mockCallBuilder.Object, mockApiContractSecurity.Object, mockAccessToken.Object);

            Assert.ThrowsAsync<AccountServiceException>(() => sut.CreateOfficerAccount(estateOfficerAccount));
        }

        [Fact]
        [Trait("OfficerAccountActionsTest", "Unit")]
        public void should_find_user_exists()
        {
            var estateOfficerAccount = new EstateOfficerAccount()
            {
                OfficerAccount = new OfficerAccount() { HackneyFirstname = "Test First", HackneyLastname = "Test_Last", HackneyEmailaddress = "flast@test.com" },
                OfficerLoginAccount = new OfficerLoginAccount() { HackneyUsername = "Test user Name", HackneyPassword = "HackneyTestPassword" }
            };


            var mockLoggerAdapter = new Mock<ILoggerAdapter<OfficerAccountActions>>();
            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);
         
            var mockApiContractSecurity = new Mock<ICryptoMethods>();
         
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<JObject>())).Returns(Task.FromResult(responsMessage));

            var sut = new Mock<OfficerAccountActions>(mockLoggerAdapter.Object, mockApiCall.Object, mockCallBuilder.Object, mockApiContractSecurity.Object, mockAccessToken.Object);

            sut.Protected().Setup<Task<Boolean>>("IsUserNameExist", ItExpr.IsAny<EstateOfficerAccount>()).ReturnsAsync(true).Verifiable();

            var response = sut.Object.CreateOfficerAccount(estateOfficerAccount).Result;

            var result = response.ToString().Contains("StatusCode: 409");

            var actual = result == true ? HttpStatusCode.Conflict : HttpStatusCode.OK;

            sut.Verify();

            Assert.Equal(HttpStatusCode.Conflict, actual);


        }       

    }

}
