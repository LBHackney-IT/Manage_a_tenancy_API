using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Services.Housing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using System.Globalization;
using System.IO;
using ManageATenancyAPI;
using ManageATenancyAPI.Extension;
using Microsoft.Extensions.DependencyInjection;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Services;
using Microsoft.Extensions.Configuration;

namespace ManageATenancyAPI.Tests.Integration
{

    public class HousingIntegrationTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly HackneyHousingAPICallBuilder _hackneyLeaseAccountApiCallBuilder;

        public HousingIntegrationTests()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json")
                    .Build()
                )
                .UseStartup<TestStartup>());

            _client = _server.CreateClient();
        }
        
        #region TransactionsIntegrationTests

        //GetTransactions API Endpoint Integration Tests
        [Fact]
        public async Task return_a_200_get_transactions_result()
        {
            var result = _client.GetAsync("v1/transactions?tagReference=000000/01").Result;
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_404_get_transactions_result()
        {
            var result = _client.GetAsync("Transaction?tagReference=000000/01").Result;
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

        }

        [Fact]
        public async Task return_a_400_get_transactions_result()
        {
            var result = _client.GetAsync("v1/transactions?tagReferenc=000000/01").Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

        }
        [Fact]
        public async Task return_a_400_get_transactions_result_for_empty_parameter()
        {
            var result = _client.GetAsync("v1/transactions?tagReference=").Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

        }
        [Fact]
        public async Task return_a_400_get_transactions_result_for_no_parameter()
        {
            var result = _client.GetAsync("v1/transactions").Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

        }

        [Fact]
        public async Task return_a_400_get_transactions_result_for_invalid_tagReference()
        {
            var result = _client.GetAsync("v1/transactions?tagReference=0000150").Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

        }

        [Fact]
        public async Task return_a_400_get_transactions_result_for_invalid_tagReference_due_to_tag_reference_being_longer_than_11_characters()
        {
            var result = _client.GetAsync("v1/transactions?tagReference=010010000/01").Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

        }

        [Fact]
        public async Task return_a_500_get_transactions_result()
        {
            var result = _client.GetAsync("v1/transactions?tagReference=123456/78").Result;
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public async Task return_a_json_object_for_valid_get_transactions_request()
        {
            var result = _client.GetAsync("v1/transactions?tagReference=000000/01").Result;
            string result_string = await result.Content.ReadAsStringAsync();
            //var result_string = result.Content.ReadAsStringAsync().Result;
            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"results\":");
            json.Append("[");
            json.Append("{");
            json.Append("\"tagReference\":\"000000/01\",");
            json.Append("\"propertyReference\":\"00013000\",");
            json.Append("\"transactionSid\":\"155000000\",");
            json.Append("\"houseReference\":\"000015\",");
            json.Append("\"transactionType\":\"RHB\",");
            json.Append("\"postDate\":\"2016-10-08T23:00:00Z\",");
            json.Append("\"realValue\":-121.16,");
            json.Append("\"transactionID\":\"1656129d-0a4a-e711-80f7-11111111111\",");
            json.Append("\"debDesc\":\"\"");
            json.Append("}");
            json.Append("]");
            json.Append("}");
            Assert.Equal(json.ToString(), result_string);
        }

        #endregion

        #region Create Contact
        [Fact]        
        public async Task return_a_200_create_contact_result()
        {
            var request = new Contact
            {
                LastName = "test last name",
                FirstName = "test first name",
                Email = "test email",
                Address1 = "maurice bishop house",
                City = "london",
                Telephone1 = "0987654321",
                HousingId = "12452",
                CreatedByOfficer = "de98e4b6-15dc-e711-8115-11111111",
                PostCode = "e81hh"

            };
            string jsonString = JsonConvert.SerializeObject(request);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = await _client.PostAsync("v1/contacts", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_404_create_contact_result()
        {
            var result = await _client.PostAsync("v1/contact", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task return_400_create_contact_result()
        {
            var request = new object();
            string jsonString = JsonConvert.SerializeObject(request);
            var result = await _client.PostAsync("v1/contacts", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task return_a_500_create_account_result()
        {
            var request = new Contact
            {
                LastName = "test last name",
                FirstName = "internal error",
                Email = "test email",
                Address1 = "maurice bishop house",
                City = "london",
                Telephone1 = "0987654321",
                HousingId = "12452",
                CreatedByOfficer = "de98e4b6-15dc-e711-8115-11111111",
                PostCode = "e81hh"
            };
            string jsonString = JsonConvert.SerializeObject(request);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var result = await _client.PostAsync("v1/contacts", new StringContent(jsonString, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        #endregion

        #region Update Contact
        [Fact]
        public async Task return_a_201_update_contact_result()
        {
            var request = new Contact
            {
                LastName = "test last name",
                FirstName = "test update contact name",
                Email = "test email",
                UpdatedByOfficer = "e64fee7c-2bba-e711-8106-1111111111"
            };
            string jsonString = JsonConvert.SerializeObject(request);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = await _client.PutAsync("v1/contacts?contactId=10496cc2-97e5-e711-8110-70106faaf8c110496cc2-97e5-e711-8110-70105bbbbbbb", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_404_update_contact_result()
        {
            var result = await _client.PutAsync("v1/contact", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task return_400_update_contact_result()
        {
            var request = new object();
            string jsonString = JsonConvert.SerializeObject(request);
            var result = await _client.PutAsync("v1/contacts?contactId=10496cc2-97e5-e711-8110-70106faaf8c110496cc2-97e5-e711-8110-70105bbbbbbb", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task return_a_500_update_account_result()
        {
            var request = new Contact
            {
                LastName = "test last name",
                FirstName = "internal error",
                Email = "test email",
                UpdatedByOfficer = "e64fee7c-2bba-e711-8106-1111111111"
            };
            string jsonString = JsonConvert.SerializeObject(request);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var result = await _client.PutAsync("v1/contacts?contactId=10496cc2-97e5-e711-8110-70106faaf8c110496cc2-97e5-e711-8110-70105bbbbbbb", new StringContent(jsonString, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        #endregion

        #region TenancyManagement Create

        [Fact]
        public async Task return_a_201_create_service__request_result()
        {
            var request = new CRMServiceRequest
            {
                Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                ContactId = "463adffe-61a5-db11-882c-000000000000",
                Title = "Tenancy Management",
                Description = "Enquiry Created By Estate Officer",
                RequestCallback = true

            };
            string jsonString = JsonConvert.SerializeObject(request);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = await _client.PostAsync("v1/TenancyManagementinteractions/servicerequest", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_404_create__service__request_result()
        {
            var result = await _client.PostAsync("v1/TenancyManagementinteraction/servicerequest", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task return_400_create__service__request_result()
        {
            var request = new CRMServiceRequest
            {
                Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                ContactId = "463adffe-61a5-db11-882c-000000000000",
                Description = "Enquiry Created By Estate Officer",
                RequestCallback = true,
                Title = ""
            };
            string jsonString = JsonConvert.SerializeObject(request);
            var result = await _client.PostAsync("v1/TenancyManagementinteractions/servicerequest", new StringContent(jsonString, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task return_a_500_create__service__request_result()
        {
            var request = new CRMServiceRequest
            {
                Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                ContactId = "463adffe-61a5-db11-882c-000000000000",
                Description = "Enquiry Created By Estate Officer",
                RequestCallback = true,
                Title = "internal error"
            };
            string jsonString = JsonConvert.SerializeObject(request);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var result = await _client.PostAsync("v1/TenancyManagementinteractions/servicerequest", new StringContent(jsonString, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }
        [Fact]
        public async Task return_a_201_create_tenancy_Management_interaction_result()
        {
            var request = new TenancyManagement
            {
                contactId = "463adffe-61a5-db11-882c-000000000000",
                enquirySubject = "100000005",
                estateOfficerId = "284216e9-d365-e711-80f9-70106aaaaaaa",
                subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                adviceGiven = "Advice Housing Repair Inquiry",
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
                },
                status = 1,
                processType = "0"
            };

            string jsonString = JsonConvert.SerializeObject(request);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = await _client.PostAsync("v1/TenancyManagementinteractions/createTenancyManagementInteraction", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_404_create__tenancy_Management_interaction_result()
        {
            var result = await _client.PostAsync("v1/TenancyManagementinteraction/createTenancyManagementInteraction", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task return_400_create__tenancy_Management_interaction_result()
        {
            var request = new TenancyManagement
            {
                contactId = "463adffe-61a5-db11-882c-000000000000",
                enquirySubject = "100000005",
                estateOfficerId = "284216e9-d365-e711-80f9-70106aaaaaaa",
                subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                adviceGiven = "Advice Housing Repair Inquiry",
                estateOffice = "5",
                source = "1",
                natureofEnquiry = "3",
                ServiceRequest = new CRMServiceRequest
                {
                    Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                    ContactId = "463adffe-61a5-db11-882c-000000000000",
                    Title = "",
                    Description = "Enquiry Created By Estate Officer"
                }

            };
            string jsonString = JsonConvert.SerializeObject(request);
            var result = await _client.PostAsync("v1/TenancyManagementinteractions/createTenancyManagementInteraction", new StringContent(jsonString, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task return_a_500_create__tenancy_Management_interaction_result()
        {
            var request = new TenancyManagement
            {
                contactId = "463adffe-61a5-db11-882c-000000000000",
                enquirySubject = "internal error",
                estateOfficerId = "284216e9-d365-e711-80f9-70106aaaaaaa",
                subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                adviceGiven = "Advice Housing Repair Inquiry",
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
                },
                processType = "0"
            };
            string jsonString = JsonConvert.SerializeObject(request);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var result = await _client.PostAsync("v1/TenancyManagementinteractions/CreateTenancyManagementInteraction", new StringContent(jsonString, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        #endregion TenancyManagement

        #region TenancyManagementInteraction Update
        [Fact]
        public async Task return_a_200_update_tenancy_result_for_a_valid_request()
        {
            TenancyManagement requestObject = new TenancyManagement
            {
                interactionId = "2a7912b3-b6e0-e711-810e-70106bbbbbbb",
                adviceGiven = "update Tenancy Management Service Request",
                ServiceRequest = new CRMServiceRequest
                {
                    Id = "2a7912b3-b6e0-e711-810e-70106bbbbbbb",
                    Description= "update Tenancy Management Service Request",
                    RequestCallback = true
                },
                estateOfficerName = "User 1",
                status = 1
            };
            var method = new HttpMethod("PATCH");
            string jsonString = JsonConvert.SerializeObject(requestObject);
            string requestUri = "v1/TenancyManagementinteractions";

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage request = new HttpRequestMessage(method, requestUri) { Content = new StringContent(jsonString.ToString(), Encoding.UTF8, "application/json") };
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_500_update_tenancy_result_for_empty_Object_properties()
        {
            TenancyManagement requestObject = new TenancyManagement
            {
                interactionId = "2a7912b3-b6e0-e711-810e-70106bbbbbbb",
                adviceGiven = "internal error",
                ServiceRequest = new CRMServiceRequest
                {
                    Id = "2a7912b3-b6e0-e711-810e-70106bbbbbbb",
                    Description = "internal error",
                    RequestCallback = false

                },
                estateOfficerName = "User 1",
                status = 1
            };
            var method = new HttpMethod("PATCH");
            string jsonString = JsonConvert.SerializeObject(requestObject);
            string requestUri = "v1/TenancyManagementinteractions";

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage request = new HttpRequestMessage(method, requestUri) { Content = new StringContent(jsonString.ToString(), Encoding.UTF8, "application/json") };
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_400_update_tenancy_result_invalid_request()
        {
            TenancyManagement requestObject = new TenancyManagement
            {
                interactionId = "2a7912b3-b6e0-e711-810e-70106bbbbbbb",
                adviceGiven = "update Tenancy Management Service Request",
                ServiceRequest = new CRMServiceRequest
                {
                    Id = "",
                    Description = "update Tenancy Management Service Request",
                    RequestCallback = false
                },
                estateOfficerName = "User 1",
                status = 1
            };
            var method = new HttpMethod("PATCH");
            string jsonString = JsonConvert.SerializeObject(requestObject);
            string requestUri = "v1/TenancyManagementinteractions";

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage request = new HttpRequestMessage(method, requestUri) { Content = new StringContent(jsonString.ToString(), Encoding.UTF8, "application/json") };
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_json_object_for_invalid_tenancy_request()
        {
            var method = new HttpMethod("PATCH");

            string requestUri = "v1/TenancyManagementinteractions";

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage request = new HttpRequestMessage(method, requestUri) { Content = new StringContent("", Encoding.UTF8, "application/json") };
            var response = await _client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            StringBuilder json = new StringBuilder();
            json.Append("[");
            json.Append("{");
            json.Append("\"developerMessage\":\"Please provide a valid TenancyManagemnt request.\",");
            json.Append("\"userMessage\":\"Please provide a valid TenancyManagemnt request.\"");
            json.Append("}");
            json.Append("]");
            Assert.Equal(json.ToString(), responseString);
        }

        [Fact]
        public async Task return_json_object_for_successful_updateRequest()
        {
            TenancyManagement requestObject = new TenancyManagement
            {
                interactionId = "2a7912b3-b6e0-e711-810e-70106bbbbbbb",
                adviceGiven = "update Tenancy Management Service Request",
                ServiceRequest = new CRMServiceRequest
                {
                    Id = "2a7912b3-b6e0-e711-810e-70106bbbbbbb",
                    Description = "update Tenancy Management Service Request",
                    RequestCallback = false
                },
                estateOfficerName = "User 1",
                status = 0
            };

            var method = new HttpMethod("PATCH");
            string jsonString = JsonConvert.SerializeObject(requestObject);
            string requestUri = "v1/TenancyManagementinteractions";

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage request = new HttpRequestMessage(method, requestUri) { Content = new StringContent(jsonString.ToString(), Encoding.UTF8, "application/json") };
            var response = await _client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"result\":");

            json.Append("{");
            json.Append("\"annotationId\":\"63a0e5b9-88df-e311-b8e5-6c3be5ccccccc\",");
            json.Append("\"serviceRequestId\":\"2a7912b3-b6e0-e711-810e-70106bbbbbbb\",");
            json.Append("\"interactionId\":\"2a7912b3-b6e0-e711-810e-70106bbbbbbb\",");
            json.Append("\"description\":\"update Tenancy Management Service Request\",");
            json.Append("\"status\":\"Closed\",");
            json.Append("\"requestCallBack\":false,");
            json.Append("\"processStage\":0");
            json.Append("}");
            json.Append("}");

            Assert.Equal(json.ToString(), responseString);
        }
        #endregion

        #region TenancyGetEndPoints
        [Fact]
        public async Task return_a_200_result_for_valid_request()
        {
            var result = await _client.GetAsync("v1/TenancyManagementinteractions?contactId=463adffe-61a5-db11-882c-000000000000&personType=contact");
            
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_400_result_for_invalid_contactid_parameter()
        {
            var result = await _client.GetAsync("v1/TenancyManagementinteractions?contactId=463adffe-61a5-0014c260c5faeeeee&status=2");
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_400_result_for_invalid_status_parameter()
        {
            var result = await _client.GetAsync("v1/TenancyManagementinteractions?contactId=463adffe-61a5-db11-882c-000000000000&status=0");
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }
        [Fact]
        public async Task return_a_400_result_for_invalid_parameter_name()
        {
            var result = await _client.GetAsync("v1/TenancyManagementinteractions?contaId=463adffe-61a5-db11-882c-000000000000&status=2");
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }
        [Fact]
        public async Task return_a_500_result_service_gives_error()
        {
            var testGuid = Guid.Empty;
            var result = await _client.GetAsync("v1/TenancyManagementinteractions?contactId="+testGuid+"&personType=manager");
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_json_output_for_valid_get_request()
        {
            var result = await _client.GetAsync("v1/TenancyManagementinteractions?contactId=463adffe-61a5-db11-882c-000000000000&personType=contact");
            var responseString = await result.Content.ReadAsStringAsync();
            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"results\":");
            json.Append("[");
            json.Append("{");
            json.Append("\"incidentId\":");
            json.Append("\"39141263-b0e0-e711-810a-e0071bbbbbbb\",");
            json.Append("\"isTransferred\":");
            json.Append("null,");
            json.Append("\"ticketNumber\":");
            json.Append("\"CAS-00059-000000\",");
            json.Append("\"stateCode\":");
            json.Append("0,");
            json.Append("\"processStage\":");
            json.Append("null,");
            json.Append("\"nccOfficersId\":");
            json.Append("\"284216e9-d365-e711-80f9-70106aaaaaaa\",");
            json.Append("\"nccEstateOfficer\":");
            json.Append("\"Test Test\","); json.Append("\"createdon\":");
            json.Append("\"2017-12-14 09:58:49\",");
            json.Append("\"nccOfficerUpdatedById\":");
            json.Append("null,");
            json.Append("\"nccOfficerUpdatedByName\":");
            json.Append("null,");
            json.Append("\"natureOfEnquiryId\":");
            json.Append("\"3\","); json.Append("\"natureOfEnquiry\":");
            json.Append("\"Estate Managment\","); json.Append("\"enquirySubjectId\":");
            json.Append("\"100000005\","); json.Append("\"enquirysubject\":");
            json.Append("\"Apply for Joint Tenancy\",");
            json.Append("\"interactionId\":");
            json.Append("\"d9f0fd60-b5e0-e711-810f-111111\",");
            json.Append("\"areamanagerId\":");
            json.Append("\"AreaManager28645uyo980\",");
            json.Append("\"areaManagerName\":");
            json.Append("\"Area Manager Name\",");
            json.Append("\"officerPatchId\":");
            json.Append("\"OfficerPatch9684056oi046\",");
            json.Append("\"officerPatchName\":");
            json.Append("\"Officer Patch Name\",");
            json.Append("\"areaName\":");
            json.Append("\"Homerton\",");
            json.Append("\"handledBy\":");
            json.Append("\"Estate Officer\",");
            json.Append("\"requestCallBack\":");
            json.Append("false,");
            json.Append("\"contactId\":");
            json.Append("\"ContactId9486954o93\",");
            json.Append("\"contactName\":");
            json.Append("\"Contact name\",");
            json.Append("\"contactPostcode\":");
            json.Append("\"E8 2HH\",");
            json.Append("\"contactAddressLine1\":");
            json.Append("\"Maurice Bishop House\",");
            json.Append("\"contactAddressLine2\":");
            json.Append("\"Hackney\",");
            json.Append("\"contactAddressLine3\":");
            json.Append("null,");
            json.Append("\"contactAddressCity\":");
            json.Append("\"London\",");
            json.Append("\"contactBirthDate\":");
            json.Append("\"01/01/1950\",");
            json.Append("\"contactTelephone\":");
            json.Append("\"123\",");
            json.Append("\"contactEmailAddress\":");
            json.Append("\"test@test.com\",");
            json.Append("\"contactCautionaryAlert\":");
            json.Append("false,");
            json.Append("\"contactPropertyCautionaryAlert\":");
            json.Append("false,");
            json.Append("\"contactLarn\":");
            json.Append("\"LARN834210\",");
            json.Append("\"contactUPRN\":");
            json.Append("null,");
            json.Append("\"householdID\":");
            json.Append("null,");
            json.Append("\"accountCreatedOn\":");
            json.Append("null,");
            json.Append("\"parentInteractionId\":");
            json.Append("parentInteractionId01,");
            json.Append("\"AnnotationList\":");
            json.Append("["); json.Append("{"); 

            json.Append("\"noteText\":"); json.Append("\"Testing closure  at 21/12/2017 13:37:18 by  Test dev\",");
            json.Append("\"annotationId\":"); json.Append("\"b6521622-54e6-e711-8111-7010bbbbbbbb\",");
            json.Append("\"noteCreatedOn\":"); json.Append("\"2017-12-21 13:37:49\""); json.Append("}]}]}");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
            Assert.Equal(json.ToString(), responseString);
        }

        #endregion


        #region GroupTrayTenancyInteractions
        [Fact]
        public async Task return_a_200_group_tray_result_for_valid_request()
        {
            var result = await _client.GetAsync("v1/TenancyManagementInteractions/getareatrayIneractions?OfficeId=5");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_400_group_tray_result_for_invalid_parameter()
        {
            var result = await _client.GetAsync("v1/TenancyManagementInteractions/getareatrayIneractions?OfficeI11d=1");
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_500_group_tray_result_service_gives_error()
        {
            var result = await _client.GetAsync("v1/TenancyManagementInteractions/getareatrayIneractions?OfficeId=00000000-0000-0000-0000-000000000000");
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_json_output_for_group_tray_valid_get_request()
        {
            var result = await _client.GetAsync("v1/TenancyManagementInteractions/getareatrayIneractions?OfficeId=5");
            var responseString = await result.Content.ReadAsStringAsync();
            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"results\":");
            json.Append("[");
            json.Append("{");
            json.Append("\"incidentId\":");
            json.Append("\"39141263-b0e0-e711-810a-e0071bbbbbbb\",");
            json.Append("\"isTransferred\":");
            json.Append("null,");
            json.Append("\"ticketNumber\":");
            json.Append("\"CAS-00059-000000\",");
            json.Append("\"stateCode\":");
            json.Append("0,");
            json.Append("\"processStage\":");
            json.Append("null,");
            json.Append("\"nccOfficersId\":");
            json.Append("\"284216e9-d365-e711-80f9-70106aaaaaaa\",");
            json.Append("\"nccEstateOfficer\":");
            json.Append("\"Test Test\","); json.Append("\"createdon\":");
            json.Append("\"2017-12-14 09:58:49\",");
            json.Append("\"nccOfficerUpdatedById\":");
            json.Append("null,");
            json.Append("\"nccOfficerUpdatedByName\":");
            json.Append("null,");
            json.Append("\"natureOfEnquiryId\":");
            json.Append("\"3\","); json.Append("\"natureOfEnquiry\":");
            json.Append("\"Estate Managment\","); json.Append("\"enquirySubjectId\":");
            json.Append("\"100000005\","); json.Append("\"enquirysubject\":");
            json.Append("\"Apply for Joint Tenancy\",");
            json.Append("\"interactionId\":");
            json.Append("\"d9f0fd60-b5e0-e711-810f-111111\",");
            json.Append("\"areamanagerId\":");
            json.Append("\"AreaManager28645uyo980\",");
            json.Append("\"areaManagerName\":");
            json.Append("\"Area Manager Name\",");
            json.Append("\"officerPatchId\":");
            json.Append("\"OfficerPatch9684056oi046\",");
            json.Append("\"officerPatchName\":");
            json.Append("\"Officer Patch Name\",");
            json.Append("\"areaName\":");
            json.Append("\"Homerton\",");
            json.Append("\"handledBy\":");
            json.Append("\"Estate Officer\",");
            json.Append("\"requestCallBack\":");
            json.Append("false,");
            json.Append("\"contactId\":");
            json.Append("\"ContactId9486954o93\",");
            json.Append("\"contactName\":");
            json.Append("\"Contact name\",");
            json.Append("\"contactPostcode\":");
            json.Append("\"E8 2HH\",");
            json.Append("\"contactAddressLine1\":");
            json.Append("\"Maurice Bishop House\",");
            json.Append("\"contactAddressLine2\":");
            json.Append("\"Hackney\",");
            json.Append("\"contactAddressLine3\":");
            json.Append("null,");
            json.Append("\"contactAddressCity\":");
            json.Append("\"London\",");
            json.Append("\"contactBirthDate\":");
            json.Append("\"01/01/1950\",");
            json.Append("\"contactTelephone\":");
            json.Append("\"123\",");
            json.Append("\"contactEmailAddress\":");
            json.Append("\"test@test.com\",");
            json.Append("\"contactLarn\":");
            json.Append("\"LARN834210\",");
            json.Append("\"contactUPRN\":");
            json.Append("null,");
            json.Append("\"householdID\":");
            json.Append("null,");
            json.Append("\"AnnotationList\":");
            json.Append("["); json.Append("{");

            json.Append("\"noteText\":"); json.Append("\"Testing closure  at 21/12/2017 13:37:18 by  Test dev\",");
            json.Append("\"annotationId\":"); json.Append("\"b6521622-54e6-e711-8111-7010bbbbbbbb\",");
            json.Append("\"noteCreatedOn\":"); json.Append("\"2017-12-21 13:37:49\""); json.Append("}]}]}");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
            Assert.Equal(json.ToString(), responseString);
        }

        #endregion

        #region Login
        [Fact]
        public async Task return_a_200_get_login_result()
        {
            var result = _client.GetAsync("/v1/login/authenticatenhofficers?username=uaccount&password=HackneyTestPassword").Result;
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_empty_result_when_username_or_pasword_doesnot_match()
        {
            var result = _client.GetAsync("/v1/login/authenticatenhofficers?username=uacco&password=HackneyTestPassword").Result;

            string result_string = await result.Content.ReadAsStringAsync();
            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"result\":");
            json.Append("{");
            json.Append("}");
            json.Append("}");

            Assert.Equal(json.ToString(), result_string);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);

        }

        [Fact]
        public async Task return_a_500_for_result_authenticatenhofficers_when_service_gives_error()
        {
            var result = await _client.GetAsync("v1/login/authenticatenhofficers?username=uacc&password=HackneyTestPassword");
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_400_for_result_authenticatenhofficers_when_parameter_name_wrong()
        {
            var result = await _client.GetAsync("v1/login/authenticatenhofficers?usernamh=uacc&password=HackneyTestPassword");
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_actual_result_when_username_or_pasword_does_match()
        {
            var result = _client.GetAsync("/v1/login/authenticatenhofficers?username=uaccount&password=HackneyTestPassword").Result;

            string result_string = await result.Content.ReadAsStringAsync();
            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"result\":");
            json.Append("{");
            json.Append("\"estateOfficerLoginId\":");
            json.Append("\"login8f6a9cba\",");
            json.Append("\"officerId\":");
            json.Append("\"OfficerId70106faa6a31\",");
            json.Append("\"firstName\":");
            json.Append("\"Test\",");
            json.Append("\"surName\":");
            json.Append("\"Smith\",");
            json.Append("\"username\":");
            json.Append("\"UserName\",");
            json.Append("\"fullName\":");
            json.Append("\"Test Smith\",");
            json.Append("\"isManager\":");
            json.Append(false.ToString().ToLower() + ",");
            json.Append("\"areamanagerId\":");
            json.Append("\"OfficermanagerId\",");
            json.Append("\"officerPatchId\":");
            json.Append("\"PatchId7yo983o01\",");
            json.Append("\"areaId\":");
            json.Append("\"AreaId1\"");
            json.Append("}");
            json.Append("}");

            Assert.Equal(json.ToString(), result_string);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);

        }
        #endregion

        #region AccountDetailsByTagOrParisRef

        [Fact]
        public async Task return_a_200_get_account_details_result()
        {
            var result = _client.GetAsync("/v1/accounts/AccountDetailsByPaymentorTagReference?referencenumber=228009977").Result;
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }


        [Fact]
        public async Task return_a_400_get_account_details_result_for_invalid_name_of_parameter()
        {
            var result = _client.GetAsync("/v1/accounts/AccountDetailsByPaymentorTagReference?parisReference1=228003470").Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

        }

        [Fact]
        public async Task return_a_400_get_account_details_result_for_invalid_parisReference_parameter()
        {
            var result = _client.GetAsync("/v1/accounts/AccountDetailsByPaymentorTagReference?reference=22800347 ").Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

        }

        [Fact]
        public async Task return_a_400_get_account_details_result_for_empty_parameter()
        {
            var result = _client.GetAsync("/v1/accounts/AccountDetailsByPaymentorTagReference?reference=").Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

        }

        [Fact]
        public async Task return_a_400_get_account_details_result_for_no_parameter()
        {
            var result = _client.GetAsync("/v1/accounts/AccountDetailsByPaymentorTagReference").Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task return_a_404_get_account_details_result()
        {
            var result = _client.GetAsync("v1/CRMAccount?parisReferenceAndPostcode=1930985702").Result;
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

        }
        [Fact]
        public async Task return_a_json_object_for_valid_account_details_requests()
        {
            var result = _client.GetAsync("/v1/accounts/AccountDetailsByPaymentorTagReference?referencenumber=228009977").Result;
            string result_string = await result.Content.ReadAsStringAsync();
            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"results\":");
            json.Append("[");
            json.Append("{");
            json.Append("\"accountid\":\"93d621ae-46c6-e711-8111-70106ssssssss\",");
            json.Append("\"tagReferenceNumber\":\"010000/01\",");
            json.Append("\"benefit\":\"0.0\",");
            json.Append("\"propertyReferenceNumber\":\"00000008\",");
            json.Append("\"currentBalance\":\"564.35\",");
            json.Append("\"rent\":\"114.04\",");
            json.Append("\"housingReferenceNumber\":\"010001\",");
            json.Append("\"directdebit\":null,");
            json.Append("\"ListOfTenants\":");
            json.Append("[");
            json.Append("{");
            json.Append("\"personNumber\":null,");
            json.Append("\"responsible\":false,");
            json.Append("\"title\":\"Mr\",");
            json.Append("\"forename\":\"TestA\",");
            json.Append("\"surname\":\"TestB\"");
            json.Append("}");
            json.Append("],");
            json.Append("\"ListOfAddresses\":");
            json.Append("[");
            json.Append("{");
            json.Append("\"postCode\":\"E8 2HH\",");
            json.Append("\"shortAddress\":\"Maurice Bishop House\",");
            json.Append("\"addressTypeCode\":1");
            json.Append("}");
            json.Append("]");
            json.Append("}");
            json.Append("]");
            json.Append("}");
            Assert.Equal(json.ToString(), result_string);
        }

        [Fact]
        public async Task return_a_500_get_account__details_result()
        {
            var result = _client.GetAsync("/v1/accounts/AccountDetailsByPaymentorTagReference?referencenumber=228000000").Result;
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }
       
        #endregion

        #region GetAreaPatch
        [Fact]
        public async Task getareapatch_should_return_a_200_statusCode_for_the_successful_given_postCode_response()
        {
            var result = await _client.GetAsync("v1/AreaPatch/GetAreaPatch?postcode=N16%205DH&UPRN=12345678");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task getareapatch_should_return_a_400_for_an_invalid_postCode_request()
        {
            var result = await _client.GetAsync("v1/AreaPatch/GetAreaPatch?postcode=N16");
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task getareapatch_should_return_a_404_for_an_invalid_request()
        {
            var result = await _client.GetAsync("v1/AreaPatch");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task getareapatch_should_return_a_500_for_an_Invalid_Request()
        {
            var result = await _client.GetAsync("v1/AreaPatch/GetAreaPatch?postcode=N16%205DC&UPRN=12345678");
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public async Task GetAreaPatch_return_valid_json_output_for_valid_get_housing_account_notification_request()
        {
            var result = await _client.GetAsync("v1/AreaPatch/GetAreaPatch?postcode=N16%205DH&UPRN=12345678");

            string result_string = await result.Content.ReadAsStringAsync();
            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"result\":");
            json.Append("{");
            json.Append("\"hackneyAreaId\":");
            json.Append("\"1\",");
            json.Append("\"hackneyareaName\":");
            json.Append("\"Stamford Hill\",");
            json.Append("\"hackneyPropertyReference\":");
            json.Append("\"00000004\",");
            json.Append("\"hackneyPostCode\":");
            json.Append("\"N16 5DH\",");
            json.Append("\"hackneyllpgReference\":");
            json.Append("\"10000000000\",");
            json.Append("\"hackneyManagerPropertyPatchId\":");
            json.Append("\"906ebd93-ee3c-47ee-8c88-46638bqqqqqq\",");
            json.Append("\"hackneyManagerPropertyPatchName\":");
            json.Append("\"Test Manager\",");
            json.Append("\"hackneyWardId\":");
            json.Append("\"2\",");
            json.Append("\"hackneyWardName\":");
            json.Append("\"SpringField\",");
            json.Append("\"hackneyEstateofficerPropertyPatchId\":");
            json.Append("\"e2420bff-b6c9-4d42-80f3-b8dcccccccc\",");
            json.Append("\"hackneyEstateofficerPropertyPatchName\":");
            json.Append("\"Test Officer\",");
            json.Append("\"hackneyEstateOfficerId\":");
            json.Append("null");
            json.Append("}");
            json.Append("}");

            Assert.Equal(json.ToString(), result_string);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }
        #endregion

        #region GetAllOfficersPerArea
        [Fact]
        public async Task return_valid_json_output_for_valid_get_all_officers_per_area_request()
        {
            var result = await _client.GetAsync("v1/areapatch/getallofficersperarea?areaId=3");

            string result_string = await result.Content.ReadAsStringAsync();
            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"results\":");
            json.Append("[{");
            json.Append("\"propertyAreaPatchId\":");
            json.Append("\"\",");
            json.Append("\"estateOfficerPropertyPatchId\":");
            json.Append("\"\",");
            json.Append("\"estateOfficerPropertyPatchName\":");
            json.Append("\"\",");
            json.Append("\"llpgReferenece\":");
            json.Append("\"\",");
            json.Append("\"patchId\":");
            json.Append("\"\",");
            json.Append("\"patchName\":");
            json.Append("\"\",");
            json.Append("\"propetyReference\":");
            json.Append("\"\",");
            json.Append("\"wardName\":");
            json.Append("\"\",");
            json.Append("\"wardId\":");
            json.Append("\"\",");
            json.Append("\"areaName\":");
            json.Append("\"Central Panel\",");
            json.Append("\"areaId\":");
            json.Append("\"3\",");
            json.Append("\"managerPropertyPatchId\":");
            json.Append("\"ae7b4690-b005-e811-811c-70106fffffff\",");
            json.Append("\"managerPropertyPatchName\":");
            json.Append("\"Test E G\",");
            json.Append("\"areaManagerName\":");
            json.Append("\"Test G\",");
            json.Append("\"areamanagerId\":");
            json.Append("\"d27c6a5c-da01-e811-8112-70106tttttt\",");
            json.Append("\"isaManager\":");
            json.Append("true,");
            json.Append("\"officerId\":");
            json.Append("\"ae7b4690-b005-e811-811c-70106fffffff\",");
            json.Append("\"officerName\":");
            json.Append("\"Test G (Area Manager)\"");
            json.Append("},");
            json.Append("{");
            json.Append("\"propertyAreaPatchId\":");
            json.Append("\"a692a27c-b205-e811-811c-70106wwwwww\",");
            json.Append("\"estateOfficerPropertyPatchId\":");
            json.Append("\"be77dd44-b005-e811-811c-70106aaaaaa\",");
            json.Append("\"estateOfficerPropertyPatchName\":");
            json.Append("\"Test Officer Patch\",");
            json.Append("\"llpgReferenece\":");
            json.Append("\"100021024456\",");
            json.Append("\"patchId\":");
            json.Append("\"9796145f-b4f7-e711-8112-70106faaaaaa\",");
            json.Append("\"patchName\":");
            json.Append("\"Test dev\",");
            json.Append("\"propetyReference\":");
            json.Append("\"00028225\",");
            json.Append("\"wardName\":");
            json.Append("\"De Beauvoir\",");
            json.Append("\"wardId\":");
            json.Append("\"1\",");
            json.Append("\"areaName\":");
            json.Append("\"Central Panel\",");
            json.Append("\"areaId\":");
            json.Append("\"3\",");
            json.Append("\"managerPropertyPatchId\":");
            json.Append("\"ae7b4690-b005-e811-811c-70106fffffff\",");
            json.Append("\"managerPropertyPatchName\":");
            json.Append("\"Test E G\",");
            json.Append("\"areaManagerName\":");
            json.Append("\"Test G\",");
            json.Append("\"areamanagerId\":");
            json.Append("\"d27c6a5c-da01-e811-8112-70106tttttt\",");
            json.Append("\"isaManager\":");
            json.Append("false,");
            json.Append("\"officerId\":");
            json.Append("\"be77dd44-b005-e811-811c-70106aaaaaa\",");
            json.Append("\"officerName\":");
            json.Append("\"Test dev\"");
            json.Append("}]");
            json.Append("}");

            Assert.Equal(json.ToString(), result_string);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_200_result_for_valid_get_all_officers_per_area_request()
        {
            var result = await _client.GetAsync("v1/areapatch/getallofficersperarea?areaId=1");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_empty_result_when_a_matching_result_for_valid_get_all_officers_per_area_request()
        {
            var result = await _client.GetAsync("v1/areapatch/getallofficersperarea?areaId=11");

            string result_string = await result.Content.ReadAsStringAsync();
            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"results\":");
            json.Append("{");
            json.Append("}");
            json.Append("}");

            Assert.Equal(json.ToString(), result_string);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_500_for_valid_get_all_officers_per_area_request_when_service_gives_error()
        {
            var result = await _client.GetAsync("v1/areapatch/getallofficersperarea?areaId=10");
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_404_for_get_all_officers_per_area()
        {
            var result = await _client.GetAsync("v1/areapatch/getallofficersp?areaId=1");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

        }

        [Fact]
        public async Task return_a_400_for_get_all_officers_per_area()
        {
            var result = await _client.GetAsync("v1/areapatch/getallofficersperarea?areaIddd=1");
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }
        #endregion

        #region TranferCall
        [Fact]
        public async Task return_200_status_code_for_successfull_call_transfer_on_valid_request()
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
                officerPatchId = "estate officer patch",
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

            string jsonString = JsonConvert.SerializeObject(transferCallRequestObject);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var result = await _client.PutAsync("v1/TenancyManagementinteractions/transfercall", new StringContent(jsonString, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task return_500_status_code_transfer_on_invalid_data_in_request()
        {
            var transferCallRequestObject = new TenancyManagement
            {
                contactId = "463adffe-61a5-db11-882c-000000000000",
                enquirySubject = "internal error",
                estateOfficerId = "284216e9-d365-e711-80f9-70106aaaaaaa",
                subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                adviceGiven = "Housing Repair Inquiry",
                estateOffice = "5",
                source = "1",
                natureofEnquiry = "3",
                officerPatchId = "be77dd44-b005-e811-811c-70106aaaaaa",
                areaName = "internal error",
                managerId = "ae7b4690-b005-e811-811c-70106fffffff",
                interactionId = "1f46ea18-fe0b-e811-8116-70106faa6a31",
                ServiceRequest = new CRMServiceRequest
                {
                    Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                    ContactId = "463adffe-61a5-db11-882c-000000000000",
                    Title = "Tenancy Management",
                    Description = "Enquiry Created By Estate Officer",
                    Id = "b2fb842c-fe0b-e811-8116-70106faa6a31"
                }
            };

            string jsonString = JsonConvert.SerializeObject(transferCallRequestObject);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var result = await _client.PutAsync("v1/TenancyManagementinteractions/transfercall", new StringContent(jsonString, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public async Task return_400_status_code_transfer_on_invalid_data_in_request()
        {
            var transferCallRequestObject = new TenancyManagement
            {
                contactId = "463adffe-61a5-db11-882c-000000000000",
                enquirySubject = "fob-key",
                estateOfficerId = "284216e9-d365-e711-80f9-70106aaaaaaa",
                subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                adviceGiven = "Housing Repair Inquiry",
                estateOffice = "5",
                source = "1",
                natureofEnquiry = "3",
                officerPatchId = "",
                areaName = "3",
                managerId = "",
                interactionId = "1f46ea18-fe0b-e811-8116-70106faa6a31",
                ServiceRequest = new CRMServiceRequest
                {
                    Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                    ContactId = "463adffe-61a5-db11-882c-000000000000",
                    Title = "Tenancy Management",
                    Description = "Enquiry Created By Estate Officer",
                    Id = "b2fb842c-fe0b-e811-8116-70106faa6a31"
                }
            };

            string jsonString = JsonConvert.SerializeObject(transferCallRequestObject);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var result = await _client.PutAsync("v1/TenancyManagementinteractions/transfercall", new StringContent(jsonString, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task return_400_status_code_transfer_on_no_data_in_request()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var result = await _client.PutAsync("v1/TenancyManagementinteractions/transfercall", new StringContent("", Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
        [Fact]
        public async Task return_404_status_code_transfer_on_no_data_in_request()
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
                officerPatchId = "estate officer patch",
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

            string jsonString = JsonConvert.SerializeObject(transferCallRequestObject);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var result = await _client.PutAsync("v1/TenancyManagementinteractions/transfercalll", new StringContent(jsonString, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
        #endregion

        #region CitizenIndexSearch
        [Fact]
        public async Task cititzen_index_search_request_returns_successfull_200_response()
        {
            var result = await _client.GetAsync("v1/CitizenIndexSearch?surname=DTEST");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task cititzen_index_search_request_throws_400_response()
        {
            var result = await _client.GetAsync("v1/CitizenIndexSearch?surname=");
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task cititzen_index_search_request_throws_404_response()
        {
            var result = await _client.GetAsync("v1/CitizenIndexSeach?surname=");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

      
        [Fact]
        public async Task cititzen_index_should_return_a_500_for_an_Invalid_Request()
        {
            var result = await _client.GetAsync("v1/CitizenIndexSearch?surname=throw500");
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }
        #endregion

        #region GetAsync Contact Cautionary Alerts

        [Fact]
        public async Task return_a_200_get_cautionary_alerts_result()
        {
            var result = _client.GetAsync("v1/Contacts/GetCautionaryAlerts?urpn=99999999").Result;
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_404_get_cautionary_alerts_result()
        {
            var result = _client.GetAsync("v1/Contact/GetCautionaryAlerts?urpn=99999999").Result;
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

        }

        [Fact]
        public async Task return_a_400_get_cautionary_alerts_result()
        {
            var result = _client.GetAsync("v1/Contacts/GetCautionaryAlerts?urpn=").Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

       
        [Fact]
        public async Task return_a_500_get_cautionary_alerts_result()
        {
            var result = _client.GetAsync("v1/Contacts/GetCautionaryAlerts?urpn=throw500").Result;
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public async Task return_a_json_object_for_valid__get_cautionary_alerts_request()
        {
            var result = _client.GetAsync("v1/Contacts/GetCautionaryAlerts?urpn=99999999").Result;
            string result_string = await result.Content.ReadAsStringAsync();
            //var result_string = result.Content.ReadAsStringAsync().Result;
            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"results\":");
            json.Append("[");
            json.Append("{");
            json.Append("\"cautionaryAlertType\":\"Dangerous dog\",");
            json.Append("\"cautionaryAlertId\":\"alertId1234567890\",");
            json.Append("\"contactId\":\"1b2b3b4b5b6b0bo08\",");
            json.Append("\"contactName\":\"Test name\",");
            json.Append("\"uprn\":\"1234567\",");
            json.Append("\"createdOn\":\"2016-12-08 23:00:00\"");
            json.Append("}");
            json.Append("]");
            json.Append("}");
            Assert.Equal(json.ToString(), result_string);
        }
        #endregion

        #region Remove Cautionary Alert
        [Fact]
        public async Task return_a_200_remove_cautionary_alerts_result()
        {
            var request = new CautionaryAlert
            {
                cautionaryAlertIds = new List<string>{
                    "12345667-8d23-e811-8120-70106f568811",
                    "12345677-8e23-e811-8120-70106f5687811",
                    "23456788-8e23-e811-8120-70106f5678181"
                },
                contactId = "o999993-e716-e811-811e-788888aa6a11",
                uprn = "1000089925",
                cautionaryAlertIsToBeRemoved = false
            };
            string jsonString = JsonConvert.SerializeObject(request);
            var result = _client.PostAsync("v1/Contacts/RemoveCautionaryAlerts", new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }
        
        [Fact]
        public async Task return_a_404_remove_cautionary_alerts_result()
        {
            var result = _client.PostAsync("v1/Contact/RemoveCautionaryAlerts", new StringContent(string.Empty, Encoding.UTF8, "application/json")).Result;
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

        }

        [Fact]
        public async Task return_a_400_remove_cautionary_alerts_result()
        {
            var result = _client.PostAsync("v1/Contacts/RemoveCautionaryAlerts", new StringContent(string.Empty, Encoding.UTF8, "application/json")).Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }


        [Fact]
        public async Task return_a_500_remove_cautionary_alerts_result()
        {
            var request = new CautionaryAlert
            {
                cautionaryAlertIds = new List<string>{
                    "12345667-8d23-e811-8120-70106f569999",
                    "12345677-8e23-e811-8120-70106f56879999",
                    "23456788-8e23-e811-8120-70106f5679999"
                },
                contactId = "o999993-e716-e811-811e-788888aa6a11",
                uprn = "internal error",
                cautionaryAlertIsToBeRemoved = false
            };
            string jsonString = JsonConvert.SerializeObject(request);
            var result = _client.PostAsync("v1/Contacts/RemoveCautionaryAlerts", new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        #endregion

        #region Create Cautionary Alert
        [Fact]
        public async Task return_a_200_create_cautionary_alerts_result()
        {
            var request = new CautionaryAlert
            {
                cautionaryAlertType = new List<string>
                {
                    "3"
                },
                contactId = "o999993-e716-e811-811e-788888aa6a11",
                uprn = "1000089925",
            };
            string jsonString = JsonConvert.SerializeObject(request);
            var result = _client.PostAsync("v1/Contacts/CreateCautionaryAlerts", new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_404_create_cautionary_alerts_result()
        {
            var result = _client.PostAsync("v1/Contact/CreateCautionaryAlert", new StringContent(string.Empty, Encoding.UTF8, "application/json")).Result;
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

        }

        [Fact]
        public async Task return_a_400_create_cautionary_alerts_result()
        {
            var result = _client.PostAsync("v1/Contacts/CreateCautionaryAlerts", new StringContent(string.Empty, Encoding.UTF8, "application/json")).Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }


        [Fact]
        public async Task return_a_500_create_cautionary_alerts_result()
        {
            var request = new CautionaryAlert
            {
                cautionaryAlertType = new List<string>
                {
                    "3"
                },
                contactId = "o999993-e716-e811-811e-788888aa6a11",
                uprn = "internal error",
            };
            string jsonString = JsonConvert.SerializeObject(request);
            var result = _client.PostAsync("v1/Contacts/CreateCautionaryAlerts", new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public async Task return_a_json_object_for_succesful_creation_of_alert()
        {
            var request = new CautionaryAlert
            {
                cautionaryAlertType = new List<string>
                {
                    "3"
                },
                contactId = "o999993-e716-e811-811e-788888aa6a11",
                uprn = "1000089925",
            };
            string createdOn = DateTime.Today.ToString("yyyy-MM-dd");
            string jsonString = JsonConvert.SerializeObject(request);
            var result = _client.PostAsync("v1/Contacts/CreateCautionaryAlerts", new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;
            string result_string = await result.Content.ReadAsStringAsync();
            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"alertContactId\":\"o999993-e716-e811-811e-788888aa6a11\",");
            json.Append("\"alertUprn\":\"1000089925\",");
            json.Append("\"alertCautionaryAlertType\":[\"3\"],");
            json.Append("\"createdOn\":\"" + createdOn+ "\"");
            json.Append("}");
            Assert.Equal(json.ToString(), result_string);
        }
      
        #endregion


        #region GetTagReferenceNumber

        [Fact]
        public async Task return_a_200_get_tag_reference_result()
        {
            var result = _client.GetAsync("/v1/accounts/gettagreferencenumber?hackneyhomesId=316669").Result;
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }


        [Fact]
        public async Task return_a_400_get_tag_reference_result_for_invalid_name_of_parameter()
        {
            var result = _client.GetAsync("/v1/accounts/gettagreferencenumber?hackneyhomesId1=316669").Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

        }

      
        [Fact]
        public async Task return_a_400_get_tag_reference_result_for_empty_parameter()
        {
            var result = _client.GetAsync("/v1/accounts/gettagreferencenumber?hackneyhomesId=").Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

        }

        [Fact]
        public async Task return_a_400_get_tag_reference_result_for_no_parameter()
        {
            var result = _client.GetAsync("/v1/accounts/gettagreferencenumber").Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task return_a_404_get_tag_reference_result()
        {
            var result = _client.GetAsync("/v1/accounts/gettagreferencenumber1=1930985702").Result;
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

        }

        #endregion

        #region Update Patch or Area Manager
        [Fact]
        public async Task return_a_200_update_patch_or_area_manager_result()
        {
            var request = new OfficerAreaPatch
            {
                officerId = "OfficerId-e811-8120-70106f5678181",
                patchId = "be77dd44-b005-e811-811c-0000000",
                updatedByOfficer = "be77dd44-b005-e811-811c-22222",
                isUpdatingPatch = true,
                deleteExistingRelationship = false
            };
            string jsonString = JsonConvert.SerializeObject(request);
            var result = _client.PutAsync("v1/AreaPatch/UpdateOfficerPatchOrAreaManager", new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_404_update_patch_or_area_manager_result()
        {
            var result = _client.PutAsync("v1/AreaPatch/UpdateOfficerPatchOrAreaManage", new StringContent(string.Empty, Encoding.UTF8, "application/json")).Result;
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

        }

        [Fact]
        public async Task return_a_400_update_patch_or_area_manager_result()
        {
            var result = _client.PutAsync("v1/AreaPatch/UpdateOfficerPatchOrAreaManager", new StringContent(string.Empty, Encoding.UTF8, "application/json")).Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }


        [Fact]
        public async Task return_a_500_update_patch_or_area_manager_result()
        {
            var request = new OfficerAreaPatch
            {
                officerId = "internal error",
                patchId = "be77dd44-b005-e811-811c-0000000",
                updatedByOfficer = "be77dd44-b005-e811-811c-22222",
                isUpdatingPatch = true,
                deleteExistingRelationship = false
            };
            string jsonString = JsonConvert.SerializeObject(request);
            var result = _client.PutAsync("v1/AreaPatch/UpdateOfficerPatchOrAreaManager", new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public async Task return_a_json_object_for_valid_update_patch_or_area_manager_result()
        {
            var request = new OfficerAreaPatch
            {
                officerId = "OfficerId-e811-8120-70106f5678181",
                patchId = "be77dd44-b005-e811-811c-0000000",
                updatedByOfficer = "be77dd44-b005-e811-811c-22222",
                isUpdatingPatch = true,
                deleteExistingRelationship = false
            };
            string jsonString = JsonConvert.SerializeObject(request);
            var result = _client.PutAsync("v1/AreaPatch/UpdateOfficerPatchOrAreaManager", new StringContent(jsonString, Encoding.UTF8, "application/json")).Result; ;
            string result_string = await result.Content.ReadAsStringAsync();
            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"id\":\"be77dd44-b005-e811-811c-0000000\",");
            json.Append("\"patchName\":\"Patch 6-2\"");
            json.Append("}");
            Assert.Equal(json.ToString(), result_string);
        }
        #endregion 

        #region GetAsync all unassigned officers
        [Fact]
        public async Task return_a_200_get_all_unassigned_officers_result()
        {
            var result = _client.GetAsync("v1/AreaPatch/GetAllUnassignedOfficers").Result;
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task return_a_404_get_all_unassigned_officers_result()
        {
            var result = _client.GetAsync("v1/AreaPatch/GetAllUnassignedOfficer").Result;
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

        }

        [Fact]
        public async Task return_a_json_object_for_valid_get_all_unassigned_officers_request()
        {
            var result = _client.GetAsync("v1/AreaPatch/GetAllUnassignedOfficers").Result;
            string result_string = await result.Content.ReadAsStringAsync();
            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"results\":");
            json.Append("[");
            json.Append("{");
            json.Append("\"firstName\":\"Test First Name\",");
            json.Append("\"lastName\":\"Test Last Name\",");
            json.Append("\"fullName\":\"Test First Name Test Last Name\",");
            json.Append("\"officerId\":\"02345-o0e93o91o-545o902\"");
            json.Append("}");
            json.Append("]");
            json.Append("}");
            Assert.Equal(json.ToString(), result_string);
        }
        #endregion

        #region Manager Officer Accounts and login Accounts
        [Fact]
        [Trait("OfficerAccountIntegrationTest", "Integration")]
        public async Task return_a_201_create_officer_account_and_officer_login_account_result()
        {
            var request = new EstateOfficerAccount()
            {
                OfficerAccount = new OfficerAccount() { HackneyFirstname = "Test First", HackneyLastname = "Test_Last", HackneyEmailaddress = "flast@test.com" },
                OfficerLoginAccount = new OfficerLoginAccount() { HackneyUsername = "Test user Name", HackneyPassword = "HackneyTestPassword" }
            };

            string jsonString = JsonConvert.SerializeObject(request);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = await _client.PostAsync("/v1/OfficerAccounts/CreateOfficerAccount", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        [Trait("OfficerAccountIntegrationTest", "Integration")]
        public async Task return_a_204_when_successfully_disabled_officer_account_and_officer_login_account_disabled()
        {

            var method = new HttpMethod("PATCH");
            string requestUri = "/v1/OfficerAccounts/DisableOfficerAccount?officerId=fa164f0b-a031-e811-811a-70106faaf8c1&officerLoginId=fb164f0b-a031-e811-811a-70106faaf8c1";

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage request = new HttpRequestMessage(method, requestUri) { Content = new StringContent(string.Empty, Encoding.UTF8, "application/json") };

            var result = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        [Trait("OfficerAccountIntegrationTest", "Integration")]
        public async Task return_a_valid_json_result_for_the_successfull_account_creation()
        {
            var request = new EstateOfficerAccount()
            {
                OfficerAccount = new OfficerAccount() { HackneyFirstname = "Test First", HackneyLastname = "Test_Last", HackneyEmailaddress = "flast@test.com" },
                OfficerLoginAccount = new OfficerLoginAccount() { HackneyUsername = "Test user Name", HackneyPassword = "HackneyTestPassword" }
            };

            string jsonString = JsonConvert.SerializeObject(request);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = await _client.PostAsync("/v1/OfficerAccounts/CreateOfficerAccount", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            string result_string = await result.Content.ReadAsStringAsync();

            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"result\":");
            json.Append("{");
            json.Append("\"EstateOfficerid\":\"fa164f0b-a031-e811-811a-70106faaf8c1\",");
            json.Append("\"Name\":\"Test First And Last\",");
            json.Append("\"FirstName\":\"Test First\",");
            json.Append("\"LastName\":\"Test_Last\",");
            json.Append("\"EmailAddress\":\"flast@test.com\",");
            json.Append("\"OfficerAccountStatus\":\"Active\",");
            json.Append("\"EstateOfficerLoginId\":\"fb164f0b-a031-e811-811a-70106faaf8c1\",");
            json.Append("\"UserName\":\"Test user Name\",");
            json.Append("\"LoginAccountStatus\":\"Active\"");
            json.Append("}");
            json.Append("}");

            Assert.Equal(json.ToString(), result_string);
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal("application/json", result.Content.Headers.ContentType.MediaType);
        }


        [Fact]
        [Trait("OfficerAccountIntegrationTest", "Integration")]
        public async Task return_a_400_for_an_invalid_request_when_create_officer_account()
        {
            var request = new EstateOfficerAccount()
            {
                OfficerAccount = new OfficerAccount() { HackneyFirstname = "", HackneyLastname = "Test_Last", HackneyEmailaddress = "flast@test.com" },
                OfficerLoginAccount = new OfficerLoginAccount() { HackneyUsername = "Test user Name", HackneyPassword = "HackneyTestPassword" }
            };

            string jsonString = JsonConvert.SerializeObject(request);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = await _client.PostAsync("/v1/OfficerAccounts/CreateOfficerAccount", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        [Trait("OfficerAccountIntegrationTest", "Integration")]
        public async Task return_a_409_for_an_invalid_request_when_username_conflict()
        {
            var request = new EstateOfficerAccount()
            {
                OfficerAccount = new OfficerAccount() { HackneyFirstname = "Test First", HackneyLastname = "Test_Last", HackneyEmailaddress = "flast@test.com" },
                OfficerLoginAccount = new OfficerLoginAccount() { HackneyUsername = "Test user Name conflict", HackneyPassword = "HackneyTestPassword" }
            };

            string jsonString = JsonConvert.SerializeObject(request);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = await _client.PostAsync("/v1/OfficerAccounts/CreateOfficerAccount", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        }
        #endregion

        #region GetTRAsForPatch

        [Fact]
        public async Task return_a_404_get_tra_for_patch_result()
        {
            var result = _client.GetAsync("v1/TRA/GetTRAForPatchs").Result;
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
        [Fact]
        public async Task return_a_400_get_tra_for_patch_result_when_input_is_invalid()
        {
            var result = _client.GetAsync("v1/TRA/GetTRAForPatch?patchId=123").Result;
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task return_empty_object_when_no_matches()
        {
            var result = _client.GetAsync("v1/TRA/GetTRAForPatch?patchId=b111b111-1111-b111-1111-11111bbbb1b1").Result;
            List<TRA> fakeListOfTRA = new List<TRA>();
            var expectedObject = new
            {
                results = fakeListOfTRA
            };
            JObject response =
                JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(JsonConvert.SerializeObject(expectedObject), JsonConvert.SerializeObject(response));
        }

        #endregion 
    }
}
