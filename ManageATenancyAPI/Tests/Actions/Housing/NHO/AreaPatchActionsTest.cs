using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Tests.Helpers;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using Xunit;
using ManageATenancyAPI.Models.Housing.NHO;
using System.Text;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Tests.Actions.Housing.NHO
{
    public class AreaPatchActionsTest
    {

        [Fact]
        public void should_throw_nullresponseexception_when_housingapiresponse_returns_null_for_the_given_postcode()
        {

            var logger = new Mock<ILoggerAdapter<AreaPatchActions>>();

            var response = "TokenOnValidRequest";

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var query = HousingAPIQueryBuilder.GetAreaPatch(It.IsAny<string>(), It.IsAny<string>());
            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, It.IsAny<string>())).ReturnsAsync(() => null);

            var sut = new AreaPatchActions(logger.Object, mockApiMockCallBuilder.Object, mockApiCall.Object,mockAccessToken.Object);

            var ex = Assert.ThrowsAsync<MissingAreaPatchServiceException>(() => sut.GetAreaPatch(It.IsAny<string>(),It.IsAny<string>()));

        }

        [Fact]
        public void shoud_throw_an_exception_when_httpresponsemessage_issuccessstatuscode_is_false()
        {
            var logger = new Mock<ILoggerAdapter<AreaPatchActions>>();

            var response = "TokenOnValidRequest";

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var query = HousingAPIQueryBuilder.GetAreaPatch(It.IsAny<string>(), It.IsAny<string>());
            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, It.IsAny<string>())).ReturnsAsync(() => httpResponseMessage);


            var sut = new AreaPatchActions(logger.Object, mockApiMockCallBuilder.Object, mockApiCall.Object,mockAccessToken.Object);

            var ex = Assert.ThrowsAsync<AreaPatchServiceException>(() => sut.GetAreaPatch(It.IsAny<string>(),It.IsAny<string>()));

        }


        [Fact]
        public void should_be_able_to_return_the_getareapatch_for_the_successfull_response_of_the_given_postcode()
        {
            var logger = new Mock<ILoggerAdapter<AreaPatchActions>>();

            var response = "TokenOnValidRequest";
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var query = HousingAPIQueryBuilder.GetAreaPatch(It.IsAny<string>(),It.IsAny<string>());
            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, It.IsAny<string>())).ReturnsAsync(() => GetAreaPatchResponseDetail());


            var sut = new AreaPatchActions(logger.Object,  mockApiMockCallBuilder.Object, mockApiCall.Object,mockAccessToken.Object);

            var OutPut = sut.GetAreaPatch(It.IsAny<string>(),It.IsAny<string>());

            var areaPatch = new AreaPatch
            {
                HackneyAreaId = null,
                HackneyareaName = null,
                HackneyPropertyReference = "123",
                HackneyPostCode = "E8111",
                HackneyllpgReference = null,
                HackneyWardId = null,
                HackneyWardName = null,
                HackneyEstateofficerPropertyPatchId = "PatchId123",
                HackneyEstateofficerPropertyPatchName = "Test Officer Last Name",
                HackneyManagerPropertyPatchId = "managerId123",
                HackneyManagerPropertyPatchName = "Test Last Name",
            };
            var expectedObject = new
            {
                result = areaPatch
            };

            Assert.Equal(JsonConvert.SerializeObject(expectedObject), JsonConvert.SerializeObject(OutPut.Result));

        }


        private HttpResponseMessage GetAreaPatchResponseDetail()
        {
            var areaPatchResponseObj = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["hackney_areaname"] = " ";
            value["hackney_areaname@OData.Community.Display.V1.FormattedValue"] = null;
            value["hackney_propertyreference"] = "123";
            value["hackney_postcode"] = "E8111";
            value["hackney_llpgref"] = null;
            value["hackney_ward"] = null;
            value["hackney_ward@OData.Community.Display.V1.FormattedValue"] = null;
            value["_hackney_estateofficerpropertypatchid_value"] = "PatchId123";
            value["_hackney_managerpropertypatchid_value"] = "managerId123";
            value["ManagerFirstName"] = "Test";
            value["ManagerLastName"] = "Last Name";
            value["OfficerFirstName"] = "Test";
            value["OfficerLastName"] = "Officer Last Name";
            listJObject.Add(value);
            areaPatchResponseObj.Add("value", listJObject);

            string jsonString = JsonConvert.SerializeObject(areaPatchResponseObj);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };
            return response;
        }


        #region GETAllOfficersPerArea
        [Fact]
        public async Task get_all_officers_return_valid_object_for_a_valid_request()
        {
            var response = "TokenOnValidRequest";

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var query = HousingAPIQueryBuilder.getAllOfficersPerArea("1");
            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, "1")).ReturnsAsync(getAllOfficersPerArea());

            var logger = new Mock<ILoggerAdapter<AreaPatchActions>>();

            var patchAction = new AreaPatchActions(logger.Object, mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object);

            var responsedata = await patchAction.GetAllOfficersPerArea("1");

            object officers = getAllOfficersPerAreaObject();

            Assert.Equal(JsonConvert.SerializeObject(responsedata), JsonConvert.SerializeObject(officers));
        }

        [Fact]
        public async Task get_all_officers_per_area_raises_exception_if_service_responds_with_null_result()
        {
            var response = "TokenOnValidRequest";
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var query = HousingAPIQueryBuilder.getAllOfficersPerArea("1");


            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, "1")).ReturnsAsync((HttpResponseMessage)null);

            var logger = new Mock<ILoggerAdapter<AreaPatchActions>>();

            var patchAction = new AreaPatchActions(logger.Object,  mockApiMockCallBuilder.Object, mockApiCall.Object,mockAccessToken.Object);

            await Assert.ThrowsAsync<MissingAreaPatchServiceException>(async () => await patchAction.GetAllOfficersPerArea("1"));

        }

        [Fact]
        public async Task get_all_officers_per_area_raises_exception_if_service_responds_with_error()
        {
            
            var response = "TokenOnValidRequest";

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            var query = HousingAPIQueryBuilder.getAllOfficersPerArea("1");

            HttpResponseMessage serviceResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("", System.Text.Encoding.UTF8, "application/json") };
            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, "1")).ReturnsAsync(serviceResponse);

            var logger = new Mock<ILoggerAdapter<AreaPatchActions>>();

            var patchAction = new AreaPatchActions(logger.Object,  mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object);

            await Assert.ThrowsAsync<AreaPatchServiceException>(async () => await patchAction.GetAllOfficersPerArea("1"));

        }


        private HttpResponseMessage getAllOfficersPerArea()
        {
            var officerDictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["hackney_propertyareapatchid"] = "a692a27c-b205-e811-811c-70106wwwwww";
            value["_hackney_estateofficerpropertypatchid_value@OData.Community.Display.V1.FormattedValue"] = "Test Officer Patch";
            value["_hackney_estateofficerpropertypatchid_value"] = "be77dd44-b005-e811-811c-70106aaaaaa";
            value["hackney_name"] = "Central Panel";
            value["hackney_areaname@OData.Community.Display.V1.FormattedValue"] = "Central Panel";
            value["hackney_areaname"] = "1";
            value["hackney_ward@OData.Community.Display.V1.FormattedValue"] = "De Beauvoir";
            value["hackney_ward"] = "1";
            value["hackney_propertyreference"] = "00028225";
            value["_hackney_managerpropertypatchid_value@OData.Community.Display.V1.FormattedValue"] = "Test E G";
            value["_hackney_managerpropertypatchid_value"] = "ae7b4690-b005-e811-811c-70106fffffff";
            value["hackney_llpgref"] = "100021024456";
            value["hackney_estatemanagerarea2_x002e_hackney_managerareaid@OData.Community.Display.V1.FormattedValue"] = "Test G";
            value["hackney_estatemanagerarea2_x002e_hackney_managerareaid"] = "d27c6a5c-da01-e811-8112-70106tttttt";
            value["hackney_estatemanagerarea2_x002e_hackney_name"] = "Test E G";
            value["hackney_estateofficerpatch1_x002e_hackney_patchid@OData.Community.Display.V1.FormattedValue"] = "Test dev";
            value["hackney_estateofficerpatch1_x002e_hackney_patchid"] = "9796145f-b4f7-e711-8112-70106faaaaaa";
            value["hackney_estatemanagerarea2_x002e_hackney_estatemanagerareaid"] = "ae7b4690-b005-e811-811c-70106fffffff";
            value["hackney_estateofficerpatch1_x002e_hackney_name"] = "Test Officer Patch";
            value["hackney_estateofficerpatch1_x002e_hackney_estateofficerpatchid"] = "be77dd44-b005-e811-811c-70106aaaaaa";

            listJObject.Add(value);
            officerDictionary.Add("value", listJObject);

            string jsonString = JsonConvert.SerializeObject(officerDictionary);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };
            return response;
        }

        private object getAllOfficersPerAreaObject()
        {
            var OfficerList = new List<dynamic>();
            dynamic officerDetails = new ExpandoObject();


            officerDetails.propertyAreaPatchId = "";
            officerDetails.estateOfficerPropertyPatchId = "";
            officerDetails.estateOfficerPropertyPatchName = "";
            officerDetails.llpgReferenece = "";
            officerDetails.patchId = "";
            officerDetails.patchName = "";
            officerDetails.propetyReference = "";
            officerDetails.wardName = "";
            officerDetails.wardId = "";
            officerDetails.areaName = "Central Panel";
            officerDetails.areaId = "1";
            officerDetails.managerPropertyPatchId = "ae7b4690-b005-e811-811c-70106fffffff";
            officerDetails.managerPropertyPatchName = "Test E G";
            officerDetails.areaManagerName = "Test G";
            officerDetails.areamanagerId = "d27c6a5c-da01-e811-8112-70106tttttt";
            officerDetails.isaManager = true;
            officerDetails.officerId = "ae7b4690-b005-e811-811c-70106fffffff";
            officerDetails.officerName = "Test G (Area Manager)";
            OfficerList.Add(officerDetails);

            officerDetails = new ExpandoObject();
            officerDetails.propertyAreaPatchId = "a692a27c-b205-e811-811c-70106wwwwww";
            officerDetails.estateOfficerPropertyPatchId = "be77dd44-b005-e811-811c-70106aaaaaa";
            officerDetails.estateOfficerPropertyPatchName = "Test Officer Patch";
            officerDetails.llpgReferenece = "100021024456";
            officerDetails.patchId = "9796145f-b4f7-e711-8112-70106faaaaaa";
            officerDetails.patchName = "Test dev";
            officerDetails.propetyReference = "00028225";
            officerDetails.wardName = "De Beauvoir";
            officerDetails.wardId = "1";
            officerDetails.areaName = "Central Panel";
            officerDetails.areaId = "1";
            officerDetails.managerPropertyPatchId = "ae7b4690-b005-e811-811c-70106fffffff";
            officerDetails.managerPropertyPatchName = "Test E G";
            officerDetails.areaManagerName = "Test G";
            officerDetails.areamanagerId = "d27c6a5c-da01-e811-8112-70106tttttt";
            officerDetails.isaManager = false;
            officerDetails.officerId = "be77dd44-b005-e811-811c-70106aaaaaa";
            officerDetails.officerName = "Test dev";
            OfficerList.Add(officerDetails);
            return new
            {
                results = OfficerList
            };
        }
        #endregion

        #region Update Patch or Area manager
        [Fact]
        public async Task update_patch_or_area_manager_returns_an_updated_object()
        {
            var response = "TokenOnValidRequest";

            var request = new OfficerAreaPatch
            {
                officerId = "be77dd44-b005-e811-811c-7111111",
                patchId = "be77dd44-b005-e811-811c-0000000",
                updatedByOfficer = "be77dd44-b005-e811-811c-22222",
                isUpdatingPatch = true,
                deleteExistingRelationship = false
            };

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            JObject updatePatch = new JObject();

            updatePatch["hackney_estateofficerpatchid"] = "be77dd44-b005-e811-811c-0000000";
            updatePatch["hackney_name"] = "Patch 6-2";

            string jsonString = JsonConvert.SerializeObject(updatePatch);

            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };
            var query = HousingAPIQueryBuilder.updateOfficerAssociatedWithPatch("be77dd44-b005-e811-811c-0000000");
            mockApiCall.Setup(x => x.SendAsJsonAsync(client, new HttpMethod("PATCH"), query, It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            var logger = new Mock<ILoggerAdapter<AreaPatchActions>>();

            var patchAction = new AreaPatchActions(logger.Object,mockApiMockCallBuilder.Object, mockApiCall.Object,mockAccessToken.Object);

            var responsedata = new
            {
                id = "be77dd44-b005-e811-811c-0000000",
                patchName = "Patch 6-2"
            };

            var expected = await patchAction.UpdatePatchOrManager(request);

            Assert.Equal(JsonConvert.SerializeObject(responsedata), JsonConvert.SerializeObject(expected));
        }

        [Fact]
        public async Task update_patch_throws_an_exception_if_service_responds_with_error()
        {
            var response = "TokenOnValidRequest";

            var request = new OfficerAreaPatch
            {
                officerId = "be77dd44-b005-e811-811c-7111111",
                patchId = "be77dd44-b005-e811-811c-0000000",
                updatedByOfficer = "be77dd44-b005-e811-811c-22222",
                isUpdatingPatch = true,
                deleteExistingRelationship = false
            };

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            JObject updatePatch = new JObject();

            updatePatch["hackney_patchid"] = "be77dd44-b005-e811-811c-0000000";
            updatePatch["hackney_name"] = "Patch 6-2";

            string jsonString = JsonConvert.SerializeObject(updatePatch);

            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
            };
            var query = HousingAPIQueryBuilder.updateOfficerAssociatedWithPatch("be77dd44-b005-e811-811c-0000000");
            mockApiCall.Setup(x => x.SendAsJsonAsync(client, new HttpMethod("PATCH"), query, It.IsAny<JObject>())).ReturnsAsync(responsMessage);

            var logger = new Mock<ILoggerAdapter<AreaPatchActions>>();

            var patchAction = new AreaPatchActions(logger.Object, mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object);

            await Assert.ThrowsAsync<UpdatePatchServiceException>(async () =>
                await patchAction.UpdatePatchOrManager(request));
        }

        [Fact]
        public async Task update_patch_throws_a_missing_result_exception_if_service_responds_with_null ()
        {
            var response = "TokenOnValidRequest";

            var request = new OfficerAreaPatch
            {
                officerId = "be77dd44-b005-e811-811c-7111111",
                patchId = "be77dd44-b005-e811-811c-0000000",
                updatedByOfficer = "be77dd44-b005-e811-811c-22222",
                isUpdatingPatch = true,
                deleteExistingRelationship = false
            };

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);
            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();
            
            var query = HousingAPIQueryBuilder.updateOfficerAssociatedWithPatch("be77dd44-b005-e811-811c-0000000");
            mockApiCall.Setup(x => x.SendAsJsonAsync(client, new HttpMethod("PATCH"), query, It.IsAny<JObject>())).ReturnsAsync(() => null);

            var logger = new Mock<ILoggerAdapter<AreaPatchActions>>();

            var patchAction = new AreaPatchActions(logger.Object, mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object);

            await Assert.ThrowsAsync<MissingResultUpdatePatchException>(async () =>
                await patchAction.UpdatePatchOrManager(request));
        }
        #endregion

        #region Get all unassigned officers

        [Fact]
        public async Task get_all_unassigned_officers_returns_a_list_successfully()
        {

            var logger = new Mock<ILoggerAdapter<AreaPatchActions>>();

            var response = "TokenOnValidRequest";
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();

            var dictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["hackney_firstname"] = "Test First Name";
            value["hackney_lastname"] = "Test Last Name";
            value["hackney_name"] = "Test First Name Test Last Name";
            value["hackney_estateofficerid"] = "02345-o0e93o91o-545o902";
           
            listJObject.Add(value);

            dictionary.Add("value", listJObject);

            string jsonString = JsonConvert.SerializeObject(dictionary);
            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            var query = HousingAPIQueryBuilder.getAllOfficersThatAreNotAssignedToPatchOrArea();
            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, It.IsAny<string>())).ReturnsAsync(responseMessage);
            dynamic officersObj = new ExpandoObject();
            officersObj.firstName = "Test First Name";
            officersObj.lastName = "Test Last Name";
            officersObj.fullName = "Test First Name Test Last Name";
            officersObj.officerId = "02345-o0e93o91o-545o902";
            var unassignedOfficersList = new List<dynamic>();
            unassignedOfficersList.Add(officersObj);

            var expected = new
            {
                results = unassignedOfficersList
            };

            var sut = new AreaPatchActions(logger.Object, mockApiMockCallBuilder.Object, mockApiCall.Object, mockAccessToken.Object);

            var actualResult = await sut.GetAllOfficersThatAreNotAssignedToAPatchOrArea();

            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actualResult));

        }

        [Fact]
        public async Task get_all_unassigned_officers_throws_a_missing_result_exception_if_service_responds_with_null()
        {

            var logger = new Mock<ILoggerAdapter<AreaPatchActions>>();

            var response = "TokenOnValidRequest";

            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();


            var query = HousingAPIQueryBuilder.getAllOfficersThatAreNotAssignedToPatchOrArea();
            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, It.IsAny<string>())).ReturnsAsync(() => null);
            dynamic officersObj = new ExpandoObject();
            officersObj.firstName = "Test First Name";
            officersObj.lastName = "Test Last Name";
            officersObj.fullName = "Test First Name Test Last Name";
            officersObj.officerId = "02345-o0e93o91o-545o902";
            var unassignedOfficersList = new List<dynamic>();
            unassignedOfficersList.Add(officersObj);

            var expected = new
            {
                results = unassignedOfficersList
            };

            var sut = new AreaPatchActions(logger.Object, mockApiMockCallBuilder.Object, mockApiCall.Object,mockAccessToken.Object);

            await Assert.ThrowsAsync<MissingResultForGetAllUnassignedOfficersException>(async () => await sut.GetAllOfficersThatAreNotAssignedToAPatchOrArea());
        }

        [Fact]
        public async Task get_all_unassigned_officers_throws_a_service_exception_if_service_responds_with_error()
        {

            var logger = new Mock<ILoggerAdapter<AreaPatchActions>>();

            var response = "TokenOnValidRequest";
            var mockAccessToken = new Mock<IHackneyGetCRM365Token>();
            mockAccessToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(response);

            var mockApiMockCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();

            var client = FakeHackneyHousingAPICallBuilder.createFakeRequest(response);
            mockApiMockCallBuilder.Setup(x => x.CreateRequest("TokenOnValidRequest")).ReturnsAsync(client);

            var mockApiCall = new Mock<IHackneyHousingAPICall>();

            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json") };
            var query = HousingAPIQueryBuilder.getAllOfficersThatAreNotAssignedToPatchOrArea();
            mockApiCall.Setup(x => x.getHousingAPIResponse(client, query, It.IsAny<string>())).ReturnsAsync(responseMessage);
            dynamic officersObj = new ExpandoObject();
            officersObj.firstName = "Test First Name";
            officersObj.lastName = "Test Last Name";
            officersObj.fullName = "Test First Name Test Last Name";
            officersObj.officerId = "02345-o0e93o91o-545o902";
            var unassignedOfficersList = new List<dynamic>();
            unassignedOfficersList.Add(officersObj);

            var expected = new
            {
                results = unassignedOfficersList
            };

            var sut = new AreaPatchActions(logger.Object, mockApiMockCallBuilder.Object, mockApiCall.Object,mockAccessToken.Object);

            await Assert.ThrowsAsync<GetAllUnassignedOfficersServiceException>(async () => await sut.GetAllOfficersThatAreNotAssignedToAPatchOrArea());
        }
        #endregion
    }
}
