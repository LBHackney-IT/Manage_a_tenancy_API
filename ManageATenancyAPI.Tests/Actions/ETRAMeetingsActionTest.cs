using System;
using System.Collections.Generic;
using System.Management.Automation.Language;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Tests.Helpers;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ManageATenancyAPI.Tests.Actions
{
    public class ETRAMeetingsActionTest
    {
        public string token;
        Mock<IHackneyHousingAPICallBuilder> mocktmiCallBuilder;
        private HttpClient client;
        private Mock<IHackneyHousingAPICall> mockingApiCall;
        private Mock<ILoggerAdapter<ETRAMeetingsAction>> mockILoggerAdapter;
        private Mock<IOptions<AppConfiguration>> mockConfig;
        private Mock<IHackneyGetCRM365Token> mockAccessToken;
        public ETRAMeetingsActionTest()
        { 
            token = "TokenOnValidRequest";
            mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(token);
            mocktmiCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();
            client = FakeHackneyHousingAPICallBuilder.createFakeRequest(token);
            mocktmiCallBuilder.Setup(x => x.CreateRequest(token)).ReturnsAsync(client);
            mockingApiCall = new Mock<IHackneyHousingAPICall>();
            mockILoggerAdapter = new Mock<ILoggerAdapter<ETRAMeetingsAction>>();
            mockConfig = new Mock<IOptions<AppConfiguration>>();

        }

        [Fact]
        public async Task successful_etra_meeting_should_return_etra_object()
        {
            var fakeServiceRequest = getRandomServiceRequestObject();
            string jsonString = JsonConvert.SerializeObject(fakeServiceRequest);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            mockingApiCall.Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            ETRAMeetingsAction tmiActions = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            var actualResponse = tmiActions.CreateETRAMeeting(getRandomInteractionObject()).Result;

            JObject createdServiceRequest = JsonConvert.DeserializeObject<JObject>(responsMessage.Content.ReadAsStringAsync().Result);
            var expectedResponse = new JObject();
            expectedResponse.Add("interactionid", null);
            expectedResponse.Add("ticketnumber", createdServiceRequest["ticketnumber"]);

            Assert.Equal(JsonConvert.SerializeObject(actualResponse), JsonConvert.SerializeObject(HackneyResult<JObject>.Create(expectedResponse)));
        }

        [Fact]
        public async Task etra_meeting_throws_an_error_if_server_responds_with_non_success_status()
        {
            var fakeServiceRequest = getRandomServiceRequestObject();
            string jsonString = JsonConvert.SerializeObject(fakeServiceRequest);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.BadGateway) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            mockingApiCall.Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            ETRAMeetingsAction tmiActions = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);
            
            await Assert.ThrowsAsync<ServiceRequestException>(async () => await tmiActions.CreateETRAMeeting(getRandomInteractionObject()));
        }

        [Fact]
        public async Task etra_meeting_throws_a_missing_result_null_server_response()
        {
            var fakeServiceRequest = getRandomServiceRequestObject();
            string jsonString = JsonConvert.SerializeObject(fakeServiceRequest);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            mockingApiCall.Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(() => null);

            ETRAMeetingsAction tmiActions = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<MissingTenancyInteractionRequestException>(async () => await tmiActions.CreateETRAMeeting(getRandomInteractionObject()));
        }

        public JObject getRandomServiceRequestObject()
        {
            var fakeData = new Faker();
            var serviceJObject = new JObject();

            serviceJObject.Add("description", fakeData.Random.String());
            serviceJObject.Add("ticketnumber", fakeData.Random.String());
            serviceJObject.Add("title", fakeData.Random.String());
            serviceJObject["_subjectid_value"] = fakeData.Random.Guid();
            serviceJObject["_customerid_value"] = fakeData.Random.Guid();
            serviceJObject.Add("incidentid", fakeData.Random.String());

            return serviceJObject;
        }

        public ETRA getRandomInteractionObject()
        {
            var fakeData = new Faker();
            var interactionJObject = new ETRA();

            var serviceRequest = new CRMServiceRequest();
            serviceRequest.Description = fakeData.Random.String();
            serviceRequest.Subject = fakeData.Random.Guid().ToString();
            serviceRequest.CreatedBy = fakeData.Random.Guid().ToString();

            interactionJObject.TRAId = fakeData.Random.String();
            interactionJObject.areaName= fakeData.Random.String();
            interactionJObject.estateOfficerId = fakeData.Random.Guid().ToString();
            interactionJObject.managerId = fakeData.Random.Guid().ToString();
            interactionJObject.officerPatchId = fakeData.Random.Guid().ToString();
            interactionJObject.ServiceRequest = serviceRequest;
            interactionJObject.subject = fakeData.Random.Guid().ToString();
            interactionJObject.estateOfficerName = fakeData.Random.String();
            return interactionJObject;
        }
    }
}
