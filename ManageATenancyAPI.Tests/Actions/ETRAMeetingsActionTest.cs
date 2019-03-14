using System;
using System.Collections.Generic;
using System.Dynamic;
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

        [Fact]
        public async Task RecordETRAMeetingAttendance_WithMeetingIdAndPopulatedRequestObject_ReturnsSuccessfulResponse()
        {
            const string fakeMeetingId = "id123";
            var request = GetRandomMeetingAttendanceRequest();
            var service = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);
            var responseJObject = new JObject {
                {"annotationid", Guid.NewGuid() }
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(responseJObject.ToString(), System.Text.Encoding.UTF8, "application/json") };
            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responseMessage);

            var response = await service.RecordETRAMeetingAttendance(fakeMeetingId, request);

            Assert.True(response.Recorded);
        }

        [Fact]
        public async Task RecordETRAMeetingAttendance_APICallFails_ThrowsTenancyServiceException()
        {
            const string fakeMeetingId = "id123";
            var service = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responseMessage);

            var request = GetRandomMeetingAttendanceRequest();

            async Task act() => await service.RecordETRAMeetingAttendance(fakeMeetingId, request);

            await Assert.ThrowsAsync<TenancyServiceException>(act);
        }

        [Fact]
        public async Task FinaliseMeeting_WithMeetingIdAndPopulatedRequestObject_ReturnsSuccessfulResponse()
        {
            const string fakeMeetingId = "id123";
            var fakeFinalisationRequest = GetRandomMeetingFinalisationRequest();
            var service = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);
            var responseJObject = new JObject {
                {"annotationid", Guid.NewGuid() }
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(responseJObject.ToString(), System.Text.Encoding.UTF8, "application/json") };
            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responseMessage);

            var response = await service.FinaliseMeeting(fakeMeetingId, fakeFinalisationRequest);

            Assert.True(response.IsFinalised);
        }

        [Fact]
        public async Task FinaliseMeeting_WithMeetingIdAndNullRequestObject_ReturnsSuccessfulResponse()
        {
            const string fakeMeetingId = "id123";
            var service = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);
            var responseJObject = new JObject {
                {"annotationid", Guid.NewGuid() }
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(responseJObject.ToString(), System.Text.Encoding.UTF8, "application/json") };
            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responseMessage);

            var response = await service.FinaliseMeeting(fakeMeetingId, null);

            Assert.True(response.IsFinalised);
        }

        [Fact]
        public async Task FinaliseMeeting_APICallFails_ThrowsTenancyServiceException()
        {
            const string fakeMeetingId = "id123";
            var service = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responseMessage);

            async Task act() => await service.FinaliseMeeting(fakeMeetingId, null);

            await Assert.ThrowsAsync<TenancyServiceException>(act);
        }

        [Fact]
        public async Task GetMeeting_HousingApiNullResponse_ThrowsNullResponseException()
        {
            const string fakeMeetingId = "id123";
            var service = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);
            HttpResponseMessage responseMessage = null;
            mockingApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(responseMessage);

            async Task act() => await service.GetMeeting(fakeMeetingId);

            await Assert.ThrowsAsync<NullResponseException>(act);
        }

        [Fact]
        public async Task GetMeeting_HousingApiNonSuccessStatusCode_ThrowsTenancyServiceException()
        {
            const string fakeMeetingId = "id123";
            var service = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            mockingApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(responseMessage);

            async Task act() => await service.GetMeeting(fakeMeetingId);

            await Assert.ThrowsAsync<TenancyServiceException>(act);
        }

        [Fact]
        public async Task GetMeeting_ValidMeetingId_ReturnsCorrectMeetingObject()
        {
            const string fakeMeetingId = "id123";
            var service = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);
            var responseJObject = GetRandomETRAMeeting(true);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(responseJObject.ToString(), System.Text.Encoding.UTF8, "application/json") };
            mockingApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(responseMessage);

            var response = await service.GetMeeting(fakeMeetingId);

            Assert.Equal(responseJObject["hackney_tenancymanagementinteractionsid"], response.Id);
        }

        public JObject GetRandomETRAMeeting(bool isConfirmed)
        {
            var fakeData = new Faker();
            var meeting = new JObject {
                { "hackney_tenancymanagementinteractionsid", fakeData.Random.Guid() },
                { "hackney_name", fakeData.Random.String() },
                { "createdon", DateTime.Now.AddDays(-2) },
                { "modifiedon", DateTime.Now.AddDays(-1) },
                { "hackney_confirmationdate", null },
                { "hackney_signaturereference", null },
                { "hackney_signatoryrole", null },
                { "hackney_pdfreference", null }
            };

            if (isConfirmed)
            {
                meeting["hackney_confirmationdate"] = DateTime.Now;
                meeting["hackney_signaturereference"] = fakeData.Random.Guid();
                meeting["hackney_signatoryrole"] = fakeData.Random.String();
                meeting["hackney_pdfreference"] = fakeData.Random.Guid();
            }

            return meeting;
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

        public FinaliseETRAMeetingRequest GetRandomMeetingFinalisationRequest()
        {
            var fakeData = new Faker();
            var finalisationObject = new FinaliseETRAMeetingRequest
            {
                Role = fakeData.Random.String(),
                SignatureId = fakeData.Random.Guid()
            };
            return finalisationObject;
        }

        public RecordETRAMeetingAttendanceRequest GetRandomMeetingAttendanceRequest()
        {
            var fakeData = new Faker();
            var attendanceObject = new RecordETRAMeetingAttendanceRequest
            {
                Councillors = fakeData.Random.String(),
                OtherCouncilStaff = fakeData.Random.String(),
                TotalAttendees = fakeData.Random.Int(2, 200)
            };
            return attendanceObject;
        }

        #region Get ETRA Issues
        [Fact]
        public async Task get_tenancy_incident_details_based_on_id_and_user_type()
        {
            mockingApiCall.Setup(x => x.getHousingAPIResponse(client, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(getIssuesDetailList());

            ETRAMeetingsAction tmiActions = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            var actualResponse = tmiActions.GetETRAIssuesByTRAorETRAMeeting(It.IsAny<string>(),false).Result;

            object listOfIssues = getIssuesListResult();

            Assert.Equal(JsonConvert.SerializeObject(listOfIssues), JsonConvert.SerializeObject(actualResponse));
        }

        [Fact]
        public async Task get_tenancy_incidents_raises_exception_if_service_responds_with_error()
        {

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            HttpResponseMessage serviceResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("", System.Text.Encoding.UTF8, "application/json") };
            mockingApiCall.Setup(x => x.getHousingAPIResponse(client, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(serviceResponse);
            
            ETRAMeetingsAction tmiActions = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<TenancyServiceException>(async () => await tmiActions.GetETRAIssuesByTRAorETRAMeeting(It.IsAny<string>(),false));
        }

        [Fact]
        public async Task get_tenancy_incidents_raises_exception_if_service_responds_with_null_result()
        {
            var mockApiCall = new Mock<IHackneyHousingAPICall>();

            mockApiCall.Setup(x => x.getHousingAPIResponse(client, It.IsAny<string>(), null)).ReturnsAsync(() => null);
            
            ETRAMeetingsAction tmiActions = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<NullResponseException>(async () =>  await tmiActions.GetETRAIssuesByTRAorETRAMeeting(It.IsAny<string>(), false));
        }
        private HttpResponseMessage getIssuesDetailList()
        {
            var TenancyManagement = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();

            value["hackney_name"] = "CAS-00059-000000";
            value["hackney_transferred"] = true;
            value["statecode"] = 0;
            value["hackney_tenancymanagementinteractionsid"] = "d9f0fd60-b5e0-e711-810f-111111";
            value["_housing_contact_value"] = "463adffe-61a5-db11-882c-000000000000";
            value["createdon@OData.Community.Display.V1.FormattedValue"] = "14/12/2017 09:58";
            value["createdon"] = "2017-12-14T09:58:49Z";
            value["hackney_estateoffice@OData.Community.Display.V1.FormattedValue"] = "Homerton Housing Neighbourhood";
            value["hackney_enquirysubject@OData.Community.Display.V1.FormattedValue"] = "Apply for Joint Tenancy";
            value["hackney_enquirysubject"] = "100000005";
            value["hackney_natureofenquiry@OData.Community.Display.V1.FormattedValue"] = "Estate Managment";
            value["hackney_natureofenquiry"] = "3";
            value["_hackney_estateofficer_createdbyid_value"] = "284216e9-d365-e711-80f9-70106aaaaaaa";
            value["annotation2_x002e_annotationid"] = "b6521622-54e6-e711-8111-7010bbbbbbbb";
            value["annotation2_x002e_createdon@OData.Community.Display.V1.FormattedValue"] = "21/12/2017 13:37";
            value["annotation2_x002e_createdon"] = "2017-12-21T13:37:49Z";
            value["annotation2_x002e_notetext"] = "Testing closure  at 21/12/2017 13:37:18 by  Test dev";
            value["_hackney_estateofficer_createdbyid_value@OData.Community.Display.V1.FormattedValue"] = "Test Test";
            value["_hackney_incidentid_value"] = "39141263-b0e0-e711-810a-e0071bbbbbbb";
            value["_hackney_managerpropertypatchid_value"] = "AreaManager28645uyo980";
            value["ManagerFirstName"] = "Area";
            value["ManagerLastName"] = "Manager Name";
            value["_hackney_estateofficerpatchid_value"] = "OfficerPatch9684056oi046";
            value["OfficerFirstName"] = "Officer";
            value["OfficerLastName"] = "Patch name";
            value["hackney_areaname"] = "Homerton";
            value["hackney_traid"] = "1";
            value["hackney_issuelocation"] = "test location";
            value["hackney_processtype"] = "1";


            listJObject.Add(value);
            TenancyManagement.Add("value", listJObject);
            string jsonString = JsonConvert.SerializeObject(TenancyManagement);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };
            return response;
        }

        private object getIssuesListResult()
        {
            var tenancyList = new List<dynamic>();
            dynamic tenancyObj = new ExpandoObject();

            tenancyObj.incidentId = "39141263-b0e0-e711-810a-e0071bbbbbbb";
            tenancyObj.isTransferred = true;
            tenancyObj.ticketNumber = "CAS-00059-000000";
            tenancyObj.stateCode = 0;
            tenancyObj.processStage = null;
            tenancyObj.nccOfficersId = "284216e9-d365-e711-80f9-70106aaaaaaa";
            tenancyObj.nccOfficerName = "Test Test";
            tenancyObj.createdon = "2017-12-14 09:58:49";
            tenancyObj.nccOfficerUpdatedById = null;
            tenancyObj.nccOfficerUpdatedByName = null;
            tenancyObj.natureOfEnquiryId = "3";
            tenancyObj.natureOfEnquiry = "Estate Managment";
            tenancyObj.enquirySubjectId = "100000005";
            tenancyObj.enquirysubject = "Apply for Joint Tenancy";
            tenancyObj.interactionId = "d9f0fd60-b5e0-e711-810f-111111";
            tenancyObj.areamanagerId = "AreaManager28645uyo980";
            tenancyObj.areaManagerName = "Area Manager Name";
            tenancyObj.officerPatchId = "OfficerPatch9684056oi046";
            tenancyObj.officerPatchName = "Officer Patch name";
            tenancyObj.areaName = null;
            tenancyObj.traId = "1";
            tenancyObj.issueLocation = "test location";
            tenancyObj.processType = "1";
            tenancyObj.AnnotationList = new List<ExpandoObject>();
            dynamic annotation = new ExpandoObject();
            annotation.noteText = "Testing closure  at 21/12/2017 13:37:18 by  Test dev";
            annotation.annotationId = "b6521622-54e6-e711-8111-7010bbbbbbbb";
            annotation.noteCreatedOn = "2017-12-21 13:37:49";
           
            tenancyObj.AnnotationList.Add(annotation);
            tenancyList.Add(tenancyObj);
            return new
            {
                results = tenancyList
            };
        }




        #endregion

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
        public async Task update_should_throw_an_exception_when_a_service_responds_with_error()
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

            HttpResponseMessage responsMessageUpdateIncident = new HttpResponseMessage(HttpStatusCode.BadGateway) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };
            HttpResponseMessage responsMessageDeleteInteraction = new HttpResponseMessage(HttpStatusCode.NoContent) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };

            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessageUpdateIncident);
            mockingApiCall.Setup(x => x.deleteObjectAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>())).ReturnsAsync(responsMessageDeleteInteraction);

            ETRAMeetingsAction tmiActions = new ETRAMeetingsAction(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<TenancyServiceException>(async () => await tmiActions.UpdateIssue(requestObject));
        }


        #endregion
    }
}
