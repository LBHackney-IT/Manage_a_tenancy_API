using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using Moq;
using Xunit;
using Newtonsoft.Json;
using System.Net;
using ManageATenancyAPI.Models.Housing;
using ManageATenancyAPI.Tests.Helpers;
using ManageATenancyAPI.Helpers.Housing;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace ManageATenancyAPI.Tests.Actions.Housing
{
    public class AccountActionsTests
    {


        #region GetAccountDetailsByContactID

        public string createAccountDetailsByContactIDObject()
        {
            JObject accountResponse = new JObject();
            accountResponse["accountid"] = "accountID";
            accountResponse["housing_tag_ref"] = "tagRef123";
            accountResponse["housing_u_saff_rentacc"] = "paymentRef123";
            accountResponse["housing_anticipated"] = "benefit123";
            accountResponse["housing_prop_ref"] = "ref123";
            accountResponse["housing_cur_bal"] = "currBalance123";
            accountResponse["housing_rent"] = "rent123";
            accountResponse["housing_house_ref"] = "houserRef123";
            accountResponse["housing_directdebit"] = "directDebit123";
            accountResponse["housing_tenure"] = "tenantType123";
            accountResponse["housing_cot"] = "startDate-12";
            accountResponse["housing_terminated"] = false;
            accountResponse["housing_accounttype"] = "accType";
            accountResponse["housing_agr_type"] = "agreementType";
            var result = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            listJObject.Add(accountResponse);
            result.Add("value", listJObject);
            return JsonConvert.SerializeObject(result);

        }
        [Fact]
        public async Task get_account_details_by_contact_id_successfully()
        {
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());

            var mockLeaseAccountCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();
            mockLeaseAccountCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(It.IsAny<HttpClient>());

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            AccountDetails accountDetails = new AccountDetails()
            {
                accountid = "accountID",
                tagReferenceNumber = "tagRef123",
                paymentReferenceNumber = "paymentRef123",
                benefit = "benefit123",
                propertyReferenceNumber = "ref123",
                currentBalance = "currBalance123",
                rent = "rent123",
                housingReferenceNumber = "houserRef123",
                directdebit = "directDebit123",
                tenuretype = "Temp Traveller",
                tenancyStartDate = "startDate-12",
                isAgreementTerminated = false,
                accountType = "accType",
                agreementType ="agreementType"
            };
            
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(createAccountDetailsByContactIDObject(), System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<AccountActions>>();

            var mockuhwWarehousRepositoryCall = new Mock<IHackneyUHWWarehouseService>();

            var actualResult = new 
            {
                results = accountDetails
            };
            var accountActions = new AccountActions(mockILoggerAdapter.Object,
                mockLeaseAccountCallBuilder.Object, mockApiCall.Object, mockuhwWarehousRepositoryCall.Object, mockAccessToken.Object);

            var expected = await accountActions.GetAccountDetailsByContactId(It.IsAny<string>());
            
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actualResult));
     
        }
        [Fact]
        public async Task get_account_details_by_contact_id_returns_empty_object_if_no_result()
        {
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());

            var mockLeaseAccountCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();
            mockLeaseAccountCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(It.IsAny<HttpClient>());

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            AccountDetails accountDetails = new AccountDetails()
            {
                accountid = null,
                tagReferenceNumber = null,
                paymentReferenceNumber = null,
                benefit = null,
                propertyReferenceNumber = null,
                currentBalance = null,
                rent = null,
                housingReferenceNumber = null,
                directdebit = null,
                tenuretype = null,
                tenancyStartDate = null,
                isAgreementTerminated = false,
                accountType = null,
                agreementType = null
            };

            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(String.Empty, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<AccountActions>>();

            var mockuhwWarehousRepositoryCall = new Mock<IHackneyUHWWarehouseService>();

            var actualResult = new
            {
                results = accountDetails
            };
            var accountActions = new AccountActions(mockILoggerAdapter.Object,
                mockLeaseAccountCallBuilder.Object, mockApiCall.Object, mockuhwWarehousRepositoryCall.Object, mockAccessToken.Object);

            var expected = await accountActions.GetAccountDetailsByContactId(It.IsAny<string>());

            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actualResult));
        }
        [Fact]
        public async Task get_account_details_by_contact_id_throws_missing_result_expection_if_result_is_null()
        {
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());

            var mockLeaseAccountCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();
            mockLeaseAccountCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(It.IsAny<HttpClient>());

            var mockApiCall = new Mock<IHackneyHousingAPICall>();

            mockApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(()=>null);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<AccountActions>>();

            var mockuhwWarehousRepositoryCall = new Mock<IHackneyUHWWarehouseService>();

            var accountActions = new AccountActions(mockILoggerAdapter.Object,
                mockLeaseAccountCallBuilder.Object, mockApiCall.Object, mockuhwWarehousRepositoryCall.Object, mockAccessToken.Object);

            await Assert.ThrowsAsync<MissingResultException>(async () => await accountActions.GetAccountDetailsByContactId(It.IsAny<string>()));
        }
        [Fact]
        public async Task get_account_details_by_contact_id_throws_service_exception_if_api_responds_with_unsuccesfull_code()
        {
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());

            var mockLeaseAccountCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();
            mockLeaseAccountCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(It.IsAny<HttpClient>());

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
         
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.BadGateway) { Content = new StringContent(String.Empty, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<AccountActions>>();

            var mockuhwWarehousRepositoryCall = new Mock<IHackneyUHWWarehouseService>();
         
            var accountActions = new AccountActions(mockILoggerAdapter.Object,
                mockLeaseAccountCallBuilder.Object, mockApiCall.Object, mockuhwWarehousRepositoryCall.Object, mockAccessToken.Object);

            await Assert.ThrowsAsync<ManageATenancyAPI.Actions.AccountServiceException>(async () => await accountActions.GetAccountDetailsByContactId(It.IsAny<string>()));
        }
        #endregion
        private static Dictionary<string, List<JObject>> AccountAddressdictionary()
        {
            var AccountAddress = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["housing_u_saff_rentacc"] = "228003470";
            value["aj_x002e_housing_post_code"] = "N1 6JJ";
            value["aj_x002e_housing_short_address"] = "St Johns Estate  82 Crondall Court";
            value["aj_x002e_addresstypecode"] = 1;
            listJObject.Add(value);
            AccountAddress.Add("value", listJObject);
            return AccountAddress;
        }
        #region AccountDetails
        [Fact]
        public async Task get_account_details_when_paymentreference_does_match()
        {
            var accountDetailsdictionary = AccountDetailsdictionary();
            var resultresponse = await BuildAccountDetailsActualresponse("228009977", accountDetailsdictionary, HttpStatusCode.OK);

            var tenantsList = new List<object>();

            var tenant = new
            {
                personNumber = null as object,
                responsible = false,
                title = "Mr",
                forename = "TestA",
                surname = "TestB",
            };
            tenantsList.Add(tenant);

            var addressList = new List<object>();

            var address = new
            {
                postCode = "E8 2HH",
                shortAddress = "Maurice Bishop House",
                addressTypeCode = "1"
            };

            addressList.Add(address);

            var accountDetails = new
            {
                accountid = "93d621ae-46c6-e711-8111-70106ssssssss",
                tagReferenceNumber = "010000/01",
                benefit = "0.0",
                propertyReferenceNumber = "00000008",
                currentBalance = "564.35",
                rent = "114.04",
                housingReferenceNumber = "010001",
                directdebit = null as object,
                ListOfTenants = tenantsList,
                ListOfAddresses = addressList

            };

            new Mock<ILoggerAdapter<AccountActions>>();
            var accountDetailList = new List<dynamic>();

            accountDetailList.Add(accountDetails);

            var json = new
            {
                results = accountDetailList
            };

            Assert.Equal(JsonConvert.SerializeObject(json), JsonConvert.SerializeObject(resultresponse));
        }

        [Fact]
        public async Task get_account_details_when_tagreference_does_match()
        {
            var accountDetailsdictionary = AccountDetailsdictionary();
            var resultresponse = await BuildAccountDetailsActualresponse("010000/01", accountDetailsdictionary, HttpStatusCode.OK);

            var tenantsList = new List<object>();

            var tenant = new
            {
                personNumber = null as object,
                responsible = false,
                title = "Mr",
                forename = "TestA",
                surname = "TestB",
            };
            tenantsList.Add(tenant);

            var addressList = new List<object>();

            var address = new
            {
                postCode = "E8 2HH",
                shortAddress = "Maurice Bishop House",
                addressTypeCode = "1"
            };

            addressList.Add(address);

            var accountDetails = new
            {
                accountid = "93d621ae-46c6-e711-8111-70106ssssssss",
                tagReferenceNumber = "010000/01",
                benefit = "0.0",
                propertyReferenceNumber = "00000008",
                currentBalance = "564.35",
                rent = "114.04",
                housingReferenceNumber = "010001",
                directdebit = null as object,
                ListOfTenants = tenantsList,
                ListOfAddresses = addressList

            };

            new Mock<ILoggerAdapter<AccountActions>>();
            var accountDetailList = new List<dynamic>();

            accountDetailList.Add(accountDetails);

            var json = new
            {
                results = accountDetailList
            };

            Assert.Equal(JsonConvert.SerializeObject(json), JsonConvert.SerializeObject(resultresponse));
        }


        [Fact]
        public async Task get_account_details_when_no_match_is_found()
        {

            var accountDetailsdicObject = new Dictionary<string, List<JObject>>();
            var listJObject = new List<JObject>();
            var value = new JObject();
            value["accountid"] = "";
            value["housing_tag_ref"] = "";
            value["housing_anticipated"] = "";
            value["housing_prop_ref"] = "";
            value["housing_cur_bal"] = "";
            value["housing_rent"] = "";
            value["housing_house_ref"] = "";
            value["housing_directdebit"] = null;
            value["contact1_x002e_hackney_personno"] = "";
            value["contact1_x002e_hackney_responsible"] = false;
            value["contact1_x002e_hackney_title"] = "";
            value["contact1_x002e_firstname"] = "";
            value["contact1_x002e_lastname"] = "";
            value["contact1_x002e_address1_postalcode"] = "";
            value["contact1_x002e_address1_line3"] = "";
            value["contact1_x002e_address1_line2"] = "";
            value["contact1_x002e_address1_line1"] = "";
            value["customeraddress2_x002e_addresstypecode"] = "";
            listJObject.Add(value);
            accountDetailsdicObject.Add("value", listJObject);

            var tenantsList = new List<object>();

            var tenant = new
            {
                personNumber ="",
                responsible = false,
                title = "",
                forename = "",
                surname = "",
            };
            tenantsList.Add(tenant);

            var addressList = new List<object>();


            var address = new
            {
                postCode = "",
                shortAddress = "  ",
                addressTypeCode = ""
            };

            addressList.Add(address);
            var resultresponse = await BuildAccountDetailsActualresponse("228003470", accountDetailsdicObject, HttpStatusCode.OK);

            var accountDetails = new
            {
                accountid = "",
                tagReferenceNumber = "",
                benefit = "",
                propertyReferenceNumber = "",
                currentBalance = "",
                rent = "",
                housingReferenceNumber = "",
                directdebit = null as object,
                ListOfTenants = tenantsList,
                ListOfAddresses = addressList
            };

            var accountlist = new List<dynamic>();
            accountlist.Add(accountDetails);
            var json = new
            {
                results = accountlist
            };

            Assert.Equal(JsonConvert.SerializeObject(json), JsonConvert.SerializeObject(resultresponse));
        }

        [Fact]
        public async Task get_account_details_throws_missing_result_exception_if_the_service_responds_with_an_error()
        {
            var response = "TokenOnValidRequest";

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockLeaseAccountCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockLeaseAccountCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var query = HousingAPIQueryBuilder.getAccountDetailsByTagorParisReferenceQuery("228003470");
            var mockApiCall = new Mock<IHackneyHousingAPICall>();

            var accountObject = new AccountObject<AccountDetails>();

            var jsonString = JsonConvert.SerializeObject(accountObject);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, "228003470")).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<AccountActions>>();

            var mockuhwWarehousRepositoryCall = new Mock<IHackneyUHWWarehouseService>();
            var accountActions = new AccountActions(mockILoggerAdapter.Object, 
                mockLeaseAccountCallBuilder.Object, mockApiCall.Object, mockuhwWarehousRepositoryCall.Object, mockAccessToken.Object);
        }

        [Fact]
        public async Task get_account_details_throws_service_exception_if_the_service_responds_with_an_error()
        {
            var accountdictionary = AccountAddressdictionary();
            await Assert.ThrowsAsync<ManageATenancyAPI.Actions.AccountServiceException>(async () => await BuildAccountDetailsActualresponse("228003470", accountdictionary, HttpStatusCode.InternalServerError));
        }

        private static async Task<object> BuildAccountDetailsActualresponse(string parisReferencenumber, Dictionary<string, List<JObject>> dictionary, HttpStatusCode httpStatusCode)
        {

            var response = "TokenOnValidRequest";
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var query = HousingAPIQueryBuilder.getAccountDetailsByTagorParisReferenceQuery(parisReferencenumber);

            var jsonString = JsonConvert.SerializeObject(dictionary);
            var responseMessage = new HttpResponseMessage(httpStatusCode)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };
            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, parisReferencenumber)).ReturnsAsync(responseMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<AccountActions>>();

            var mockuhwWarehousRepositoryCall = new Mock<IHackneyUHWWarehouseService>();

            var actions = new AccountActions(mockILoggerAdapter.Object,
              mockApiMockCallBuilder.Object, mockApiCall.Object, mockuhwWarehousRepositoryCall.Object, mockAccessToken.Object);


            var actualresponse = await actions.GetAccountDetailsByParisorTagReference(parisReferencenumber);

            return actualresponse;
        }
        private static Dictionary<string, List<JObject>> AccountDetailsdictionary()
        {
            var accountDetails = new Dictionary<string, List<JObject>>();
            var listJObject = new List<JObject>();
            var value = new JObject();
            value["accountid"] = "93d621ae-46c6-e711-8111-70106ssssssss";
            value["housing_tag_ref"] = "010000/01";
            value["housing_anticipated"] = "0.0";
            value["housing_prop_ref"] = "00000008";
            value["housing_cur_bal"] = "564.35";
            value["housing_rent"] = "114.04";
            value["housing_house_ref"] = "010001";
            value["housing_directdebit"] = null;
            value["contact1_x002e_hackney_personno"] = null;
            value["contact1_x002e_hackney_responsible"] = false;
            value["contact1_x002e_hackney_title"] = "Mr";
            value["contact1_x002e_firstname"] = "TestA";
            value["contact1_x002e_lastname"] = "TestB";
            value["contact1_x002e_address1_postalcode"] = "E8 2HH";
            value["contact1_x002e_address1_line1"] = "Maurice";
            value["contact1_x002e_address1_line2"] = "Bishop";
            value["contact1_x002e_address1_line3"] = "House";
            value["customeraddress2_x002e_addresstypecode"] = "1";
            listJObject.Add(value);
            accountDetails.Add("value", listJObject);
            return accountDetails;
        }
        #endregion

    
        #region GetTagReference
        [Fact]
        public async Task get_tag_reference_when_hackneyhomesId_does_match()
        {
          
            var response = "TokenOnValidRequest";

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();


            var mockILoggerAdapter = new Mock<ILoggerAdapter<AccountActions>>();

            var fakeUhwWarehouseService = new Mock<IHackneyUHWWarehouseService>();

            fakeUhwWarehouseService.Setup(service => service.GetTagReferencenumber("12345")).ReturnsAsync("12345/01");

            var actions = new AccountActions(mockILoggerAdapter.Object,
                mockApiMockCallBuilder.Object, mockApiCall.Object, fakeUhwWarehouseService.Object, mockAccessToken.Object);

            var resultresponse = actions.GetTagReferencenumber("12345").Result;

            var result = "12345/01";

            var tagReferenceList = new List<string> { result };

            var json = new
            {
                results = tagReferenceList
            };

            Assert.Equal(JsonConvert.SerializeObject(json), JsonConvert.SerializeObject(resultresponse));
        }


        [Fact]
        public async Task get_tag_reference_when_hackneyhomesId_when_no_match_is_found()
        {
            var response = "TokenOnValidRequest";
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();


            var mockILoggerAdapter = new Mock<ILoggerAdapter<AccountActions>>();

            var fakeUhwWarehouseService = new Mock<IHackneyUHWWarehouseService>();


            fakeUhwWarehouseService.Setup(service => service.GetTagReferencenumber("12345")).ReturnsAsync("");

            var actions = new AccountActions(mockILoggerAdapter.Object,
                mockApiMockCallBuilder.Object, mockApiCall.Object, fakeUhwWarehouseService.Object, mockAccessToken.Object);

            var resultresponse = actions.GetTagReferencenumber("12345").Result;


            var tagReferenceList = new List<string> { "" };

            var json = new
            {
                results = tagReferenceList
            };

            Assert.Equal(JsonConvert.SerializeObject(json), JsonConvert.SerializeObject(resultresponse));
        }


        #endregion GetTagReference
    }
}

