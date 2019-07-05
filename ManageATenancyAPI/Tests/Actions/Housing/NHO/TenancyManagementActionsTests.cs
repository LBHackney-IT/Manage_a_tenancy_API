using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Helpers.Housing;
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

namespace ManageATenancyAPI.Tests.Actions.Housing.NHO
{
    public class TenancyManagementActionsTests
    {
        #region TenancyManagement Interaction

        [Fact]
        public async Task create_tenancy_Management_interaction_returns_a_create_object()
        {
            #region ServiceRequest

            var response = "TokenOnValidRequest";

           
            var mocktmiCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mocktmiCallBuilder.Setup(x => x.CreateRequest(response)).ReturnsAsync(client);


            var serviceJObject = new JObject();

            serviceJObject.Add("description", "Enquiry Created By Estate Officer");
            serviceJObject.Add("ticketnumber", "ticketnumber");
            serviceJObject.Add("title", "Tenancy Management");
            serviceJObject["_subjectid_value"] = "c1f72d01-28dc-e711-8115-70106aaaaaaa";
            serviceJObject["_customerid_value"] = "463adffe-61a5-db11-882c-000000000000";
            serviceJObject.Add("incidentid", "incidentid");
            serviceJObject.Add("annotationid", "63a0e5b9-88df-e311-b8e5-6c3be5ccccccc");

            string jsonString = JsonConvert.SerializeObject(serviceJObject);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            var mockingApiCall = new Mock<IHackneyHousingAPICall>();
            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessage);


            #endregion ServiceRequest



            var tmiJObject = new JObject();

            tmiJObject["_hackney_incidentid_value"] = "incidentid";
            tmiJObject.Add("hackney_name", "ticketnumber");
            tmiJObject["_hackney_subjectid_value"] = "c1f72d01-28dc-e711-8115-70106aaaaaaa";
            tmiJObject["_hackney_contactid_value"] = "463adffe-61a5-db11-882c-000000000000";
            tmiJObject.Add("hackney_estateoffice", "5");
            tmiJObject.Add("hackney_natureofenquiry", "3");
            tmiJObject.Add("hackney_enquirysubject", "100000005");
            tmiJObject.Add("hackney_tenancymanagementinteractionsid", "c1f72d01-28dc-e711-8115-70106aaaaaaa");

            Guid guid = new Guid("c1f72d01-28dc-e711-8115-70106aaaaaaa");

            var jsonStringtmi = JsonConvert.SerializeObject(tmiJObject);
            HttpResponseMessage responsMessagetmi = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonStringtmi, System.Text.Encoding.UTF8, "application/json") };
            responsMessagetmi.Headers.Add("OData-EntityId", "(" + guid + ")");

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockingApiCall.Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessagetmi);
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            TenancyManagementActions tmiActions = new TenancyManagementActions(mockILoggerAdapter.Object,  mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);


            var request = new TenancyManagement
            {
                contactId = "463adffe-61a5-db11-882c-000000000000",
                enquirySubject = "100000005",
                estateOfficerId = "284216e9-d365-e711-80f9-70106aaaaaaa",
                subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                adviceGiven = "Housing Repair Inquiry",
                estateOffice = "5",
                source = "1",
                natureofEnquiry = "3",
                officerPatchId = "be77dd44-b005-e811-811c-70106aaaaaa",
                areaName = "3",
                managerId = "ae7b4690-b005-e811-811c-70106fffffff",
                ServiceRequest = new CRMServiceRequest
                {
                    Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                    ContactId = "463adffe-61a5-db11-882c-000000000000",
                    Title = "Tenancy Management",
                    Description = "Enquiry Created By Estate Officer"
                }

            };

            var resoponseExpencted = new TenancyManagement
            {
                interactionId = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                contactId = "463adffe-61a5-db11-882c-000000000000",
                enquirySubject = "100000005",
                estateOfficerId = "284216e9-d365-e711-80f9-70106aaaaaaa",
                subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                adviceGiven = "Housing Repair Inquiry",
                estateOffice = "5",
                source = "1",
                natureofEnquiry = "3",
                officerPatchId = "be77dd44-b005-e811-811c-70106aaaaaa",
                areaName = "3",
                managerId = "ae7b4690-b005-e811-811c-70106fffffff",
                ServiceRequest = new CRMServiceRequest
                {
                    TicketNumber = "ticketnumber",
                    Id = "incidentid",
                    Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                    ContactId = "463adffe-61a5-db11-882c-000000000000",
                    Title = "Tenancy Management",
                    Description = "Enquiry Created By Estate Officer"
                }

            };

            var resultResponseRequired = await tmiActions.CreateTenancyManagementInteraction(request);

            Assert.Equal(JsonConvert.SerializeObject(resoponseExpencted), JsonConvert.SerializeObject(resultResponseRequired));
        }

        [Fact]
        public async Task create_tenancy_Management_interaction_throws_service_exception_if_the_service_responds_with_an_error()
        {
            #region ServiceRequest

         
            var response = "TokenOnValidRequest";

            var mocktmiCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mocktmiCallBuilder.Setup(x => x.CreateRequest(response)).ReturnsAsync(client);


            var serviceJObject = new JObject();

            serviceJObject.Add("description", "Enquiry Created By Estate Officer");
            serviceJObject.Add("ticketnumber", "ticketnumber");
            serviceJObject.Add("title", "Tenancy Management");
            serviceJObject["_subjectid_value"] = "c1f72d01-28dc-e711-8115-70106aaaaaaa";
            serviceJObject["_customerid_value"] = "463adffe-61a5-db11-882c-000000000000";
            serviceJObject.Add("incidentid", "incidentid");
            serviceJObject.Add("annotationid", "63a0e5b9-88df-e311-b8e5-6c3be5ccccccc");

            string jsonString = JsonConvert.SerializeObject(serviceJObject);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            var mockingApiCall = new Mock<IHackneyHousingAPICall>();
            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessage);


            #endregion ServiceRequest


            var tmiJObject = new JObject();


            tmiJObject["_hackney_incidentid_value"] = "incidentid";
            tmiJObject.Add("hackney_name", "ticketnumber");
            tmiJObject["_hackney_subjectid_value"] = "c1f72d01-28dc-e711-8115-70106aaaaaaa";
            tmiJObject["_hackney_contactid_value"] = "463adffe-61a5-db11-882c-000000000000";
            tmiJObject.Add("hackney_estateoffice", "5");
            tmiJObject.Add("hackney_natureofenquiry", "3");
            tmiJObject.Add("hackney_enquirysubject", "100000005");
            tmiJObject.Add("hackney_tenancymanagementinteractionsid", "c1f72d01-28dc-e711-8115-70106aaaaaaa");

            Guid guid = new Guid("c1f72d01-28dc-e711-8115-70106aaaaaaa");

            var jsonStringtmi = JsonConvert.SerializeObject(tmiJObject);
            HttpResponseMessage responsMessagetmi = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(jsonStringtmi, System.Text.Encoding.UTF8, "application/json") };
            responsMessagetmi.Headers.Add("OData-EntityId", "(" + guid + ")");

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockingApiCall.Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessagetmi);
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            TenancyManagementActions tmiActions = new TenancyManagementActions(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);


            var request = new TenancyManagement
            {
                contactId = "463adffe-61a5-db11-882c-000000000000",
                enquirySubject = "100000005",
                estateOfficerId = "284216e9-d365-e711-80f9-70106aaaaaaa",
                subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                adviceGiven = "Housing Repair Inquiry",
                estateOffice = "5",
                source = "1",
                natureofEnquiry = "3",
                officerPatchId = "be77dd44-b005-e811-811c-70106aaaaaa",
                areaName = "3",
                managerId = "ae7b4690-b005-e811-811c-70106fffffff",
                ServiceRequest = new CRMServiceRequest
                {
                    Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                    ContactId = "463adffe-61a5-db11-882c-000000000000",
                    Title = "Tenancy Management",
                    Description = "Enquiry Created By Estate Officer"
                }

            };

            await Assert.ThrowsAsync<TenancyInteractionServiceException>(async () => await tmiActions.CreateTenancyManagementInteraction(request));
        }
        [Fact]
        public async Task create_tenancy_Management_interaction_throws_missing_result_exception_if_the_service_responds_with_an_error()
        {
            #region ServiceRequest
 
            var response = "TokenOnValidRequest";
            
            var mocktmiCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mocktmiCallBuilder.Setup(x => x.CreateRequest(response)).ReturnsAsync(client);


            var serviceJObject = new JObject();

            serviceJObject.Add("description", "Enquiry Created By Estate Officer");
            serviceJObject.Add("ticketnumber", "ticketnumber");
            serviceJObject.Add("title", "Tenancy Management");
            serviceJObject["_subjectid_value"] = "c1f72d01-28dc-e711-8115-70106aaaaaaa";
            serviceJObject["_customerid_value"] = "463adffe-61a5-db11-882c-000000000000";
            serviceJObject.Add("incidentid", "incidentid");
            serviceJObject.Add("annotationid", "63a0e5b9-88df-e311-b8e5-6c3be5ccccccc");

            string jsonString = JsonConvert.SerializeObject(serviceJObject);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            var mockingApiCall = new Mock<IHackneyHousingAPICall>();
            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessage);


            #endregion ServiceRequest


            var tmiJObject = new JObject();

            var jsonStringtmi = JsonConvert.SerializeObject(tmiJObject);
            HttpResponseMessage responsMessagetmi = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonStringtmi, System.Text.Encoding.UTF8, "application/json") };
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            string query = HousingAPIQueryBuilder.PostTenancyManagementInteractionQuery();

            mockingApiCall.Setup(x => x.postHousingAPI(client, query, tmiJObject)).ReturnsAsync(responsMessagetmi);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            TenancyManagementActions tmiActions = new TenancyManagementActions(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            var request = new TenancyManagement();

            await Assert.ThrowsAsync<MissingTenancyInteractionRequestException>(async () =>
                await tmiActions.CreateTenancyManagementInteraction(request));



        }

        #endregion TenancyManagement Interaction

        #region ServiceRequest

        [Fact]
        public async Task create_service_request_returns_a_create_object()
        {
                                  
            var response = "TokenOnValidRequest";
                      
            var mocktmiCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mocktmiCallBuilder.Setup(x => x.CreateRequest(response)).ReturnsAsync(client);
            
            var serviceJObject = new JObject();

            serviceJObject.Add("description", "Enquiry Created By Estate Officer");
            serviceJObject.Add("ticketnumber", "ticketnumber");
            serviceJObject.Add("title", "Tenancy Management");
            serviceJObject["_subjectid_value"] = "c1f72d01-28dc-e711-8115-70106aaaaaaa";
            serviceJObject["_customerid_value"] = "463adffe-61a5-db11-882c-000000000000";
            serviceJObject.Add("incidentid", "incidentid");
            serviceJObject.Add("annotationid", "63a0e5b9-88df-e311-b8e5-6c3be5ccccccc");


            string jsonString = JsonConvert.SerializeObject(serviceJObject);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            var mockingApiCall = new Mock<IHackneyHousingAPICall>();

            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessage);
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockILoggerAdapter = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            TenancyManagementActions tmiActions = new TenancyManagementActions(mockILoggerAdapter.Object,  mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            var request = new CRMServiceRequest
            {
                Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                ContactId = "463adffe-61a5-db11-882c-000000000000",
                Title = "Tenancy Management",
                Description = "Enquiry Created By Estate Officer",
                RequestCallback = true

            };

            CRMServiceRequest resultResponseRequired = await tmiActions.CreateCrmServiceRequest(request);


            var resoponseExpencted = new CRMServiceRequest
            {
                TicketNumber = "ticketnumber",
                Id = "incidentid",
                Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                ContactId = "463adffe-61a5-db11-882c-000000000000",
                Title = "Tenancy Management",
                Description = "Enquiry Created By Estate Officer",
                RequestCallback = true

            };

            Assert.Equal(JsonConvert.SerializeObject(resoponseExpencted), JsonConvert.SerializeObject(resultResponseRequired));

        }

        [Fact]
        public async Task create_service_request_throws_service_exception_if_the_service_responds_with_an_error()
        {

            var response = "TokenOnValidRequest";

            var mocktmiCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mocktmiCallBuilder.Setup(x => x.CreateRequest(response)).ReturnsAsync(client);



            var serviceJObject = new JObject();

            serviceJObject.Add("description", "Enquiry Created By Estate Officer");
            serviceJObject.Add("ticketnumber", "ticketnumber");
            serviceJObject.Add("title", "Tenancy Management");
            serviceJObject["_subjectid_value"] = "c1f72d01-28dc-e711-8115-70106aaaaaaa";
            serviceJObject["_customerid_value"] = "463adffe-61a5-db11-882c-000000000000";
            serviceJObject.Add("incidentid", "incidentid");
            serviceJObject.Add("annotationid", "63a0e5b9-88df-e311-b8e5-6c3be5ccccccc");


            string jsonString = JsonConvert.SerializeObject(serviceJObject);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            var mockingApiCall = new Mock<IHackneyHousingAPICall>();

            mockingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockILoggerAdapter = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            TenancyManagementActions tmiActions = new TenancyManagementActions(mockILoggerAdapter.Object,  mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            var request = new CRMServiceRequest
            {
                Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                ContactId = "463adffe-61a5-db11-882c-000000000000",
                Title = "Tenancy Management",
                Description = "Enquiry Created By Estate Officer",
                RequestCallback = true

            };


            await Assert.ThrowsAsync<ServiceRequestException>(async () => await tmiActions.CreateCrmServiceRequest(request));

        }
        [Fact]
        public async Task create_service_request_throws_missing_result_exception_if_the_service_responds_with_an_error()
        {

                      
            var response = "TokenOnValidRequest";

            var mocktmiCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mocktmiCallBuilder.Setup(x => x.CreateRequest(response)).ReturnsAsync(client);

            var serviceJObject = new JObject();
            string query = HousingAPIQueryBuilder.PostIncidentQuery();

            string jsonString = JsonConvert.SerializeObject(serviceJObject);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            var mockingApiCall = new Mock<IHackneyHousingAPICall>();

            mockingApiCall.Setup(x => x.SendAsJsonAsync(client, HttpMethod.Post, query, serviceJObject)).ReturnsAsync(responsMessage);
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockILoggerAdapter = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            TenancyManagementActions tmiActions = new TenancyManagementActions(mockILoggerAdapter.Object, mocktmiCallBuilder.Object, mockingApiCall.Object, mockAccessToken.Object,mockConfig.Object);

            var request = new CRMServiceRequest();

            await Assert.ThrowsAsync<MissingServiceRequestException>(async () => await tmiActions.CreateCrmServiceRequest(request));

        }

        #endregion ServiceRequest

        #region UpdateTenancy
        [Fact]
        public async Task update_tenancy_Managementinteraction_when_closing_incident_returns_update_response()
        {
         
            var response = "TokenOnValidRequest";

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();

            TenancyManagement updateRequestObject = new TenancyManagement
            {
                interactionId = "2a7912b3-b6e0-e711-810e-70106bbbbbbb",
                adviceGiven = "update Tenancy Management Service Request",
                ServiceRequest = new CRMServiceRequest
                {
                    Id = "fbb201a1-b6e0-e711-810a-e0071b7fe041",
                    Description = "update Tenancy Management Service Request",
                    RequestCallback = false
                },
                estateOfficerName = "User 1",
                estateOfficerId = "estate Officer Id",
                status = 0,
                processStage = 2
            };

            //first add for annotation
            string requestUrl = "api/data/v8.2/annotations?$select=annotationid";
            var annotationDictionary = new Dictionary<string, string>();
            annotationDictionary.Add("annotationid", "63a0e5b9-88df-e311-b8e5-6c3be5ccccccc");
            string jsonString = JsonConvert.SerializeObject(annotationDictionary);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.SendAsJsonAsync(client, HttpMethod.Post, requestUrl, It.IsAny<JObject>())).ReturnsAsync(responsMessage);
            //2nd update for intraction
            JObject tenancyInteraction = new JObject();
            tenancyInteraction.Add("statuscode", "1");
            tenancyInteraction.Add("statecode", "2");
            tenancyInteraction.Add("housing_advicegiven", updateRequestObject.adviceGiven);

            string interactionQuery = HousingAPIQueryBuilder.updateInteractionQuery(updateRequestObject.interactionId);
            mockApiCall.Setup(x => x.UpdateObject(client, It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(true);

            HttpResponseMessage callClosureMsg = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };
            mockApiCall.Setup(x => x.SendAsJsonAsync(client, HttpMethod.Post, requestUrl, It.IsAny<JObject>())).ReturnsAsync(callClosureMsg);
            var mockActionlogger = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            var TenancyManagementActions = new TenancyManagementActions(mockActionlogger.Object,  mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            //3rd update for service Request

            var updateResult = new
            {
                annotationId = "63a0e5b9-88df-e311-b8e5-6c3be5ccccccc",
                serviceRequestId = "fbb201a1-b6e0-e711-810a-e0071b7fe041",
                interactionId = "2a7912b3-b6e0-e711-810e-70106bbbbbbb",
                Description = "update Tenancy Management Service Request",
                status = "Closed",
                requestCallBack = false,
                processStage = 2

            };

            var json = new
            {
                result = updateResult
            };

            var responsedata = await TenancyManagementActions.UpdateTenancyManagementInteraction(updateRequestObject);
            Assert.Equal(JsonConvert.SerializeObject(responsedata), JsonConvert.SerializeObject(json));
        }

        [Fact]
        public async Task update_tenancy_Managementinteraction_returns_update_response()
        { 
          
            var response = "TokenOnValidRequest";
                    
            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();

            TenancyManagement updateRequestObject = new TenancyManagement
            {
                interactionId = "2a7912b3-b6e0-e711-810e-70106bbbbbbb",
                adviceGiven = "update Tenancy Management Service Request",
                ServiceRequest = new CRMServiceRequest
                {
                    Id = "fbb201a1-b6e0-e711-810a-e0071b7fe041",
                    Description = "update Tenancy Management Service Request",
                    RequestCallback = true
                },
                estateOfficerName = "User 1",
                estateOfficerId = "estate Officer Id",
                status = 1,
            };

            //first add for annotation
            string requestUrl = "api/data/v8.2/annotations?$select=annotationid";
            var annotationDictionary = new Dictionary<string, string>();
            annotationDictionary.Add("annotationid", "63a0e5b9-88df-e311-b8e5-6c3be5ccccccc");
            string jsonString = JsonConvert.SerializeObject(annotationDictionary);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.SendAsJsonAsync(client, HttpMethod.Post, requestUrl, It.IsAny<JObject>())).ReturnsAsync(responsMessage);
            //2nd update for intraction
          
            mockApiCall.Setup(x => x.UpdateObject(client, It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(true);
            
            var mockActionlogger = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            var TenancyManagementActions = new TenancyManagementActions(mockActionlogger.Object,  mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object,mockConfig.Object);

            //3rd update for service Request

            var updateResult = new
            {
                annotationId = "63a0e5b9-88df-e311-b8e5-6c3be5ccccccc",
                serviceRequestId = "fbb201a1-b6e0-e711-810a-e0071b7fe041",
                interactionId = "2a7912b3-b6e0-e711-810e-70106bbbbbbb",
                Description = "update Tenancy Management Service Request",
                status = "InProgress",
                requestCallBack = true,
                processStage = 0

            };

            var json = new
            {
                result = updateResult
            };

            var responsedata = await TenancyManagementActions.UpdateTenancyManagementInteraction(updateRequestObject);
            Assert.Equal(JsonConvert.SerializeObject(responsedata), JsonConvert.SerializeObject(json));
        }

        [Fact]
        public async Task update_tenancy_raises_exception_if_annotation_not_created()
        {
            TenancyManagement requestObject = new TenancyManagement
            {
                interactionId = "2a7912b3-b6e0-e711-810e-70106bbbbbbb",
                adviceGiven = "update Tenancy Management Service Request",
                ServiceRequest = new CRMServiceRequest
                {
                    Id = "fbb201a1-b6e0-e711-810a-e0071b7fe041",
                    Description = "update Tenancy Management Service Request"
                },
                estateOfficerName = "User 1",
                estateOfficerId = "estate officer id",
                status = 1
            };

         
            var response = "TokenOnValidRequest";

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            string requestUrl = "api/data/v8.2/annotations?$select=annotationid";

            mockApiCall.Setup(x => x.SendAsJsonAsync(client, HttpMethod.Post, requestUrl, It.IsAny<JObject>())).ReturnsAsync((HttpResponseMessage)null);

            var mockActionlogger = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            var TenancyManagementActions = new TenancyManagementActions(mockActionlogger.Object,mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<NullResponseException>(async () => await TenancyManagementActions.UpdateTenancyManagementInteraction(requestObject));
        }
        [Fact]
        public async Task update_tenancy_raises_exception_if_service_responds_with_error()
        {

            TenancyManagement requestObject = new TenancyManagement
            {
                interactionId = "2a7912b3-b6e0-e711-810e-70106bbbbbbb",
                adviceGiven = "update Tenancy Management Service Request",
                ServiceRequest = new CRMServiceRequest
                {
                    Id = "fbb201a1-b6e0-e711-810a-e0071b7fe041",
                    Description = "update Tenancy Management Service Requestn"
                },
                estateOfficerName = "User 1",
                estateOfficerId = "estate officer id",
                status = 1
            };

          
            var response = "TokenOnValidRequest";
            
            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var annotationDictionary = new Dictionary<string, string>();
            annotationDictionary.Add("annotationid", "63a0e5b9-88df-e311-b8e5-6c3be5ccccccc");
            string jsonString = JsonConvert.SerializeObject(annotationDictionary);
            string requestUrl = "api/data/v8.2/annotations?$select=annotationid";
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.SendAsJsonAsync(client, HttpMethod.Post, requestUrl, It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            var mockActionlogger = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            mockApiCall.Setup(x => x.UpdateObject(client, It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(false);
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            var TenancyManagementActions = new TenancyManagementActions(mockActionlogger.Object, mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object,mockConfig.Object);

            await Assert.ThrowsAsync<TenancyServiceException>(async () => await TenancyManagementActions.UpdateTenancyManagementInteraction(requestObject));
        }

        [Fact]
        public async Task update_tenancy_raises_exception_if_service_responds_with_error_when_creating_annotation()
        {

            TenancyManagement requestObject = new TenancyManagement
            {
                interactionId = "2a7912b3-b6e0-e711-810e-70106bbbbbbb",
                adviceGiven = "update Tenancy Management Service Request",
                ServiceRequest = new CRMServiceRequest
                {
                    Id = "fbb201a1-b6e0-e711-810a-e0071b7fe041",
                    Description = "update Tenancy Management Service Requestn",
                    RequestCallback = false
                },
                estateOfficerName = "User 1",
                estateOfficerId = "estate officer id",
                status = 1
            };

          
            var response = "TokenOnValidRequest";

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var annotationDictionary = new Dictionary<string, string>();
            annotationDictionary.Add("annotationid", "63a0e5b9-88df-e311-b8e5-6c3be5ccccccc");
            string jsonString = JsonConvert.SerializeObject(annotationDictionary);
            string requestUrl = "api/data/v8.2/annotations?$select=annotationid";
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };

            mockApiCall.Setup(x => x.SendAsJsonAsync(client, HttpMethod.Post, requestUrl, It.IsAny<JObject>()))
                .ReturnsAsync(responsMessage);

            var mockActionlogger = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            mockApiCall.Setup(x => x.UpdateObject(client, It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(false);
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            var TenancyManagementActions = new TenancyManagementActions(mockActionlogger.Object,
             mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object,mockConfig.Object);

            await Assert.ThrowsAsync<TenancyServiceException>(async () =>
                await TenancyManagementActions.UpdateTenancyManagementInteraction(requestObject));
        }

        #endregion

            #region GetTenancy
        [Fact]
        public async Task get_tenancy_incident_details_based_on_id_and_user_type()
        {
           
            var response = "TokenOnValidRequest";

        
            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var query = HousingAPIQueryBuilder.getTenancyInteractionDeatils("463adffe-61a5-db11-882c-000000000000", "contact");
            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, "463adffe-61a5-db11-882c-000000000000")).ReturnsAsync(getTenantIncidentDetail());

            var mockActionlogger = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            var TenancyManagementActions = new TenancyManagementActions(mockActionlogger.Object,  mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            var responsedata = await TenancyManagementActions.GetTenancyManagementInteraction("463adffe-61a5-db11-882c-000000000000", "contact");

            object tenancy = getTenantIncidentDetailActionResult();

            Assert.Equal(JsonConvert.SerializeObject(tenancy), JsonConvert.SerializeObject(responsedata));
        }

        [Fact]
        public async Task get_tenancy_incidents_raises_exception_if_service_responds_with_error()
        {
    
            var response = "TokenOnValidRequest";

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var query = HousingAPIQueryBuilder.getTenancyInteractionDeatils("463adffe-61a5-db11-882c-000000000000", "contact");

            HttpResponseMessage serviceResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("", System.Text.Encoding.UTF8, "application/json") };
            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, "463adffe-61a5-db11-882c-000000000000")).ReturnsAsync(serviceResponse);

            var mockActionlogger = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            var TenancyManagementActions = new TenancyManagementActions(mockActionlogger.Object, mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<TenancyServiceException>(async () => await TenancyManagementActions.GetTenancyManagementInteraction("463adffe-61a5-db11-882c-000000000000", "contact"));

        }

        [Fact]
        public async Task get_tenancy_incidents_raises_exception_if_service_responds_with_null_result()
        {
            
            var response = "TokenOnValidRequest";

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var query = HousingAPIQueryBuilder.getTenancyInteractionDeatils("463adffe-61a5-db11-882c-000000000000", "contact");


            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, "463adffe-61a5-db11-882c-000000000000")).ReturnsAsync((HttpResponseMessage)null);

            var mockActionlogger = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            var TenancyManagementActions = new TenancyManagementActions(mockActionlogger.Object,  mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<NullResponseException>(async () => await TenancyManagementActions.GetTenancyManagementInteraction("463adffe-61a5-db11-882c-000000000000", "contact"));

        }
        private HttpResponseMessage getTenantIncidentDetail()
        {
            var TenancyManagement = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();

            value["hackney_name"] = "CAS-00059-000000";
            value["hackney_transferred"] = true;
            value["statecode"] = 0;
            value["contact3_x002e_telephone1"] = "123";
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
            value["hackney_handleby@OData.Community.Display.V1.FormattedValue"] = "Estate Officer";
            value["incident1_x002e_housing_requestcallback"] = false;
            value["_hackney_contactid_value"] = "ContactId9486954o93";
            value["_hackney_contactid_value@OData.Community.Display.V1.FormattedValue"] = "Contact name";
            value["contact3_x002e_hackney_title"] = "Contact Title";
            value["contact3_x002e_address1_postalcode"] = "E8 2HH";
            value["contact3_x002e_address1_line1"] = "Maurice Bishop House";
            value["contact3_x002e_address1_line2"] = "Hackney";
            value["contact3_x002e_address1_line3"] = null;
            value["contact3_x002e_address1_city"] = "London";
            value["contact3_x002e_birthdate"] = "01/01/1950";
            value["contact3_x002e_emailaddress1"] = "test@test.com";
            value["contact3_x002e_hackney_cautionaryalert"] = false;
            value["contact3_x002e_hackney_propertycautionaryalert"] = false;
            value["contact3_x002e_hackney_larn"] = "LARN834210";
            value["accountCreatedOn"] = null;
            value["hackney_parent_interactionidname"] = "parentInteractionId01";
        
            listJObject.Add(value);
            TenancyManagement.Add("value", listJObject);
            string jsonString = JsonConvert.SerializeObject(TenancyManagement);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };
            return response;
        }

        private object getTenantIncidentDetailActionResult()
        {
            var tenancyList = new List<dynamic>();
            dynamic tenancyObj = new ExpandoObject();

            tenancyObj.incidentId = "39141263-b0e0-e711-810a-e0071bbbbbbb";
            tenancyObj.isTransferred = true;
            tenancyObj.ticketNumber = "CAS-00059-000000";
            tenancyObj.stateCode = 0;
            tenancyObj.processStage = null;
            tenancyObj.nccOfficersId = "284216e9-d365-e711-80f9-70106aaaaaaa";
            tenancyObj.nccEstateOfficer = "Test Test";
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
            tenancyObj.handledBy = "Estate Officer";
            tenancyObj.requestCallBack = false;
            tenancyObj.contactId = "ContactId9486954o93";
            tenancyObj.contactName = "Contact name";
            tenancyObj.contactTitle = "Contact Title";
            tenancyObj.contactPostcode = "E8 2HH";
            tenancyObj.contactAddressLine1 = "Maurice Bishop House";
            tenancyObj.contactAddressLine2 = "Hackney";
            tenancyObj.contactAddressLine3 = null;
            tenancyObj.contactAddressCity = "London";
            tenancyObj.contactBirthDate = "01/01/1950";
            tenancyObj.contactTelephone = "123";
            tenancyObj.contactEmailAddress = "test@test.com";
            tenancyObj.contactCautionaryAlert = false;
            tenancyObj.contactPropertyCautionaryAlert = false;
           
            tenancyObj.contactLarn = "LARN834210";
            tenancyObj.contactUPRN = null;
            tenancyObj.householdID = null;
            tenancyObj.accountCreatedOn = null;
            tenancyObj.parentInteractionId = "parentInteractionId01";
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


        #region Get Group Tray
        [Fact]
        public async Task get_group_tray_details_return_valid_object()
        {

            var response = "TokenOnValidRequest";

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);


            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var query = HousingAPIQueryBuilder.getAreaTrayInteractions("1");
            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, "1")).ReturnsAsync(groupTrayResponseDetail());

            var mockActionlogger = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            var TenancyManagementActions = new TenancyManagementActions(mockActionlogger.Object, mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object,mockConfig.Object);

            var responsedata = await TenancyManagementActions.GetAreaTrayInteractions("1");

            object tenancy = groupTrayObjectDetail();

            Assert.Equal(JsonConvert.SerializeObject(tenancy), JsonConvert.SerializeObject(responsedata));
        }

        [Fact]
        public async Task get_group_tray_details_raises_exception_if_service_responds_with_error()
        {
          
            var response = "TokenOnValidRequest";

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var query = HousingAPIQueryBuilder.getAreaTrayInteractions("1");

            HttpResponseMessage serviceResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("", System.Text.Encoding.UTF8, "application/json") };
            mockApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), query, "1")).ReturnsAsync(serviceResponse);

            var mockActionlogger = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            var TenancyManagementActions = new TenancyManagementActions(mockActionlogger.Object, mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<TenancyServiceException>(async () => await TenancyManagementActions.GetAreaTrayInteractions("1"));

        }

        [Fact]
        public async Task get_group_tray_details_raises_exception_if_service_responds_with_null_result()
        {
           
            var response = "TokenOnValidRequest";

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var query = HousingAPIQueryBuilder.getAreaTrayInteractions("100");


            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, "100")).ReturnsAsync((HttpResponseMessage)null);

            var mockActionlogger = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            var TenancyManagementActions = new TenancyManagementActions(mockActionlogger.Object, mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object,mockConfig.Object);

            await Assert.ThrowsAsync<NullResponseException>(async () => await TenancyManagementActions.GetAreaTrayInteractions("100"));

        }

        private HttpResponseMessage groupTrayResponseDetail()
        {
            var TenancyManagement = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["hackney_name"] = "CAS-00059-000000";
            value["hackney_transferred"] = false;
            value["statecode"] = 0;
            value["contact3_x002e_telephone1"] = "123";
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
            value["hackney_handleby@OData.Community.Display.V1.FormattedValue"] = "Estate Officer";
            value["incident1_x002e_housing_requestcallback"] = false;
            value["_hackney_contactid_value"] = "ContactId9486954o93";
            value["_hackney_contactid_value@OData.Community.Display.V1.FormattedValue"] = "Contact name";
            value["contact3_x002e_address1_postalcode"] = "E8 2HH";
            value["contact3_x002e_address1_line1"] = "Maurice Bishop House";
            value["contact3_x002e_address1_line2"] = "Hackney";
            value["contact3_x002e_address1_line3"] = null;
            value["contact3_x002e_address1_city"] = "London";
            value["contact3_x002e_birthdate"] = "01/01/1950";
            value["contact3_x002e_emailaddress1"] = "test@test.com";
            value["contact3_x002e_hackney_larn"] = "LARN834210";

            listJObject.Add(value);
            TenancyManagement.Add("value", listJObject);
            string jsonString = JsonConvert.SerializeObject(TenancyManagement);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };
            return response;
        }

        private object groupTrayObjectDetail()
        {
            var tenancyList = new List<dynamic>();
            dynamic tenancyObj = new ExpandoObject();

            tenancyObj.incidentId = "39141263-b0e0-e711-810a-e0071bbbbbbb";
            tenancyObj.isTransferred = false;
            tenancyObj.ticketNumber = "CAS-00059-000000";
            tenancyObj.stateCode = 0;
            tenancyObj.processStage = null;
            tenancyObj.nccOfficersId = "284216e9-d365-e711-80f9-70106aaaaaaa";
            tenancyObj.nccEstateOfficer = "Test Test";
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
            tenancyObj.handledBy = "Estate Officer";
            tenancyObj.requestCallBack = false;
            tenancyObj.contactId = "ContactId9486954o93";
            tenancyObj.contactName = "Contact name";
            tenancyObj.contactPostcode = "E8 2HH";
            tenancyObj.contactAddressLine1 = "Maurice Bishop House";
            tenancyObj.contactAddressLine2 = "Hackney";
            tenancyObj.contactAddressLine3 = null;
            tenancyObj.contactAddressCity = "London";
            tenancyObj.contactBirthDate = "01/01/1950";
            tenancyObj.contactTelephone = "123";
            tenancyObj.contactEmailAddress = "test@test.com";
            tenancyObj.contactLarn = "LARN834210";
            tenancyObj.contactUPRN = null;
            tenancyObj.householdID = null;
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

        #region CallTransfer
        [Fact]
        public async Task call_transfer_returns_update_response_on_successful_call_transfer_to_patch()
        {
            var transferCallRequestObject = new TenancyManagement
            {
                contactId = "463adffe-61a5-db11-882c-000000000000",
                enquirySubject = "100000005",
                estateOfficerId = "284216e9-d365-e711-80f9-70106aaaaaaa",
                subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                adviceGiven = "Housing Repair Inquiry",
                estateOffice = "5",
                source = "1",
                natureofEnquiry = "3",
                officerPatchId = "be77dd44-b005-e811-811c-70106aaaaaa",
                areaName = "3",
                managerId = "ae7b4690-b005-e811-811c-70106fffffff",
                assignedToManager = true,
                interactionId = "1f46ea18-fe0b-e811-8116-70106faa6a31",
                ServiceRequest = new CRMServiceRequest
                {
                    Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                    ContactId = "463adffe-61a5-db11-882c-000000000000",
                    Title = "Tenancy Management",
                    Description = "Enquiry Created By Estate Officer",
                    Id = "incidentid"
                }
            };

         
            var response = "TokenOnValidRequest";
                    
            var mocktmiCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mocktmiCallBuilder.Setup(x => x.CreateRequest(response)).ReturnsAsync(client);

            var tmiJObject = new JObject();

            var mockApiCall = new Mock<IHackneyHousingAPICall>();

            dynamic transferCallResponseObject = new ExpandoObject();
            transferCallResponseObject.interactionId = "1f46ea18-fe0b-e811-8116-70106faa6a31";
            transferCallResponseObject.contactId = "463adffe-61a5-db11-882c-000000000000";
            transferCallResponseObject.enquirySubject = "100000005";
            transferCallResponseObject.estateOfficerId = "284216e9-d365-e711-80f9-70106aaaaaaa";
            transferCallResponseObject.subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa";
            transferCallResponseObject.adviceGiven = "Housing Repair Inquiry";
            transferCallResponseObject.estateOffice = "5";
            transferCallResponseObject.source = "1";
            transferCallResponseObject.natureofEnquiry = "3";
            transferCallResponseObject.estateOfficerName = null;
            transferCallResponseObject.officerPatchId = "be77dd44-b005-e811-811c-70106aaaaaa";
            transferCallResponseObject.areaName = "3";
            transferCallResponseObject.managerId = null;
            transferCallResponseObject.assignedToPatch = false;
            transferCallResponseObject.assignedToManager = false;
            transferCallResponseObject.transferred = false;
            transferCallResponseObject.ServiceRequest = new CRMServiceRequest
            {
                Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                ContactId = "463adffe-61a5-db11-882c-000000000000",
                Title = "Tenancy Management",
                Description = "Enquiry Created By Estate Officer",
                Id = "incidentid"
            };
            transferCallResponseObject.status = 0;
            transferCallResponseObject.parentInteractionId = null;
            transferCallResponseObject.processType = null;
            transferCallResponseObject.householdId = null;
            transferCallResponseObject.processStage = 0;
            transferCallResponseObject.reasonForStartingProcess = null;
            transferCallResponseObject.annotationid = "annotationId";
          
            var method = new HttpMethod("PATCH");
            var annotationDictionary = new Dictionary<string, string>();
            annotationDictionary.Add("annotationid", "annotationId");
            string jsonString = JsonConvert.SerializeObject(annotationDictionary);
            string disassociateManagerFromInteractionQuery =
                HousingAPIQueryBuilder.deleteAssociationOfManagerWithInteraction(
                    transferCallRequestObject.interactionId, transferCallRequestObject.managerId,
                    client.BaseAddress.ToString());
            HttpResponseMessage responsMessageOfDeleteCall = new HttpResponseMessage(HttpStatusCode.NoContent) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.deleteObjectAPIResponse(client, disassociateManagerFromInteractionQuery))
                .ReturnsAsync(responsMessageOfDeleteCall);

            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };
            string interactionQuery = HousingAPIQueryBuilder.updateInteractionQuery(transferCallRequestObject.interactionId);
            mockApiCall.Setup(x => x.SendAsJsonAsync(client, It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            var mockActionlogger = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            var TenancyManagementActions = new TenancyManagementActions(mockActionlogger.Object,  mocktmiCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object,mockConfig.Object);

            var responsedata = await TenancyManagementActions.TransferCallToAreaAndPatch(transferCallRequestObject);
            //since we are returning the same request object as response when the call is successfully transferred
            Assert.Equal(JsonConvert.SerializeObject(responsedata), JsonConvert.SerializeObject(transferCallResponseObject));
        }

        [Fact]
        public async Task call_transfer_throws_exception_if_service_responds_with_null_response()
        {
            var transferCallRequestObject = new TenancyManagement
            {
                contactId = "463adffe-61a5-db11-882c-000000000000",
                enquirySubject = "100000005",
                estateOfficerId = "284216e9-d365-e711-80f9-70106aaaaaaa",
                subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                adviceGiven = "Housing Repair Inquiry",
                estateOffice = "5",
                source = "1",
                natureofEnquiry = "3",
                officerPatchId = "be77dd44-b005-e811-811c-70106aaaaaa",
                areaName = "3",
                managerId = "ae7b4690-b005-e811-811c-70106fffffff",
                interactionId = "1f46ea18-fe0b-e811-8116-70106faa6a31",
                ServiceRequest = new CRMServiceRequest
                {
                    Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                    ContactId = "463adffe-61a5-db11-882c-000000000000",
                    Title = "Tenancy Management",
                    Description = "Enquiry Created By Estate Officer",
                    Id = "incidentid"
                }
            };

       
            var response = "TokenOnValidRequest";

            var mocktmiCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mocktmiCallBuilder.Setup(x => x.CreateRequest(response)).ReturnsAsync(client);

            var tmiJObject = new JObject();

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            string disassociateManagerFromInteractionQuery =
                HousingAPIQueryBuilder.deleteAssociationOfManagerWithInteraction(
                    transferCallRequestObject.interactionId, transferCallRequestObject.managerId,
                    client.BaseAddress.ToString());
            HttpResponseMessage responsMessageOfDeleteCall = new HttpResponseMessage(HttpStatusCode.NoContent) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.deleteObjectAPIResponse(client, disassociateManagerFromInteractionQuery))
                .ReturnsAsync(responsMessageOfDeleteCall);
            string interactionQuery = HousingAPIQueryBuilder.updateInteractionQuery(transferCallRequestObject.interactionId);
            mockApiCall.Setup(x => x.SendAsJsonAsync(client, It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync((HttpResponseMessage)null);

            var mockActionlogger = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockAccessTokenn = new Mock<IHackneyGetCRM365Token>();
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            var TenancyManagementActions = new TenancyManagementActions(mockActionlogger.Object,mocktmiCallBuilder.Object, mockApiCall.Object, mockAccessTokenn.Object,mockConfig.Object);

            await Assert.ThrowsAsync<NullResponseException>(async () =>
              await TenancyManagementActions.TransferCallToAreaAndPatch(transferCallRequestObject));
        }


        [Fact]
        public async Task call_transfer_throws_exception_if_service_responds_with_exception()
        {
            var transferCallRequestObject = new TenancyManagement
            {
                contactId = "463adffe-61a5-db11-882c-000000000000",
                enquirySubject = "100000005",
                estateOfficerId = "284216e9-d365-e711-80f9-70106aaaaaaa",
                subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                adviceGiven = "Housing Repair Inquiry",
                estateOffice = "5",
                source = "1",
                natureofEnquiry = "3",
                officerPatchId = "be77dd44-b005-e811-811c-70106aaaaaa",
                areaName = "3",
                managerId = "ae7b4690-b005-e811-811c-70106fffffff",
                interactionId = "1f46ea18-fe0b-e811-8116-70106faa6a31",
                ServiceRequest = new CRMServiceRequest
                {
                    Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                    ContactId = "463adffe-61a5-db11-882c-000000000000",
                    Title = "Tenancy Management",
                    Description = "Enquiry Created By Estate Officer",
                    Id = "incidentid"
                }
            };

       
            var response = "TokenOnValidRequest";

            var mocktmiCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mocktmiCallBuilder.Setup(x => x.CreateRequest(response)).ReturnsAsync(client);

            var tmiJObject = new JObject();
            string jsonString = JsonConvert.SerializeObject(tmiJObject);
            var mockApiCall = new Mock<IHackneyHousingAPICall>();

            string disassociateManagerFromInteractionQuery =
                HousingAPIQueryBuilder.deleteAssociationOfManagerWithInteraction(
                    transferCallRequestObject.interactionId, transferCallRequestObject.managerId,
                    client.BaseAddress.ToString());
            HttpResponseMessage responsMessageOfDeleteCall = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.deleteObjectAPIResponse(client, disassociateManagerFromInteractionQuery))
                .ReturnsAsync(responsMessageOfDeleteCall);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };
            string interactionQuery = HousingAPIQueryBuilder.updateInteractionQuery(transferCallRequestObject.interactionId);
            mockApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            var mockActionlogger = new Mock<ILoggerAdapter<TenancyManagementActions>>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            var mockConfig = new Mock<IOptions<AppConfiguration>>();
            var TenancyManagementActions = new TenancyManagementActions(mockActionlogger.Object,  mocktmiCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object,mockConfig.Object);

            await Assert.ThrowsAsync<TenancyServiceException>(async () =>
              await TenancyManagementActions.TransferCallToAreaAndPatch(transferCallRequestObject));
        }
        #endregion
    }
}

