using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models.Housing;
using ManageATenancyAPI.Tests.Helpers;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ManageATenancyAPI.Tests.Actions.Housing
{
    public class TransactionsActionsTest
    {
        private static readonly Guid transTestGuid = Guid.NewGuid();

        [Fact]
        public async Task get_transactions_when_tag_reference_is_correct()
        {
            //set up token generator
            var response = "TokenOnValidRequest";

            var mocktmiCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mocktmiCallBuilder.Setup(x => x.CreateRequest(response)).ReturnsAsync(client);
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockLeaseAccountTransactionsCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();
            string query = HousingAPIQueryBuilder.getTransactionsByTagReference("000000/01");

            Transactions transactions = new Transactions
            {
                tagReference = "000000/01",
                propertyReference = "00000003",
                transactionSid = "155000000",
                houseReference = "000015",
                transactionType = "RHB",
                postDate = DateTime.Parse("2016-12-08T23:00:00Z"),
                realValue = -121.16,
                transactionID = transTestGuid.ToString(),
                debDesc = ""


            };

            var Transactionsdictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["housing_tag_ref"] = "000000/01";
            value["housing_prop_ref"] = "00000003";
            value["housing_rtrans_sid"] = "155000000";
            value["housing_house_ref"] = "000015";
            value["housing_trans_type"] = "RHB";
            value["housing_postdate"] = DateTime.Parse("2016-12-08T23:00:00Z");
            value["housing_real_value"] = "-121.16";
            value["housing_rtransid"] = transTestGuid;
            value["housing_deb_desc"] = "";
            listJObject.Add(value);

            Transactionsdictionary.Add("value", listJObject);

            string jsonString = JsonConvert.SerializeObject(Transactionsdictionary);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            mockingAPICall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<TransactionsActions>>();
            var transactionsList = new List<Transactions>();
            transactionsList.Add(transactions);
            var json = new
            {
                results = transactionsList
            };
            TransactionsActions transactionsActions = new TransactionsActions(mockILoggerAdapter.Object, mockLeaseAccountTransactionsCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object);
            var resultResponse = await transactionsActions.GetTransactionsByTagReference("000000/01");

            Assert.Equal(JsonConvert.SerializeObject(json), JsonConvert.SerializeObject(resultResponse));
        }

        [Fact]
        public async Task get_transactions_when_no_match_is_found()
        {
            //set up token generator
            string response = "TokenOnValidRequest";

            var mockLeaseAccountTransactionsCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockLeaseAccountTransactionsCallBuilder.Setup(x => x.CreateRequest(response)).ReturnsAsync(client);
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            string query = HousingAPIQueryBuilder.getTransactionsByTagReference("000000/01");

            Transactions transactions = new Transactions
            {
                tagReference = "",
                propertyReference = "",
                transactionSid = "",
                houseReference = "",
                transactionType = "",
                postDate = DateTime.MinValue,
                realValue = 0,
                transactionID = "",
                debDesc = ""


            };

            var Transactionsdictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["housing_tag_ref"] = "";
            value["housing_prop_ref"] = "";
            value["housing_rtrans_sid"] = "";
            value["housing_house_ref"] = "";
            value["housing_trans_type"] = "";
            value["housing_postdate"] = DateTime.MinValue;
            value["housing_real_value"] = 0;
            value["housing_rtransid"] = "";
            value["housing_deb_desc"] = "";
            listJObject.Add(value);
            Transactionsdictionary.Add("value", listJObject);
            string jsonString = JsonConvert.SerializeObject(Transactionsdictionary);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            mockingAPICall.Setup(x => x.getHousingAPIResponse(client, query, "000000/01")).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<TransactionsActions>>();
            var transactionsList = new List<Transactions>();
            transactionsList.Add(transactions);
            var json = new
            {
                results = transactionsList
            };
            TransactionsActions transactionsActions = new TransactionsActions(mockILoggerAdapter.Object, mockLeaseAccountTransactionsCallBuilder.Object, mockingAPICall.Object, mockAccessToken.Object);
            var resultResponse = await transactionsActions.GetTransactionsByTagReference("000000/01");

            Assert.Equal(JsonConvert.SerializeObject(json), JsonConvert.SerializeObject(resultResponse));
        }

        [Fact]
        public async Task get_transactions_throws_missing_result_exception_if_the_service_responds_with_an_error()
        {
            //set up token generator

            string response = "TokenOnValidRequest";

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockLeaseAccountTransactionsCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockLeaseAccountTransactionsCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            string query = HousingAPIQueryBuilder.getTransactionsByTagReference("000000/01");

            TransactionsObject<Transactions> transactions = new TransactionsObject<Transactions>();
            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            string jsonString = JsonConvert.SerializeObject(transactions);
            HttpResponseMessage responsMessage = null;

            mockingAPICall.Setup(x => x.getHousingAPIResponse(client, query, "000000/01")).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<TransactionsActions>>();

            TransactionsActions transactionsActions = new TransactionsActions(mockILoggerAdapter.Object,
                mockLeaseAccountTransactionsCallBuilder.Object,
                mockingAPICall.Object, mockAccessToken.Object);

            await Assert.ThrowsAsync<TransactionsMissingResultsException>(async () => await transactionsActions.GetTransactionsByTagReference("000000/01"));
        }

        [Fact]
        public async Task get_transactions_throws_service_exception_if_the_service_responds_with_an_error()
        {
            //set up token generator

            string response = "TokenOnValidRequest";
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockLeaseAccountTransactionsCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockLeaseAccountTransactionsCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            string query = HousingAPIQueryBuilder.getTransactionsByTagReference("000000/01");

            TransactionsObject<Transactions> transactions = new TransactionsObject<Transactions>();
            var mockingAPICall = new Mock<IHackneyHousingAPICall>();
            string jsonString = JsonConvert.SerializeObject(transactions);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            mockingAPICall.Setup(x => x.getHousingAPIResponse(client, query, "000000/01")).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<TransactionsActions>>();

            TransactionsActions transactionsActions = new TransactionsActions(mockILoggerAdapter.Object,
               mockLeaseAccountTransactionsCallBuilder.Object,
                mockingAPICall.Object, mockAccessToken.Object);


            await Assert.ThrowsAsync<TransactionsServiceException>(async () => await transactionsActions.GetTransactionsByTagReference("000000/01"));
        }

    }
}