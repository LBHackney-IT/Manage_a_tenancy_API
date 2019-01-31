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
            mockConfig.SetupGet(x => x.Value).Returns(new AppConfiguration());

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
            expectedResponse.Add("incidentId", createdServiceRequest["incidentid"]);
            expectedResponse.Add("ticketnumber", createdServiceRequest["ticketnumber"]);
            expectedResponse.Add("annotationId", createdServiceRequest["annotationid"]);


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
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            mockingApiCall.Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(() => null);

            ETRAMeetingsAction tmiActions = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<MissingTenancyInteractionRequestException>(async () => await tmiActions.CreateETRAMeeting(getRandomInteractionObject()));
        }

        [Fact]
        public async Task successful_etra_issue_creation_should_return_etra_object()
        {
            var fakeServiceRequest = getRandomServiceRequestObject();
            string jsonString = JsonConvert.SerializeObject(fakeServiceRequest);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            mockingApiCall.Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            ETRAMeetingsAction tmiActions = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            var actualResponse = tmiActions.CreateETRAMeeting(getRandomInteractionObjectForIssueCreation()).Result;

            JObject createdServiceRequest = JsonConvert.DeserializeObject<JObject>(responsMessage.Content.ReadAsStringAsync().Result);
            var expectedResponse = new JObject();
            expectedResponse.Add("interactionid", null);
            expectedResponse.Add("incidentId", createdServiceRequest["incidentid"]);
            expectedResponse.Add("ticketnumber", createdServiceRequest["ticketnumber"]);
            expectedResponse.Add("annotationId", createdServiceRequest["annotationid"]);


            Assert.Equal(JsonConvert.SerializeObject(actualResponse), JsonConvert.SerializeObject(HackneyResult<JObject>.Create(expectedResponse)));
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
            serviceJObject.Add("annotationid", fakeData.Random.Guid());

            return serviceJObject;
        }

        public ETRAIssue getRandomInteractionObject()
        {
            var fakeData = new Faker();
            var interactionJObject = new ETRAIssue();

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
            interactionJObject.processType = fakeData.Random.String();
            interactionJObject.natureOfEnquiry = fakeData.Random.String();
            interactionJObject.enquirySubject = fakeData.Random.String();

            return interactionJObject;
        }

        public ETRAIssue getRandomInteractionObjectForIssueCreation()
        {
            var fakeData = new Faker();
            var interactionJObject = new ETRAIssue();

            var serviceRequest = new CRMServiceRequest();
            serviceRequest.Description = fakeData.Random.String();
            serviceRequest.Subject = fakeData.Random.Guid().ToString();
            serviceRequest.CreatedBy = fakeData.Random.Guid().ToString();

            interactionJObject.TRAId = fakeData.Random.String();
            interactionJObject.areaName = fakeData.Random.String();
            interactionJObject.estateOfficerId = fakeData.Random.Guid().ToString();
            interactionJObject.managerId = fakeData.Random.Guid().ToString();
            interactionJObject.officerPatchId = fakeData.Random.Guid().ToString();
            interactionJObject.ServiceRequest = serviceRequest;
            interactionJObject.subject = fakeData.Random.Guid().ToString();
            interactionJObject.estateOfficerName = fakeData.Random.String();
            interactionJObject.processType = fakeData.Random.String();
            interactionJObject.natureOfEnquiry = fakeData.Random.String();
            interactionJObject.enquirySubject = fakeData.Random.String();
            interactionJObject.issueLocation = fakeData.Random.String();

            return interactionJObject;
        }

        #region Update ETRA Issue
        [Fact]
        public async Task update_issue_succesfully_deletes_issue_if_issueIsToBeDeleted_is_true()
        {
            var faker = new Faker();
            var requestObject = new UpdateETRAIssue()
            {
                note = faker.Random.String(),
                issueIncidentId = faker.Random.Guid(),
                issueInteractionId = faker.Random.Guid(),
                estateOfficerId = faker.Random.String(),
                estateOfficerName = faker.Random.String(),
                isNewNote = false,
                issueIsToBeDeleted = true
            };

            HttpResponseMessage responsMessageUpdateIncident = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };
            HttpResponseMessage responsMessageDeleteInteraction = new HttpResponseMessage(HttpStatusCode.NoContent) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };

            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessageUpdateIncident);
            mockingApiCall.Setup(x => x.deleteObjectAPIResponse(It.IsAny<HttpClient>(),  It.IsAny<string>())).ReturnsAsync(responsMessageDeleteInteraction);

            ETRAMeetingsAction tmiActions = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            var actualResponse = tmiActions.UpdateIssue(requestObject).Result;
            var expectedResult = new JObject();
            expectedResult.Add("interactionId", requestObject.issueInteractionId);
            expectedResult.Add("incidentId", requestObject.issueIncidentId);
            expectedResult.Add("action", "deleted");
            Assert.Equal(JsonConvert.SerializeObject(expectedResult),JsonConvert.SerializeObject(actualResponse));
        }

        [Fact]
        public async Task update_issue_action_returned_is_updated_if_issueIsToBeDeleted_is_false()
        {
            var faker = new Faker();
            var requestObject = new UpdateETRAIssue()
            {
                note = faker.Random.String(),
                issueIncidentId = faker.Random.Guid(),
                issueInteractionId = faker.Random.Guid(),
                estateOfficerId = faker.Random.String(),
                estateOfficerName = faker.Random.String(),
                isNewNote = false,
                issueIsToBeDeleted = false
            };

            HttpResponseMessage responsMessageUpdateIncident = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };
            HttpResponseMessage responsMessageDeleteInteraction = new HttpResponseMessage(HttpStatusCode.NoContent) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };

            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessageUpdateIncident);
            mockingApiCall.Setup(x => x.deleteObjectAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>())).ReturnsAsync(responsMessageDeleteInteraction);

            ETRAMeetingsAction tmiActions = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            var actualResponse = tmiActions.UpdateIssue(requestObject).Result;
            var expectedResult = new JObject();
            expectedResult.Add("interactionId", requestObject.issueInteractionId);
            expectedResult.Add("incidentId", requestObject.issueIncidentId);
            expectedResult.Add("action", "updated");
            Assert.Equal(JsonConvert.SerializeObject(expectedResult), JsonConvert.SerializeObject(actualResponse));
            Assert.Equal("updated", actualResponse["action"]);
        }


        [Fact]
        public async Task update_annotation_should_execute_succesfully_without_exceptions_being_th()
        {
            var faker = new Faker();
            var requestObject = new UpdateETRAIssue()
            {
                note = faker.Random.String(),
                issueIncidentId = faker.Random.Guid(),
                issueInteractionId = faker.Random.Guid(),
                estateOfficerId = faker.Random.String(),
                estateOfficerName = faker.Random.String(),
                isNewNote = false,
                issueIsToBeDeleted = false
            };

            HttpResponseMessage responsMessageUpdateIncident = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };
            HttpResponseMessage responsMessageDeleteInteraction = new HttpResponseMessage(HttpStatusCode.NoContent) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };

            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessageUpdateIncident);
            mockingApiCall.Setup(x => x.deleteObjectAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>())).ReturnsAsync(responsMessageDeleteInteraction);

            ETRAMeetingsAction tmiActions = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            var actualResponse = tmiActions.UpdateIssue(requestObject).Result;
            var expectedResult = new JObject();
            expectedResult.Add("interactionId", requestObject.issueInteractionId);
            expectedResult.Add("incidentId", requestObject.issueIncidentId);
            expectedResult.Add("action", "updated");
            Assert.Equal(JsonConvert.SerializeObject(expectedResult), JsonConvert.SerializeObject(actualResponse));
            Assert.Equal("updated", actualResponse["action"]);
        }
        #endregion
    }
}
