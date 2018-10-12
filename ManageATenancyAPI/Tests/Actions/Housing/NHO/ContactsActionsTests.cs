using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Tests.Helpers;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ManageATenancyAPI.Tests.Actions.Housing.NHO
{
    public class ContactsActionsTests
    {
        readonly Guid householdGuid = Guid.NewGuid();
        readonly Guid accountIdGuid = Guid.NewGuid();
       
        #region Create Contact

        [Fact]
        public async Task create_contact_returns_a_create_object()
        {
            var request = new Contact
            {
                LastName = "test last name",
                FirstName = "test first name",
                Email = "test email",
                Address1 = "maurice bishop house",
                City = "london",
                Telephone1 = "0987654321",
                Telephone2 = "0987456321",
                HousingId = "12452",
                CreatedByOfficer = "de98e4b6-15dc-e711-8115-11111111",
                PostCode = "e81hh"

            };
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);


            var ctact = new JObject();

            Guid contactId = Guid.NewGuid();

            ctact["contactid"] = contactId;
            ctact["firstname"] = "test first name";
            ctact["lastname"] = "test last name";
            ctact["fullname"] = "test first name test last name";
            ctact["birthdate"] = "2002-01-15";
            ctact["emailaddress1"] = "test email";
            ctact["address1_line1"] = "maurice bishop house";
            ctact["address1_line2"] = null;
            ctact["address1_line3"] = null;
            ctact["address1_city"] = "london";
            ctact["address1_postalcode"] = "e81hh";
            ctact["telephone1"] = "0987654321";
            ctact["telephone2"] = "0987456321";
            ctact["addresss1_name"] = "maurice bishop house";
            ctact["hackney_larn"] = null;
            ctact["hackney_hackneyhomesid"] = null;
            ctact["hackney_createdby@odata.bind"] = "/hackney_estateofficers(de98e4b6-15dc-e711-8115-11111111)";

            string jsonString = JsonConvert.SerializeObject(ctact);

            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            mockingAPICall.Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<String>(), It.IsAny<JObject>()))
                .ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);
            TestStatus.IsRunningInTests = true;
            var resultResponse = await contactsActions.CreateContact(request);

            var createdContact = new
            {
                contactid = contactId,
                hackneyHomesId = null as object,
                firstName = "test first name",
                lastName = "test last name",
                fullName = "test first name test last name",
                dateOfBirth = "2002-01-15",
                email = "test email",
                address1 = "maurice bishop house",
                address2 = null as object,
                address3 = null as object,
                city = "london",
                postcode = "e81hh",
                telephone1 = "0987654321",
                telephone2 = "0987456321",
                larn = null as object,

            };

            Assert.Equal(JsonConvert.SerializeObject(resultResponse), JsonConvert.SerializeObject(createdContact));

        }


        [Fact]
        public async Task create_contact_throws_missing_result_exception_if_the_service_responds_with_an_error()
        {

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            string query = HousingAPIQueryBuilder.PostContactQuery();

            var ctact = new JObject();
            string jsonString = JsonConvert.SerializeObject(ctact);

            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            mockingAPICall.Setup(x => x.postHousingAPI(client, query, ctact)).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            Contact contact = new Contact();

            await Assert.ThrowsAsync<CreateContactMissingResultException>(async () =>
                await contactsActions.CreateContact(contact));

        }

        [Fact]
        public async Task create_contact_throws_service_exception_if_the_service_responds_with_an_error()
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
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            string query = HousingAPIQueryBuilder.PostContactQuery();

            var ctact = new JObject();
            ctact["hackney_hackneyhomesid"] = null;
            ctact["firstname"] = "test first name";
            ctact["lastname"] = "test last name";
            ctact["fullname"] = "test first name test last name";
            ctact["birthdate"] = "2002-01-15";
            ctact["emailaddress1"] = "test email";
            ctact["address1_line1"] = "maurice bishop house";
            ctact["address1_line2"] = null;
            ctact["address1_line3"] = null;
            ctact["address1_city"] = "london";
            ctact["address1_postalcode"] = "e81hh";
            ctact["address1_name"] = "maurice bishop house";
            ctact["telephone1"] = "0987654321";
            ctact["hackney_larn"] = null;
            ctact["hackney_createdby@odata.bind"] = "/hackney_estateofficers(de98e4b6-15dc-e711-8115-11111111)";

            string jsonString = JsonConvert.SerializeObject(ctact);

            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            mockingAPICall.Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<String>(), It.IsAny<JObject>()))
                .ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<ContactsServiceException>(async () =>
                await contactsActions.CreateContact(request));

        }

        #endregion

        #region Update contact

        [Fact]
        public async Task update_contact_returns_an_updated_object()
        {
            var request = new Contact
            {
                LastName = "test last name",
                FirstName = "test first name",
                Email = "test email",
                UpdatedByOfficer = "e64fee7c-2bba-e711-8106-1111111111"
            };
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);


            var ctact = new JObject();

            ctact["firstname"] = "test first name";
            ctact["lastname"] = "test last name";
            ctact["fullname"] = "test first name test last name";
            ctact["emailaddress1"] = "test email";
            ctact["hackney_updatedby@odata.bind"] = "/hackney_estateofficers(e64fee7c-2bba-e711-8106-1111111111)";

            string jsonString = JsonConvert.SerializeObject(ctact);

            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            mockingAPICall
                .Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var updatedContact = new
            {
                firstName = "test first name",
                lastName = "test last name",
                dateOfBirth = null as object,
                email = "test email",
                address1 = null as object,
                address2 = null as object,
                address3 = null as object,
                city = null as object,
                postcode = null as object,
                telephone1 = null as object,
                telephone2 = null as object,
                telephone3 = null as object
            };

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            var resultResponse =
                await contactsActions.UpdateContact(
                    "10496cc2-97e5-e711-8110-70106faaf8c110496cc2-97e5-e711-8110-70105bbbbbbb", request);

            Assert.Equal(JsonConvert.SerializeObject(resultResponse), JsonConvert.SerializeObject(updatedContact));

        }

        [Fact]
        public async Task update_contact_throws_missing_result_exception_if_the_service_responds_with_an_error()
        {

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            var ctact = new JObject();
            string jsonString = JsonConvert.SerializeObject(ctact);
            string query =
                HousingAPIQueryBuilder.updateContactQuery(
                    "10496cc2-97e5-e711-8110-70106faaf8c110496cc2-97e5-e711-8110-70105bbbbbbb");
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            mockingAPICall.Setup(x => x.SendAsJsonAsync(client, new HttpMethod("PATCH"), query, ctact))
                .ReturnsAsync(responsMessage);
            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            Contact contact = new Contact();

            await Assert.ThrowsAsync<UpdateContactMissingResultException>(async () =>
                await contactsActions.UpdateContact(
                    "10496cc2-97e5-e711-8110-70106faaf8c110496cc2-97e5-e711-8110-70105bbbbbbb", contact));

        }

        [Fact]
        public async Task update_contact_throws_service_exception_if_the_service_responds_with_an_error()
        {
            var request = new Contact
            {
                LastName = "test last name",
                FirstName = "test first name",
                Email = "test email",
                UpdatedByOfficer = "e64fee7c-2bba-e711-8106-1111111111"
            };
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            var ctact = new JObject();
            ctact["firstname"] = "test first name";
            ctact["lastname"] = "test last name";
            ctact["fullname"] = "test first name test last name";
            ctact["emailaddress1"] = "test email";
            ctact["hackney_updatedby@odata.bind"] = "/hackney_estateofficers(e64fee7c-2bba-e711-8106-1111111111)";

            string jsonString = JsonConvert.SerializeObject(ctact);

            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            mockingAPICall
                .Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessage);
            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<ContactsServiceException>(async () =>
                await contactsActions.UpdateContact(
                    "10496cc2-97e5-e711-8110-70106faaf8c110496cc2-97e5-e711-8110-70105bbbbbbb", request));

        }

        #endregion

        #region Get Contact Cautionary Alerts

        [Fact]
        public async Task get_contact_cautionary_alert_when_there_is_a_match()
        {
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            string query = HousingAPIQueryBuilder.getContactCautionaryAlert("1234567");

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            mockingAPICall.Setup(x => x.getHousingAPIResponse(client, query, "1234567"))
                .ReturnsAsync(getCautionaryAlerts());

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            var result = await contactsActions.GetCautionaryAlert("1234567");

            var expected = getContactCautionaryAlertsResult();

            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));

        }


        [Fact]
        public async Task get_contact_cautionary_alert_when_there_is_no_match()
        {
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            string query = HousingAPIQueryBuilder.getContactCautionaryAlert("1234567");

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            mockingAPICall.Setup(x => x.getHousingAPIResponse(client, query, "1234567"))
                .ReturnsAsync(getEmptyCautionaryAlerts());

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            var result = await contactsActions.GetCautionaryAlert("1234567");
            var expected = getEmptyContactCautionaryAlertsResult();


            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));

        }

        [Fact]
        public async Task get_contact_cautionary_alert_throws_an_exception_when_service_responds_with_error()
        {
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            string query = HousingAPIQueryBuilder.getContactCautionaryAlert("1234567");

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
            };

            mockingAPICall.Setup(x => x.getHousingAPIResponse(client, query, "1234567")).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);


            await Assert.ThrowsAsync<ContactsServiceException>(async () =>
                await contactsActions.GetCautionaryAlert("1234567"));
        }


        [Fact]
        public async Task get_contact_cautionary_alert_throws_an_exception_when_response_is_empty()
        {
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            string query = HousingAPIQueryBuilder.getContactCautionaryAlert("1234567");

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();

            mockingAPICall.Setup(x => x.getHousingAPIResponse(client, query, "1234567")).ReturnsAsync(() => null);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<GetCautionaryAlertMissingResultException>(async () =>
                await contactsActions.GetCautionaryAlert("1234567"));
        }

        private object getEmptyContactCautionaryAlertsResult()
        {
            var cautionaryAlertList = new List<dynamic>();
            dynamic cautionaryAlertObject = new ExpandoObject();
            cautionaryAlertObject.cautionaryAlertType = "";
            cautionaryAlertObject.cautionaryAlertId = "";
            cautionaryAlertObject.contactId = "";
            cautionaryAlertObject.contactName = "";
            cautionaryAlertObject.uprn = "";
            cautionaryAlertObject.createdOn = null;
            cautionaryAlertList.Add(cautionaryAlertObject);
            return new
            {
                results = cautionaryAlertList
            };
        }

        private object getContactCautionaryAlertsResult()
        {
            var cautionaryAlertList = new List<dynamic>();
            dynamic cautionaryAlertObject = new ExpandoObject();
            cautionaryAlertObject.cautionaryAlertType = "Dangerous dog";
            cautionaryAlertObject.cautionaryAlertId = "alertId1234567890";
            cautionaryAlertObject.contactId = "1b2b3b4b5b6b0bo08";
            cautionaryAlertObject.contactName = "Test name";
            cautionaryAlertObject.uprn = "1234567";
            cautionaryAlertObject.createdOn = "2016-12-08 23:00:00";
            cautionaryAlertList.Add(cautionaryAlertObject);
            return new
            {
                results = cautionaryAlertList
            };
        }

        private HttpResponseMessage getCautionaryAlerts()
        {
            var cautionaryAlertdictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["hackney_cautionaryalerttype@OData.Community.Display.V1.FormattedValue"] = "Dangerous dog";
            value["hackney_cautionaryalertid"] = "alertId1234567890";
            value["_hackney_contactid_value"] = "1b2b3b4b5b6b0bo08";
            value["_hackney_contactid_value@OData.Community.Display.V1.FormattedValue"] = "Test name";
            value["hackney_uprn"] = "1234567";
            value["createdon"] = DateTime.Parse("2016-12-08T23:00:00Z");
            listJObject.Add(value);

            cautionaryAlertdictionary.Add("value", listJObject);

            string jsonString = JsonConvert.SerializeObject(cautionaryAlertdictionary);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };

            return responsMessage;
        }

        private HttpResponseMessage getEmptyCautionaryAlerts()
        {
            var cautionaryAlertdictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["hackney_cautionaryalerttype@OData.Community.Display.V1.FormattedValue"] = "";
            value["hackney_cautionaryalertid"] = "";
            value["_hackney_contactid_value"] = "";
            value["_hackney_contactid_value@OData.Community.Display.V1.FormattedValue"] = "";
            value["hackney_uprn"] = "";
            value["createdon"] = null;
            listJObject.Add(value);

            cautionaryAlertdictionary.Add("value", listJObject);

            string jsonString = JsonConvert.SerializeObject(cautionaryAlertdictionary);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };

            return responsMessage;
        }

        #endregion

        #region Remove Cautionary Alert

        [Fact]
        public async Task remove_cautionary_alert_returns_successful_response()
        {
            var request = new CautionaryAlert
            {
                cautionaryAlertIds = new List<string>
                {
                    "12345667-8d23-e811-8120-70106f568811",
                    "12345677-8e23-e811-8120-70106f5687811",
                    "23456788-8e23-e811-8120-70106f5678181"
                },
                contactId = "o999993-e716-e811-811e-788888aa6a11",
                uprn = "1000089925",
                cautionaryAlertIsToBeRemoved = false
            };
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            var alertsDictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var firstContact = new JObject();

            firstContact["contactId"] = "newContactId12345";
            firstContact["hackney_cautionaryalert"] = true;
            firstContact["hackney_propertycautionaryalert"] = true;
            listJObject.Add(firstContact);
            var secondContact = new JObject();
            secondContact["contactId"] = "newContactId5678";
            secondContact["hackney_cautionaryalert"] = false;
            secondContact["hackney_propertycautionaryalert"] = true;

            listJObject.Add(secondContact);

            alertsDictionary.Add("value", listJObject);


            string jsonString = JsonConvert.SerializeObject(alertsDictionary);

            HttpResponseMessage responsMessageForDeleteAndUpdateApiCall =
                new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                };
            HttpResponseMessage responsMessageForGetContactsApiCall = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };
            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            //set up contact update response
            mockingAPICall
                .Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessageForDeleteAndUpdateApiCall);
            //set up delete response
            mockingAPICall
                .Setup(x => x.deleteObjectAPIResponse(It.IsAny<HttpClient>(), It.IsAny<String>()))
                .ReturnsAsync(responsMessageForDeleteAndUpdateApiCall);
            //set up get contacts response
            mockingAPICall
                .Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<String>(), It.IsAny<String>()))
                .ReturnsAsync(responsMessageForGetContactsApiCall);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            var resultResponse =
                await contactsActions.RemoveCautionaryAlerts(request);

            Assert.Equal(JsonConvert.SerializeObject(resultResponse),
                JsonConvert.SerializeObject(responsMessageForDeleteAndUpdateApiCall.StatusCode));

        }


        [Fact]
        public async Task remove_cautionary_alert_throws_missing_result_exception_if_delete_call_responds_with_null()
        {
            var request = new CautionaryAlert
            {
                cautionaryAlertIds = new List<string>
                {
                    "12345667-8d23-e811-8120-70106f568811",
                    "12345677-8e23-e811-8120-70106f5687811",
                    "23456788-8e23-e811-8120-70106f5678181"
                },
                contactId = "o999993-e716-e811-811e-788888aa6a11",
                uprn = "1000089925",
                cautionaryAlertIsToBeRemoved = false
            };
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            var alertsDictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var firstContact = new JObject();

            firstContact["contactId"] = "newContactId12345";
            firstContact["hackney_cautionaryalert"] = true;
            firstContact["hackney_propertycautionaryalert"] = true;
            listJObject.Add(firstContact);
            var secondContact = new JObject();
            secondContact["contactId"] = "newContactId5678";
            secondContact["hackney_cautionaryalert"] = false;
            secondContact["hackney_propertycautionaryalert"] = true;

            listJObject.Add(secondContact);

            alertsDictionary.Add("value", listJObject);


            string jsonString = JsonConvert.SerializeObject(alertsDictionary);

            HttpResponseMessage responsMessageForDeleteAndUpdateApiCall =
                new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                };
            HttpResponseMessage responsMessageForGetContactsApiCall = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };
            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            //set up contact update response
            mockingAPICall
                .Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessageForDeleteAndUpdateApiCall);
            //set up delete response
            mockingAPICall
                .Setup(x => x.deleteObjectAPIResponse(It.IsAny<HttpClient>(), It.IsAny<String>()))
                .ReturnsAsync(() => null);
            //set up get contacts response
            mockingAPICall
                .Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<String>(), It.IsAny<String>()))
                .ReturnsAsync(responsMessageForGetContactsApiCall);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<RemoveCautionaryAlertMissingResultException>(async () =>
                await contactsActions.RemoveCautionaryAlerts(request));

        }

        [Fact]
        public async Task
            remove_cautionary_alert_throws_missing_result_exception_if_get_contacts_call_responds_with_null()
        {
            var request = new CautionaryAlert
            {
                cautionaryAlertIds = new List<string>
                {
                    "12345667-8d23-e811-8120-70106f568811",
                    "12345677-8e23-e811-8120-70106f5687811",
                    "23456788-8e23-e811-8120-70106f5678181"
                },
                contactId = "o999993-e716-e811-811e-788888aa6a11",
                uprn = "1000089925",
                cautionaryAlertIsToBeRemoved = false
            };
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            HttpResponseMessage responsMessageForDeleteAndUpdateApiCall =
                new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                };

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            //set up contact update response
            mockingAPICall
                .Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessageForDeleteAndUpdateApiCall);
            //set up delete response
            mockingAPICall
                .Setup(x => x.deleteObjectAPIResponse(It.IsAny<HttpClient>(), It.IsAny<String>()))
                .ReturnsAsync(responsMessageForDeleteAndUpdateApiCall);
            //set up get contacts response
            mockingAPICall
                .Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<String>(), It.IsAny<String>()))
                .ReturnsAsync(() => null);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<RemoveCautionaryAlertMissingResultException>(async () =>
                await contactsActions.RemoveCautionaryAlerts(request));

        }

        [Fact]
        public async Task
            remove_cautionary_alert_throws_exception_if_server_responds_with_exception_when_calling_get_contacts_api()
        {
            var request = new CautionaryAlert
            {
                cautionaryAlertIds = new List<string>
                {
                    "12345667-8d23-e811-8120-70106f568811",
                    "12345677-8e23-e811-8120-70106f5687811",
                    "23456788-8e23-e811-8120-70106f5678181"
                },
                contactId = "o999993-e716-e811-811e-788888aa6a11",
                uprn = "1000089925",
                cautionaryAlertIsToBeRemoved = false
            };
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            HttpResponseMessage responsMessageForDeleteAndUpdateApiCall =
                new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                };

            HttpResponseMessage responsMessageForGetContactsApiCall = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
            };

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            //set up contact update response
            mockingAPICall
                .Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessageForDeleteAndUpdateApiCall);
            //set up delete response
            mockingAPICall
                .Setup(x => x.deleteObjectAPIResponse(It.IsAny<HttpClient>(), It.IsAny<String>()))
                .ReturnsAsync(responsMessageForDeleteAndUpdateApiCall);
            //set up get contacts response
            mockingAPICall
                .Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<String>(), It.IsAny<String>()))
                .ReturnsAsync(responsMessageForGetContactsApiCall);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();


            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);


            await Assert.ThrowsAsync<RemoveCautionaryAlertServiceException>(async () =>
                await contactsActions.RemoveCautionaryAlerts(request));

        }

        [Fact]
        public async Task
            remove_cautionary_alert_throws_exception_if_server_responds_with_exception_when_calling_update_or_delete_api()
        {
            var request = new CautionaryAlert
            {
                cautionaryAlertIds = new List<string>
                {
                    "12345667-8d23-e811-8120-70106f568811",
                    "12345677-8e23-e811-8120-70106f5687811",
                    "23456788-8e23-e811-8120-70106f5678181"
                },
                contactId = "o999993-e716-e811-811e-788888aa6a11",
                uprn = "1000089925",
                cautionaryAlertIsToBeRemoved = false
            };
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            var alertsDictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var firstContact = new JObject();

            firstContact["contactId"] = "newContactId12345";
            firstContact["hackney_cautionaryalert"] = true;
            firstContact["hackney_propertycautionaryalert"] = true;
            listJObject.Add(firstContact);
            var secondContact = new JObject();
            secondContact["contactId"] = "newContactId5678";
            secondContact["hackney_cautionaryalert"] = false;
            secondContact["hackney_propertycautionaryalert"] = true;

            listJObject.Add(secondContact);

            alertsDictionary.Add("value", listJObject);


            string jsonString = JsonConvert.SerializeObject(alertsDictionary);

            HttpResponseMessage responsMessageForDeleteAndUpdateApiCall =
                new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                };

            HttpResponseMessage responsMessageForGetContactsApiCall = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            //set up contact update response
            mockingAPICall
                .Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessageForDeleteAndUpdateApiCall);
            //set up delete response
            mockingAPICall
                .Setup(x => x.deleteObjectAPIResponse(It.IsAny<HttpClient>(), It.IsAny<String>()))
                .ReturnsAsync(responsMessageForDeleteAndUpdateApiCall);
            //set up get contacts response
            mockingAPICall
                .Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<String>(), It.IsAny<String>()))
                .ReturnsAsync(responsMessageForGetContactsApiCall);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);


            await Assert.ThrowsAsync<RemoveCautionaryAlertServiceException>(async () =>
                await contactsActions.RemoveCautionaryAlerts(request));

        }

        #endregion

        #region Create Cautionary Alert
        [Fact]
        public async Task create_cautionary_alert_returns_created_object()
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
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            JObject postApiResponse = new JObject();
            postApiResponse["hackney_cautionaryalerttype"] = "3";
            postApiResponse["_hackney_contactid_value"] = "o999993-e716-e811-811e-788888aa6a11";
            postApiResponse["hackney_uprn"] = "1000089925";
            postApiResponse["hackney_cautionaryalertid"] = "123993-e716-e811-811e-788888aa6a";

            string jsonString = JsonConvert.SerializeObject(postApiResponse);

            HttpResponseMessage responsMessageForPostAlertApi = new HttpResponseMessage(HttpStatusCode.NoContent)
            {
                Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
            };

            var alertsDictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var firstContact = new JObject();

            firstContact["contactid"] = "newContactId12345";
            firstContact["hackney_cautionaryalert"] = true;
            firstContact["hackney_propertycautionaryalert"] = true;
            listJObject.Add(firstContact);
            var secondContact = new JObject();
            secondContact["contactid"] = "newContactId5678";
            secondContact["hackney_cautionaryalert"] = false;
            secondContact["hackney_propertycautionaryalert"] = true;

            listJObject.Add(secondContact);

            alertsDictionary.Add("value", listJObject);


            string jsonStringGetContacts = JsonConvert.SerializeObject(alertsDictionary);

            HttpResponseMessage responsMessageForGetContactsApiCall = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonStringGetContacts, System.Text.Encoding.UTF8, "application/json")
            };

            HttpResponseMessage responsMessageForUpdateApiCall =
                new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                };
            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            //set up create alert response
            mockingAPICall
                .Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessageForPostAlertApi);

            //set up get contacts response
            mockingAPICall
                .Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<String>(), It.IsAny<String>()))
                .ReturnsAsync(responsMessageForGetContactsApiCall);
            //set up contact update response
            mockingAPICall
                .Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessageForUpdateApiCall);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            //  mockConfig.SetupGet<ConnStringConfiguration>(x => x.Value).Returns("the string you want to return");
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());

            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            var resultResponse =
                await contactsActions.CreateCautionaryAlerts(request);

            var expectedResponse = new
            {
                alertContactId = "o999993-e716-e811-811e-788888aa6a11",
                alertUprn = "1000089925",
                alertCautionaryAlertType = new List<string> { "3" },
                createdOn = DateTime.Today.ToString("yyyy-MM-dd")
            };
            Assert.Equal(JsonConvert.SerializeObject(resultResponse),
                JsonConvert.SerializeObject(expectedResponse));

        }


        [Fact]
        public async Task create_cautionary_alert_throws_missing_result_exception_if_postAPI_returns_null()
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
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);


            var alertsDictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var firstContact = new JObject();

            firstContact["contactId"] = "newContactId12345";
            firstContact["hackney_cautionaryalert"] = true;
            firstContact["hackney_propertycautionaryalert"] = true;
            listJObject.Add(firstContact);
            var secondContact = new JObject();
            secondContact["contactId"] = "newContactId5678";
            secondContact["hackney_cautionaryalert"] = false;
            secondContact["hackney_propertycautionaryalert"] = true;

            listJObject.Add(secondContact);

            alertsDictionary.Add("value", listJObject);


            string jsonStringGetContacts = JsonConvert.SerializeObject(alertsDictionary);

            HttpResponseMessage responsMessageForGetContactsApiCall = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonStringGetContacts, System.Text.Encoding.UTF8, "application/json")
            };

            HttpResponseMessage responsMessageForUpdateApiCall =
                new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                };
            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            //set up create alert response
            mockingAPICall
                .Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(() => null);

            //set up get contacts response
            mockingAPICall
                .Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<String>(), It.IsAny<String>()))
                .ReturnsAsync(responsMessageForGetContactsApiCall);
            //set up contact update response
            mockingAPICall
                .Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessageForUpdateApiCall);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);


            await Assert.ThrowsAsync<CreateCautionaryAlertMissingResultException>(async () =>
                await contactsActions.CreateCautionaryAlerts(request));

        }

        [Fact]
        public async Task create_cautionary_alert_throws_missing_result_exception_if_get_contacts_api_returns_null()
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
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            HttpResponseMessage responsMessageForPostAlertApi = new HttpResponseMessage(HttpStatusCode.NoContent)
            {
                Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
            };

            HttpResponseMessage responsMessageForUpdateApiCall =
                new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                };
            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            //set up create alert response
            mockingAPICall
                .Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessageForPostAlertApi);

            //set up get contacts response
            mockingAPICall
                .Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<String>(), It.IsAny<String>()))
                .ReturnsAsync(() => null);
            //set up contact update response
            mockingAPICall
                .Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessageForUpdateApiCall);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);



            await Assert.ThrowsAsync<CreateCautionaryAlertMissingResultException>(async () =>
                await contactsActions.CreateCautionaryAlerts(request));

        }

        [Fact]
        public async Task create_cautionary_alert_throws_service_exception_if_postAPI_responds_with_error()
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
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);


            var alertsDictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var firstContact = new JObject();

            firstContact["contactId"] = "newContactId12345";
            firstContact["hackney_cautionaryalert"] = true;
            firstContact["hackney_propertycautionaryalert"] = true;
            listJObject.Add(firstContact);
            var secondContact = new JObject();
            secondContact["contactId"] = "newContactId5678";
            secondContact["hackney_cautionaryalert"] = false;
            secondContact["hackney_propertycautionaryalert"] = true;

            listJObject.Add(secondContact);

            alertsDictionary.Add("value", listJObject);


            string jsonStringGetContacts = JsonConvert.SerializeObject(alertsDictionary);

            HttpResponseMessage responsMessageForGetContactsApiCall = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonStringGetContacts, System.Text.Encoding.UTF8, "application/json")
            };
            HttpResponseMessage responsMessageForPostAlertApi = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
            };

            HttpResponseMessage responsMessageForUpdateApiCall =
                new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                };
            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            //set up create alert response
            mockingAPICall
                .Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessageForPostAlertApi);

            //set up get contacts response
            mockingAPICall
                .Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<String>(), It.IsAny<String>()))
                .ReturnsAsync(responsMessageForGetContactsApiCall);
            //set up contact update response
            mockingAPICall
                .Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessageForUpdateApiCall);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<CreateCautionaryAlertServiceException>(async () =>
                await contactsActions.CreateCautionaryAlerts(request));

        }


        [Fact]
        public async Task create_cautionary_alert_throws_service_exception_if_get_contacts_api_responds_with_error()
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
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            var alertsDictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var firstContact = new JObject();

            firstContact["contactId"] = "newContactId12345";
            firstContact["hackney_cautionaryalert"] = true;
            firstContact["hackney_propertycautionaryalert"] = true;
            listJObject.Add(firstContact);
            var secondContact = new JObject();
            secondContact["contactId"] = "newContactId5678";
            secondContact["hackney_cautionaryalert"] = false;
            secondContact["hackney_propertycautionaryalert"] = true;

            listJObject.Add(secondContact);

            alertsDictionary.Add("value", listJObject);


            string jsonStringGetContacts = JsonConvert.SerializeObject(alertsDictionary);

            HttpResponseMessage responsMessageForGetContactsApiCall = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(jsonStringGetContacts, System.Text.Encoding.UTF8, "application/json")
            };
            HttpResponseMessage responsMessageForPostAlertApi = new HttpResponseMessage(HttpStatusCode.NoContent)
            {
                Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
            };

            HttpResponseMessage responsMessageForUpdateApiCall =
                new HttpResponseMessage(HttpStatusCode.NoContent)
                {
                    Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                };
            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            //set up create alert response
            mockingAPICall
                .Setup(x => x.postHousingAPI(It.IsAny<HttpClient>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessageForPostAlertApi);

            //set up get contacts response
            mockingAPICall
                .Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<String>(), It.IsAny<String>()))
                .ReturnsAsync(responsMessageForGetContactsApiCall);
            //set up contact update response
            mockingAPICall
                .Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessageForUpdateApiCall);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);


            await Assert.ThrowsAsync<CreateCautionaryAlertServiceException>(async () =>
                await contactsActions.CreateCautionaryAlerts(request));

        }
        #endregion

        #region Get Contacts by UPRN

        [Fact]
        public async Task get_contact_by_uprn_when_there_is_a_match()
        {
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            string query = HousingAPIQueryBuilder.GetAllContactsandDetailsByUprn("1234567");

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            mockingAPICall.Setup(x => x.getHousingAPIResponse(client, query, "1234567"))
                .ReturnsAsync(getcontacts());

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            var result = await contactsActions.GetContactsByUprn("1234567");

            var expected = getContactsByUprn();

            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));

        }


        [Fact]
        public async Task get_contact_by_uprn_throws_an_exception_when_service_responds_with_error()
        {

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());

            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            string query = HousingAPIQueryBuilder.GetAllContactsandDetailsByUprn("1234567");

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
            };

            mockingAPICall.Setup(x => x.getHousingAPIResponse(client, query, "1234567")).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<ContactsServiceException>(async () =>
                await contactsActions.GetContactsByUprn("1234567"));
        }

        private object getContactsByUprn()
        {
            var contactList = new List<dynamic>();
            dynamic contactObj = new ExpandoObject();

            contactObj.contactId = "123456";
            contactObj.emailAddress = "test@test.com";
            contactObj.uprn = "1234567";
            contactObj.addressLine1 = "Test Address Line 1";
            contactObj.addressLine2 = "Test Address Line 2";
            contactObj.addressLine3 = "Test Address Line 3";
            contactObj.firstName = "Test";
            contactObj.lastName = "Test 1";
            contactObj.fullName = "Test Test 1";
            contactObj.larn = "LARN 1234567";
            contactObj.telephone1 = "98765678967";
            contactObj.telephone2 = "";
            contactObj.telephone3 = "";
            contactObj.cautionaryAlert = "yes";
            contactObj.propertyCautionaryAlert = "No";
            contactObj.houseRef = "ref123";
            contactObj.title = "Miss";
            contactObj.fullAddressDisplay = "Test Address Line 1 Test Address Line 2\r\nTest Address Line 3\r\nLONDON\r\nE14 9LH";
            contactObj.fullAddressSearch = "Test Address Line 1 Test Address Line 2 Test Address Line 3 LONDON E14 9LH";
            contactObj.postCode = "E14 9LH";
            contactObj.dateOfBirth = "1986-08-15";
            contactObj.hackneyHomesId = "hackneyHomes12";
            contactObj.disabled = null;
            contactObj.relationship = null;
            contactObj.extendedrelationship = null;
            contactObj.responsible = null;
            contactObj.age = null;
            contactList.Add(contactObj);
            return new
            {
                results = contactList
            };
        }

        private HttpResponseMessage getcontacts()
        {
            var contactdictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["contactid"] = "123456";
            value["emailaddress1"] = "test@test.com";
            value["hackney_uprn"] = "1234567";
            value["address1_line1"] = "Test Address Line 1";
            value["address1_line2"] = "Test Address Line 2";
            value["address1_line3"] = "Test Address Line 3";
            value["firstname"] = "Test";
            value["lastname"] = "Test 1";
            value["fullname"] = "Test Test 1";
            value["hackney_larn"] = "LARN 1234567";
            value["telephone1"] = "98765678967";
            value["telephone2"] = "";
            value["telephone3"] = "";
            value["hackney_cautionaryalert"] = "yes";
            value["hackney_propertycautionaryalert"] = "No";
            value["housing_house_ref"] = "ref123";
            value["hackney_title"] = "Miss";
            value["address1_composite"] = "Test Address Line 1 Test Address Line 2\r\nTest Address Line 3\r\nLONDON\r\nE14 9LH";
            value["address1_name"] = "Test Address Line 1 Test Address Line 2 Test Address Line 3 LONDON E14 9LH";
            value["address1_postalcode"] = "E14 9LH";
            value["birthdate"] = "1986-08-15";
            value["hackney_hackneyhomesid"] = "hackneyHomes12";

            listJObject.Add(value);

            contactdictionary.Add("value", listJObject);

            string jsonString = JsonConvert.SerializeObject(contactdictionary);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };

            return responsMessage;
        }
        #endregion

        #region Get Contact Details by Contact ID

        [Fact]
        public async Task get_contact_by_contact_id_when_there_is_a_match()
        {
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);
            
            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            mockingAPICall.Setup(x => x.getHousingAPIResponse(client, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(getContactDetailsByContactID());

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            var result = await contactsActions.GetContactDetailsByContactId(It.IsAny<string>());

            Assert.Equal(JsonConvert.SerializeObject(getContactDetailsByContactIDobject()), JsonConvert.SerializeObject(result));
        }
        [Fact]
        public async Task get_contact_by_contact_id_returns_empty_object_when_no_result_is_found()
        {
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
            };
            mockingAPICall.Setup(x => x.getHousingAPIResponse(client, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            var result = await contactsActions.GetContactDetailsByContactId(It.IsAny<string>());
           
            Assert.Equal(JsonConvert.SerializeObject(null), JsonConvert.SerializeObject(result));
        }


        [Fact]
        public async Task get_contact_by_contact_id_throws_an_exception_when_service_responds_with_error()
        {

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());

            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);
         
            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
            };

            mockingAPICall.Setup(x => x.getHousingAPIResponse(client, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<GetContactDetailsServiceException>(async () =>
                await contactsActions.GetContactDetailsByContactId(It.IsAny<string>()));
        }

        [Fact]
        public async Task get_contact_by_contact_id_throws_an_exception_when_service_responds_with_missing_result()
        {

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());

            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
           
            mockingAPICall.Setup(x => x.getHousingAPIResponse(client, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => null);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            await Assert.ThrowsAsync<GetContactDetailsMissingResultException>(async () =>
                await contactsActions.GetContactDetailsByContactId(It.IsAny<string>()));
        }

        private HttpResponseMessage getContactDetailsByContactID()
        {
          
         
            var value = new JObject();
            value["contactid"] = "123456";
            value["emailaddress1"] = "test@test.com";
            value["hackney_uprn"] = "1234567";
            value["address1_line1"] = "Test Address Line 1";
            value["address1_line2"] = "Test Address Line 2";
            value["address1_line3"] = "Test Address Line 3";
            value["firstname"] = "Test";
            value["lastname"] = "Test 1";
            value["hackney_larn"] = "LARN 1234567";
            value["telephone1"] = "98765678967";
            value["telephone2"] = null;
            value["telephone3"] = null;
            value["hackney_cautionaryalert"] = true;
            value["hackney_propertycautionaryalert"] = true;
            value["housing_house_ref"] = "ref123";
            value["hackney_title"] = "Miss";
            value["address1_composite"] = "Test Address Line 1 Test Address Line 2\r\nTest Address Line 3\r\nLONDON\r\nE14 9LH";
            value["address1_name"] = "Test Address Line 1 Test Address Line 2 Test Address Line 3 LONDON E14 9LH";
            value["address1_postalcode"] = "E14 9LH";
            value["birthdate"] = "1986-03-05";
            value["hackney_hackneyhomesid"] = "hackneyHomes12";
            value["_hackney_household_contactid_value"] = householdGuid;
            value["hackney_membersid"] = "membersId";
            value["hackney_personno"] = "1";
            value["_parentcustomerid_value"] = accountIdGuid;
            value["hackney_nextofkinname"] = "Test name";
            value["hackney_nextofkinaddress"] = "Test address";
            value["hackney_nextofkinrelationship"] = "test relationship";
            value["hackney_nextofkinotherphone"] = "123456";
            value["hackney_nextofkinemail"] = "email address";
            value["hackney_nextofkinmobile"] = "1234555";
        

            string jsonString = JsonConvert.SerializeObject(value);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };

            return responsMessage;
        }

        private object getContactDetailsByContactIDobject()
        {
            return new
            {
                contactId = "123456",
                emailAddress = "test@test.com",
                uprn = "1234567",
                addressLine1 = "Test Address Line 1",
                addressLine2 = "Test Address Line 2",
                addressLine3 = "Test Address Line 3",
                firstName = "Test",
                lastName = "Test 1",
                larn = "LARN 1234567",
                address1AddressId = null as object,
                address2AddressId = null as object,
                address3AddressId = null as object,
                telephone1 = "98765678967",
                telephone2 = null as object,
                telephone3 = null as object,
                cautionaryAlert = true,
                propertyCautionaryAlert = true,
                houseRef = "ref123",
                title = "Miss",
                fullAddressDisplay = "Test Address Line 1 Test Address Line 2\r\nTest Address Line 3\r\nLONDON\r\nE14 9LH",
                fullAddressSearch = "Test Address Line 1 Test Address Line 2 Test Address Line 3 LONDON E14 9LH",
                postCode = "E14 9LH",
                dateOfBirth = "1986-03-05",
                hackneyHomesId = "hackneyHomes12",
                houseHoldId = householdGuid,
                memberId = "membersId",
                personno = "1",
                accountId = accountIdGuid,
                nextOfKinName = "Test name",
                nextOfKinAddress = "Test address",
                nextOfKinRelationship = "test relationship",
                nextOfKinOtherPhone = "123456",
                nextOfKinEmail = "email address",
                nextOfKinMobile = "1234555"
            };
        }
        #endregion


        #region UpdateNextOfKin 
        [Fact]
        public async Task update_next_of_kin_returns_an_updated_object()
        {
            var contactID = Guid.NewGuid();

            var request = new NextOfKin
            {
                nextOfKinName = "test name",
                nextOfKinAddress = "test address",
                nextOfKinEmail = "test email",
                nextOfKinMobile = "0123456789",
                nextOfKinOtherTelehone = null,
                nextOfKinRelationship = "sister",
                contactID = contactID
            };

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            var nextOfKin = new JObject();
            nextOfKin["hackney_nextofkinname"] = "test name";
            nextOfKin["hackney_nextofkinaddress"] = "test address";
            nextOfKin["hackney_nextofkinrelationship"] = "sister";
            nextOfKin["hackney_nextofkinotherphone"] = null;
            nextOfKin["hackney_nextofkinemail"] = "test email";
            nextOfKin["hackney_nextofkinmobile"] = "0123456789";

            string jsonString = JsonConvert.SerializeObject(nextOfKin);

            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            mockingAPICall
                .Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<String>(),
                    It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();

            var updatedNextOfKin = new
            {
                nextOfKinName = "test name",
                nextOfKinAddress = "test address",
                nextOfKinRelationship = "sister",
                nextOfKinOtherPhone = null as object,
                nextOfKinEmail = "test email",
                nextOfKinMobile = "0123456789"
            };

            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());

            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            var resultResponse = await contactsActions.UpdateNextOfKin(request);

            Assert.Equal(JsonConvert.SerializeObject(resultResponse), JsonConvert.SerializeObject(updatedNextOfKin));

        }
        [Fact]
        public async Task update_next_of_kin_throws_missing_result_exception_if_the_service_responds_with_empty_result()
        {

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            var ctact = new JObject();
            string jsonString = JsonConvert.SerializeObject(ctact);

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            mockingAPICall.Setup(x => x.SendAsJsonAsync(client, new HttpMethod("PATCH"), It.IsAny<string>(), ctact))
                .ReturnsAsync(() => null);
            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            NextOfKin nextOfKin = new NextOfKin();

            await Assert.ThrowsAsync<UpdateNextOfKinMissingResultException>(async () =>
                await contactsActions.UpdateNextOfKin(nextOfKin));
        }
        [Fact]
        public async Task update_next_of_kin_throws_service_exception_if_the_service_responds_with_an_error()
        {

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            var ctact = new JObject();
            string jsonString = JsonConvert.SerializeObject(ctact);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };
            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            mockingAPICall.Setup(x => x.SendAsJsonAsync(client, new HttpMethod("PATCH"), It.IsAny<string>(), ctact))
                .ReturnsAsync(responsMessage);
            var mockILoggerAdapter = new Mock<ILoggerAdapter<ContactsActions>>();
            var mockConfig = new Mock<IOptions<ConnStringConfiguration>>();
            mockConfig.SetupGet(x => x.Value).Returns(new ConnStringConfiguration());
            ContactsActions contactsActions = new ContactsActions(mockILoggerAdapter.Object, mockCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object, mockConfig.Object);

            NextOfKin nextOfKin = new NextOfKin();

            await Assert.ThrowsAsync<ContactsServiceException>(async () =>
                await contactsActions.UpdateNextOfKin(nextOfKin));
        }
        #endregion
    }
}