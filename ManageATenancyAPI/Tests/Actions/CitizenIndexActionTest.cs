using ManageATenancyAPI.Actions;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Tests.Helpers;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using static ManageATenancyAPI.Actions.CitizenIndexAction;
using Moq.Protected;

namespace ManageATenancyAPI.Tests.Actions
{
    public class CitizenIndexActionTest
    {
        [Fact]
        public void should_handle_the_exception_throw_gracefully_when_citizenIndexRepository_SearchCitizenIndex_called()
        {
            var mockLoggerAdapter = new Mock<ILoggerAdapter<CitizenIndexAction>>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);
            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var mockCitizenIndexRepository = new Mock<ICitizenIndexRepository>();

            mockCitizenIndexRepository.Setup(x => x.SearchCitizenIndex(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                     .Throws(new Exception()); 

            var sut = new CitizenIndexAction(mockLoggerAdapter.Object, mockCallBuilder.Object, mockApiCall.Object, mockCitizenIndexRepository.Object,mockAccessToken.Object);

            var addressSearch = $"17 47 BRIDGE HOUSE ADELAIDE AVENUE LADYWELL LONDON LEWISHAM SE4 1LF";

            Assert.ThrowsAsync<Exception>(() => sut.CitizenIndexSearch(It.IsAny<string>(), It.IsAny<string>(), addressSearch, It.IsAny<string>(),false));
        }

        [Fact]
        public void should_handle_the_exception_throw_gracefully_when_getcrmcontacts_housing_api_request()
        {

            var mockILoggerAdapter = new Mock<ILoggerAdapter<CitizenIndexAction>>();
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);
            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), "")).Returns(Task.FromResult(responsMessage));

            var mockCitizenIndexRepository = new Mock<ICitizenIndexRepository>();

            
            Mock<CitizenIndexAction> mockCitizenIndexAction = new Mock<CitizenIndexAction>(mockILoggerAdapter.Object, mockCallBuilder.Object, mockApiCall.Object, mockCitizenIndexRepository.Object, mockAccessToken.Object);

            mockCitizenIndexAction.Protected().Setup<Task<List<CIPerson>>>("GetCRMContacts", ItExpr.IsAny<string>(), ItExpr.IsAny<string>(), ItExpr.IsAny<string[]>(), ItExpr.IsAny<string>());

            var addressSearch = $"17 47 BRIDGE HOUSE ADELAIDE AVENUE LADYWELL LONDON LEWISHAM SE4 1LF";

            Assert.ThrowsAsync<CitizenIndexServiceException>(() => mockCitizenIndexAction.Object.CitizenIndexSearch(It.IsAny<string>(), It.IsAny<string>(), addressSearch, It.IsAny<string>(),false));

        }

        [Fact]
        public async Task should_get_citizen_index_result_when_query_search_is_matched_SqlDatastore()
        {
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            new Mock<ILoggerAdapter<CitizenIndexAction>>();

            string[] searchquery = new[] { "17 47 Bridge House" };
            var query = HousingAPIQueryBuilder.getCRMCitizenSearch("", "", searchquery, "");
            var mockApiCall = new Mock<IHackneyHousingAPICall>();

            var jsonString = JsonConvert.SerializeObject(CitizenIndexResultResponse());
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, It.IsAny<string>())).ReturnsAsync(responsMessage);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<CitizenIndexAction>>();
            
            var mockCitizenIndexRepository = new Mock<ICitizenIndexRepository>();

            mockCitizenIndexRepository.Setup(x => x.SearchCitizenIndex(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                      .Returns(() => citizenIndexresults());


            var sut = new CitizenIndexAction(mockILoggerAdapter.Object, mockCallBuilder.Object, mockApiCall.Object, mockCitizenIndexRepository.Object, mockAccessToken.Object);
            
            var addressSearch = $"17 47 Bridge House";

            var expectedresults = new { results = citizenIndexresults() };

            var actualresponse = await sut.CitizenIndexSearch(It.IsAny<string>(), It.IsAny<string>(), addressSearch, It.IsAny<string>(),true);

            Assert.Equal(JsonConvert.SerializeObject(expectedresults), JsonConvert.SerializeObject(actualresponse));
        }

        [Fact]
        public void should_make_the_call_getcrmcontacts_to_look_up_citizen_exits_in_the_crm_and_merge_citizen_exists_in_sqlDataStore()
        {
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
            var mockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(It.IsAny<string>());
            mockCallBuilder.Setup(x => x.CreateRequest(It.IsAny<string>())).ReturnsAsync(client);

            var mockILoggerAdapter = new Mock<ILoggerAdapter<CitizenIndexAction>>();

            string[] searchquery = new[] { "17 47 Bridge House" };
            var query = HousingAPIQueryBuilder.getCRMCitizenSearch("", "", searchquery, "");
            var mockApiCall = new Mock<IHackneyHousingAPICall>();

            var jsonString = JsonConvert.SerializeObject(CitizenIndexResultResponse());
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, "")).Returns(Task.FromResult(responsMessage));
            var mockCitizenIndexRepository = new Mock<ICitizenIndexRepository>();

            mockCitizenIndexRepository.Setup(x => x.SearchCitizenIndex(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                      .Returns(() => citizenIndexresults());

            Mock<CitizenIndexAction> mockCitizenIndexAction = new Mock<CitizenIndexAction>(mockILoggerAdapter.Object, mockCallBuilder.Object, mockApiCall.Object, mockCitizenIndexRepository.Object, mockAccessToken.Object);


            var returnedData = citizenIndexresults();

            mockCitizenIndexAction.Protected().Setup<Task<List<CIPerson>>>("GetCRMContacts", ItExpr.IsAny<string>(), ItExpr.IsAny<string>(), ItExpr.IsAny<string[]>(), ItExpr.IsAny<string>()).Returns(Task.FromResult(returnedData));

            var addressSearch = $"17 47 BRIDGE HOUSE ADELAIDE AVENUE LADYWELL LONDON LEWISHAM SE4 1LF";

            var mergedcitizen = mockCitizenIndexAction.Object.CitizenIndexSearch(It.IsAny<string>(), It.IsAny<string>(), addressSearch, It.IsAny<string>(),false);

            var expectedresults = new { results = citizenIndexresults()};

            Assert.Equal(JsonConvert.SerializeObject(expectedresults), JsonConvert.SerializeObject(mergedcitizen.Result));

        }

        
        private List<CIPerson> citizenIndexresults()
        {

            return new List<CIPerson> {
                new CIPerson
                {
                ID = null,
                HackneyhomesId = "",
                Title = "MISS",
                Surname = "Testing",
                FirstName = "firstName",
                DateOfBirth = "",
                FullAddressDisplay = "17 47 BRIDGE HOUSE ADELAIDE AVENUE LADYWELL LONDON LEWISHAM SE4 1LF",
                AddressLine1 = "17 47 BRIDGE HOUSE ADELAIDE AVENUE LADYWELL LONDON LEWISHAM SE4 1LF",
                AddressLine2 = "ADELAIDE AVENUE",
                AddressLine3 = "LADYWELL",
                AddressCity = "LONDON",
                AddressCountry = "LEWISHAM",
                PostCode = "SE4 1LF",
                SystemName = "CitizenIndex",
                LARN = "LARN00000000",
                UPRN = "100020000000",
                USN = "800000",
                FullAddressSearch = "1747BRIDGEHOUSEADELAIDEAVENUELADYWELLLONDONLEWISHAMSE41LF"
                }
            };
        }

        private Dictionary<string, object> CitizenIndexResultResponse()
        {
            var citizenIndexResultResponse = new
            {
                ID = "",
                HackneyhomesId = "",
                Title = "MISS",
                Surname = "Testing",
                firstName = "firstName",
                DateOfBirth = "",
                Address = "17 47 BRIDGE HOUSE ADELAIDE AVENUE LADYWELL LONDON LEWISHAM SE4 1LF",
                AddressLine1 = "17 47 BRIDGE HOUSE ADELAIDE AVENUE LADYWELL LONDON LEWISHAM SE4 1LF",
                AddressLine2 = "ADELAIDE AVENUE",
                AddressLine3 = "LADYWELL",
                AddressCity = "LONDON",
                AddressCountry = "LEWISHAM",
                PostCode = "SE4 1LF",
                SystemName = "CitizenIndex",
                LARN = "LARN00000000",
                UPRN = "100020000000",
                USN = "800000",
                FullAddresSearch = "1747BRIDGEHOUSEADELAIDEAVENUELADYWELLLONDONLEWISHAMSE41LF",
                FullName = "Testing firstName"
            };
            List<dynamic> listJObject = new List<dynamic>();

            listJObject.Add(citizenIndexResultResponse);

            var citizenIndexDictionary = new Dictionary<string, object>();
            citizenIndexDictionary.Add("value", listJObject);
            return citizenIndexDictionary;
        }
    }

}
