using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Services;
using Microsoft.AspNetCore.Routing;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ManageATenancyAPI.Tests.Unit.Services
{
    public class OfficerServiceTests
    {
        private readonly Mock<IHackneyHousingAPICall> _mockHousingApiCall;
        private readonly OfficerService _service;

        public OfficerServiceTests()
        {
            _mockHousingApiCall = new Mock<IHackneyHousingAPICall>();
            var mockHousingApiCallBuilder = new Mock<IHackneyHousingAPICallBuilder>();
            var mockCrmToken = new Mock<IHackneyGetCRM365Token>();
            _service = new OfficerService(mockHousingApiCallBuilder.Object, _mockHousingApiCall.Object, mockCrmToken.Object);
        }

        [Fact]
        public async Task GetOfficerDetails_NoSuchHousingOfficer_ReturnsNull()
        {
            var dict = new Dictionary<string, object> {
                { "value", new string[0] }
            };
            var responseObject = JObject.FromObject(dict);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(responseObject.ToString(), System.Text.Encoding.UTF8, "application/json") };
            _mockHousingApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(responseMessage);

            var details = await _service.GetOfficerDetails("nope");

            Assert.Null(details);
        }

        [Fact]
        public async Task GetOfficerDetails_MultipleHousingOfficers_ThrowsInvalidOperationException()
        {
            var results = GetOfficerResponse();
            results.Add(
                new
                {
                    _owningbusinessunit_value = "dd387f80-5b8f-e711-8102-70106faa0331",
                    hackney_emailaddress = "annette.reid@hackney.gov.uk",
                    statecode = 0,
                    statuscode = 1,
                    hackney_lastnewtenancycheckdate = (DateTime?)null,
                    _createdby_value = "e1207267-40a8-e711-810c-70106faa6a11",
                    hackney_name = "Annette Reid",
                    hackney_estateofficerid = "13b5d781-de46-e811-811d-70106faaf8c1",
                    hackney_lastname = "Reid",
                    _ownerid_value = "e1207267-40a8-e711-810c-70106faa6a11",
                    modifiedon = new DateTime(2018,4,23,10,10,11),
                    _owninguser_value = "e1207267-40a8-e711-810c-70106faa6a11",
                    _modifiedby_value = "e1207267-40a8-e711-810c-70106faa6a11",
                    versionnumber = 23669052,
                    hackney_firstname = "Annette",
                    createdon = new DateTime(2018,4,23,10,10,11),
                }
            );
            var dict = new Dictionary<string, object> {
                { "value", results }
            };
            var responseObject = JObject.FromObject(dict);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseObject.ToString(), System.Text.Encoding.UTF8, "application/json") };
            _mockHousingApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(responseMessage);

            async Task act() => await _service.GetOfficerDetails("nope");

            await Assert.ThrowsAsync<InvalidOperationException>(act);
        }

        [Fact]
        public async Task GetOfficerDetails_ValidEmailAddress_ReturnsOfficerDetails()
        {
            var dict = new Dictionary<string, object> {
                { "value", GetOfficerResponse() }
            };
            var responseObject = JObject.FromObject(dict);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseObject.ToString(), System.Text.Encoding.UTF8, "application/json") };
            _mockHousingApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(responseMessage);

            var details = await _service.GetOfficerDetails("yep");

            Assert.IsType<OfficerDetails>(details);
        }

        [Fact]
        public async Task UpdateLastNewTenancyCheckDate_ResponseCodeIsNotSuccessful_ThrowsTenancyServiceException()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            _mockHousingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>()))
                .ReturnsAsync(responseMessage);

            async Task act() => await _service.UpdateLastNewTenancyCheckDate("nope", DateTime.Now);

            await Assert.ThrowsAsync<TenancyServiceException>(act);
        }

        [Fact]
        public async Task GetNewTenanciesForHousingOfficer_ThereAreNewTenancies_ReturnsListOfNewTenancies()
        {
            var dict = new Dictionary<string, object>
            {
                { "value", GetNewTenanciesResponse() }
            };
            var responseObject = JObject.FromObject(dict);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseObject.ToString(), System.Text.Encoding.UTF8, "application/json") };
            _mockHousingApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(responseMessage);

            var updateResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            _mockHousingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>()))
                .ReturnsAsync(updateResponseMessage);

            var officer = GetOfficer();
            officer.LastNewTenancyCheck = null;

            var newTenancies = await _service.GetNewTenanciesForHousingOfficer(officer);

            Assert.IsAssignableFrom<IList<NewTenancyResponse>>(newTenancies);
            //there are 6 new tenancies in the test data
            Assert.True(newTenancies.Count == 6);
        }

        [Fact]
        public async Task GetNewTenanciesForHousingOfficer_ThereAreNoNewTenancies_ReturnsEmptyList()
        {
            var dict = new Dictionary<string, object>
            {
                { "value", new string[0] }
            };
            var responseObject = JObject.FromObject(dict);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseObject.ToString(), System.Text.Encoding.UTF8, "application/json") };
            _mockHousingApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(responseMessage);

            var updateResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            _mockHousingApiCall.Setup(x => x.SendAsJsonAsync(It.IsAny<HttpClient>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<JObject>()))
                .ReturnsAsync(updateResponseMessage);

            var officer = GetOfficer();
            officer.LastNewTenancyCheck = null;

            var newTenancies = await _service.GetNewTenanciesForHousingOfficer(officer);

            Assert.IsAssignableFrom<IList<NewTenancyResponse>>(newTenancies);
            Assert.True(newTenancies.Count == 0);
        }

        private static IList<dynamic> GetOfficerResponse()
        {
            return new List<dynamic>
            {
                new
                {
                    _owningbusinessunit_value= "dd387f80-5b8f-e711-8102-70106faa0331",
                    hackney_emailaddress= "sadelle.agyekum@hackney.gov.uk",
                    statecode= 0,
                    statuscode= 1,
                    hackney_lastnewtenancycheckdate= new DateTime(2019,3,19,10,33,33),
                    _createdby_value= "e1207267-40a8-e711-810c-70106faa6a11",
                    hackney_name= "Sadelle Agyekum",
                    hackney_estateofficerid= "41fdf60c-5efd-e811-a96f-002248072fe8",
                    timezoneruleversionnumber= 4,
                    hackney_lastname= "Agyekum",
                    _ownerid_value= "e1207267-40a8-e711-810c-70106faa6a11",
                    _modifiedby_value= "e1207267-40a8-e711-810c-70106faa6a11",
                    _owninguser_value= "e1207267-40a8-e711-810c-70106faa6a11",
                    createdon= new DateTime(2018,12,11,16,01,52),
                    versionnumber= 48706716,
                    hackney_firstname= "Sadelle",
                    modifiedon= new DateTime(2019,3,19,10,33,48),
                }
            };
        }

        private static OfficerDetails GetOfficer()
        {
            var dict = new RouteValueDictionary(GetOfficerResponse()[0]);
            return OfficerDetails.Create(dict);
        }

        private static IList<dynamic> GetNewTenanciesResponse()
        {
            return new List<dynamic>
            {
                new {
                    housing_tenure= "MPA",
                    accountid= "2d952c17-f83c-e911-a975-002248072781",
                    createdon= DateTime.Parse("2019-03-02T14:33:12Z"),
                    hackney_propertyareapatch2_x002e_hackney_estateaddress= "Harman Estate Stanway Street",
                    contact1_x002e_hackney_responsible= true,
                    estateOfficerId= "41fdf60c-5efd-e811-a96f-002248072fe8",
                    contact1_x002e_hackney_title= "Mr",
                    OfficerFullName= "Sadelle Agyekum",
                    OfficerEmailAddress= "sadelle.agyekum@hackney.gov.uk",
                    OfficerFirstName= "Sadelle",
                    contact1_x002e_firstname= "Jonathan",
                    contact1_x002e_address1_composite= "FLAT 7 JAMES ANDERSON COURT\r\nKINGSLAND ROAD\r\nHACKNEY\r\nLONDON E2 8BE",
                    contact1_x002e_lastname= "Greenwood",
                    OfficerLastName= "Agyekum",
                    contact1_x002e_fullname= "Jonathan Greenwood",
                    contact1_x002e_address1_postalcode= "E2 8BE",
                    hackney_propertyareapatch2_x002e_hackney_neighbourhoodofficedesc= "Shoreditch Neighbourhood",
                    contact1_x002e_address1_line3= "HACKNEY",
                    contact1_x002e_address1_line2= "KINGSLAND ROAD",
                    contact1_x002e_address1_line1= "FLAT 7 JAMES ANDERSON COURT",
                    contact1_x002e_hackney_personno= "1"
                },
                new {
                  housing_tenure= "SEC",
                  accountid= "3d6d7bb8-1c47-e911-a977-002248072781",
                  createdon= DateTime.Parse("2019-03-15T12:20:36Z"),
                  hackney_propertyareapatch2_x002e_hackney_estateaddress= "Colville Estate Felton Street",
                  contact1_x002e_hackney_responsible= true,
                  estateOfficerId= "41fdf60c-5efd-e811-a96f-002248072fe8",
                  contact1_x002e_hackney_title= "Mrs",
                  OfficerFullName= "Sadelle Agyekum",
                  OfficerEmailAddress= "sadelle.agyekum@hackney.gov.uk",
                  OfficerFirstName= "Sadelle",
                  contact1_x002e_firstname= "Dilek",
                  contact1_x002e_address1_composite= "36 Bridport House (New)  Bridport Place (north)\r\nHackney\r\nLondon N1 5DG",
                  contact1_x002e_lastname= "Colak",
                  OfficerLastName= "Agyekum",
                  contact1_x002e_fullname= "Dilek Colak",
                  contact1_x002e_address1_postalcode= "N1 5DG    ",
                  hackney_propertyareapatch2_x002e_hackney_neighbourhoodofficedesc= "Shoreditch Neighbourhood",
                  contact1_x002e_address1_line3= "Hackney",
                  contact1_x002e_address1_line1= "36 Bridport House (New)  Bridport Place (north)",
                  contact1_x002e_hackney_personno= "1"
                },
                new {
                  housing_tenure= "SEC",
                  accountid= "3d6d7bb8-1c47-e911-a977-002248072781",
                  createdon= DateTime.Parse("2019-03-15T12:20:36Z"),
                  hackney_propertyareapatch2_x002e_hackney_estateaddress= "Colville Estate Felton Street",
                  contact1_x002e_hackney_responsible= false,
                  estateOfficerId= "41fdf60c-5efd-e811-a96f-002248072fe8",
                  contact1_x002e_hackney_title= "Mr",
                  OfficerFullName= "Sadelle Agyekum",
                  OfficerEmailAddress= "sadelle.agyekum@hackney.gov.uk",
                  OfficerFirstName= "Sadelle",
                  contact1_x002e_firstname= "Huseyin",
                  contact1_x002e_address1_composite= "36 Bridport House (New)  Bridport Place (north)\r\nHackney\r\nLondon N1 5DG",
                  contact1_x002e_lastname= "Sener",
                  OfficerLastName= "Agyekum",
                  contact1_x002e_fullname= "Huseyin Sener",
                  contact1_x002e_address1_postalcode= "N1 5DG    ",
                  hackney_propertyareapatch2_x002e_hackney_neighbourhoodofficedesc= "Shoreditch Neighbourhood",
                  contact1_x002e_address1_line3= "Hackney",
                  contact1_x002e_address1_line1= "36 Bridport House (New)  Bridport Place (north)",
                  contact1_x002e_hackney_personno= "2"
                },
                new {
                  housing_tenure= "SEC",
                  accountid= "1be70f4c-9436-e911-a972-002248072fe8",
                  createdon= DateTime.Parse("2019-02-22T11:23:47Z"),
                  hackney_propertyareapatch2_x002e_hackney_estateaddress= "Harman Estate Stanway Street",
                  contact1_x002e_hackney_responsible= true,
                  estateOfficerId= "41fdf60c-5efd-e811-a96f-002248072fe8",
                  contact1_x002e_hackney_title= "Mr",
                  OfficerFullName= "Sadelle Agyekum",
                  OfficerEmailAddress= "sadelle.agyekum@hackney.gov.uk",
                  OfficerFirstName= "Sadelle",
                  contact1_x002e_firstname= "Edward Richard",
                  contact1_x002e_address1_composite= "FLAT 38 SARA LANE COURT\r\nSTANWAY STREET\r\nHACKNEY\r\nLONDON N1 6RH",
                  contact1_x002e_lastname= "Jones",
                  OfficerLastName= "Agyekum",
                  contact1_x002e_fullname= "Edward Richard Jones",
                  contact1_x002e_address1_postalcode= "N1 6RH",
                  hackney_propertyareapatch2_x002e_hackney_neighbourhoodofficedesc= "Shoreditch Neighbourhood",
                  contact1_x002e_address1_line3= "HACKNEY",
                  contact1_x002e_address1_line2= "STANWAY STREET",
                  contact1_x002e_address1_line1= "FLAT 38 SARA LANE COURT",
                  contact1_x002e_hackney_personno= "1"
                },
                new {
                  housing_tenure= "SEC",
                  accountid= "65c0a970-c136-e911-a972-002248072fe8",
                  createdon= DateTime.Parse("2019-02-22T16:46:54Z"),
                  hackney_propertyareapatch2_x002e_hackney_estateaddress= "Colville Estate Felton Street",
                  contact1_x002e_hackney_responsible= true,
                  estateOfficerId= "41fdf60c-5efd-e811-a96f-002248072fe8",
                  contact1_x002e_hackney_title= "Miss",
                  OfficerFullName= "Sadelle Agyekum",
                  OfficerEmailAddress= "sadelle.agyekum@hackney.gov.uk",
                  OfficerFirstName= "Sadelle",
                  contact1_x002e_firstname= "Maryori Janed Pacheco",
                  contact1_x002e_address1_composite= "FLAT 30 HIGGINS HOUSE\r\nCOLVILLE ESTATE\r\nHACKNEY\r\nLONDON N1 5PR",
                  contact1_x002e_lastname= "Pacheco Macias",
                  OfficerLastName= "Agyekum",
                  contact1_x002e_fullname= "Maryori Janed Pacheco Pacheco Macias",
                  contact1_x002e_address1_postalcode= "N1 5PR",
                  hackney_propertyareapatch2_x002e_hackney_neighbourhoodofficedesc= "Shoreditch Neighbourhood",
                  contact1_x002e_address1_line3= "HACKNEY",
                  contact1_x002e_address1_line2= "COLVILLE ESTATE",
                  contact1_x002e_address1_line1= "FLAT 30 HIGGINS HOUSE",
                  contact1_x002e_hackney_personno= "1"
                },
                new {
                  housing_tenure= "SEC",
                  accountid= "65c0a970-c136-e911-a972-002248072fe8",
                  createdon= DateTime.Parse("2019-02-22T16:46:54Z"),
                  hackney_propertyareapatch2_x002e_hackney_estateaddress= "Colville Estate Felton Street",
                  contact1_x002e_hackney_responsible= true,
                  estateOfficerId= "41fdf60c-5efd-e811-a96f-002248072fe8",
                  contact1_x002e_hackney_title= "MISS",
                  OfficerFullName= "Sadelle Agyekum",
                  OfficerEmailAddress= "sadelle.agyekum@hackney.gov.uk",
                  OfficerFirstName= "Sadelle",
                  contact1_x002e_firstname= "MARYORI",
                  contact1_x002e_address1_composite= "FLAT 30 HIGGINS HOUSE\r\nCOLVILLE ESTATE\r\nHACKNEY\r\nLONDON N1 5PR",
                  contact1_x002e_lastname= "MACIAS",
                  OfficerLastName= "Agyekum",
                  contact1_x002e_fullname= "MARYORI MACIAS",
                  contact1_x002e_address1_postalcode= "N1 5PR",
                  hackney_propertyareapatch2_x002e_hackney_neighbourhoodofficedesc= "Shoreditch Neighbourhood",
                  contact1_x002e_address1_line3= "HACKNEY",
                  contact1_x002e_address1_line2= "COLVILLE ESTATE",
                  contact1_x002e_address1_line1= "FLAT 30 HIGGINS HOUSE",
                  contact1_x002e_hackney_personno= "1"
                },
                new {
                  housing_tenure= "SEC",
                  accountid= "65c0a970-c136-e911-a972-002248072fe8",
                  createdon= DateTime.Parse("2019-02-22T16:46:54Z"),
                  hackney_propertyareapatch2_x002e_hackney_estateaddress= "Colville Estate Felton Street",
                  contact1_x002e_hackney_responsible= false,
                  estateOfficerId= "41fdf60c-5efd-e811-a96f-002248072fe8",
                  contact1_x002e_hackney_title= "Mr",
                  OfficerFullName= "Sadelle Agyekum",
                  OfficerEmailAddress= "sadelle.agyekum@hackney.gov.uk",
                  OfficerFirstName= "Sadelle",
                  contact1_x002e_firstname= "Alefandro",
                  contact1_x002e_address1_composite= "FLAT 30 HIGGINS HOUSE\r\nCOLVILLE ESTATE\r\nHACKNEY\r\nLONDON N1 5PR",
                  contact1_x002e_lastname= "Garcia Pacheco",
                  OfficerLastName= "Agyekum",
                  contact1_x002e_fullname= "Alefandro Garcia Pacheco",
                  contact1_x002e_address1_postalcode= "N1 5PR",
                  hackney_propertyareapatch2_x002e_hackney_neighbourhoodofficedesc= "Shoreditch Neighbourhood",
                  contact1_x002e_address1_line3= "HACKNEY",
                  contact1_x002e_address1_line2= "COLVILLE ESTATE",
                  contact1_x002e_address1_line1= "FLAT 30 HIGGINS HOUSE",
                  contact1_x002e_hackney_personno= "2"
                },
                new {
                  housing_tenure= "MPA",
                  accountid= "1114a4c0-d236-e911-a972-002248072fe8",
                  createdon= DateTime.Parse("2019-02-22T18:50:51Z"),
                  hackney_propertyareapatch2_x002e_hackney_estateaddress= "Colville Estate Felton Street",
                  contact1_x002e_hackney_responsible= true,
                  estateOfficerId= "41fdf60c-5efd-e811-a96f-002248072fe8",
                  contact1_x002e_hackney_title= "Mr",
                  OfficerFullName= "Sadelle Agyekum",
                  OfficerEmailAddress= "sadelle.agyekum@hackney.gov.uk",
                  OfficerFirstName= "Sadelle",
                  contact1_x002e_firstname= "Serkan",
                  contact1_x002e_address1_composite= "FLAT 13 BRIDPORT HOUSE\r\nBRIDPORT PLACE\r\nHACKNEY\r\nLONDON N1 5DG",
                  contact1_x002e_lastname= "Yeter",
                  OfficerLastName= "Agyekum",
                  contact1_x002e_fullname= "Serkan Yeter",
                  contact1_x002e_address1_postalcode= "N1 5DG",
                  hackney_propertyareapatch2_x002e_hackney_neighbourhoodofficedesc= "Shoreditch Neighbourhood",
                  contact1_x002e_address1_line3= "HACKNEY",
                  contact1_x002e_address1_line2= "BRIDPORT PLACE",
                  contact1_x002e_address1_line1= "FLAT 13 BRIDPORT HOUSE",
                  contact1_x002e_hackney_personno= "1"
                },
                new {
                  housing_tenure= "NON",
                  accountid= "28470685-e436-e911-a972-002248072fe8",
                  createdon= DateTime.Parse("2019-02-22T20:57:58Z"),
                  hackney_propertyareapatch2_x002e_hackney_estateaddress= "Colville Estate Felton Street",
                  contact1_x002e_hackney_responsible= true,
                  estateOfficerId= "41fdf60c-5efd-e811-a96f-002248072fe8",
                  contact1_x002e_hackney_title= "Miss",
                  OfficerFullName= "Sadelle Agyekum",
                  OfficerEmailAddress= "sadelle.agyekum@hackney.gov.uk",
                  OfficerFirstName= "Sadelle",
                  contact1_x002e_firstname= "Claudia Lorena",
                  contact1_x002e_address1_composite= "FLAT 19 ROSEMARY HOUSE\r\nCOLVILLE ESTATE\r\nHACKNEY\r\nLONDON N1 5PL",
                  contact1_x002e_lastname= "Tabares Galvis",
                  OfficerLastName= "Agyekum",
                  contact1_x002e_fullname= "Claudia Lorena Tabares Galvis",
                  contact1_x002e_address1_postalcode= "N1 5PL",
                  hackney_propertyareapatch2_x002e_hackney_neighbourhoodofficedesc= "Shoreditch Neighbourhood",
                  contact1_x002e_address1_line3= "HACKNEY",
                  contact1_x002e_address1_line2= "COLVILLE ESTATE",
                  contact1_x002e_address1_line1= "FLAT 19 ROSEMARY HOUSE",
                  contact1_x002e_hackney_personno= "1"
                },
                new {
                  housing_tenure= "NON",
                  accountid= "28470685-e436-e911-a972-002248072fe8",
                  createdon= DateTime.Parse("2019-02-22T20:57:58Z"),
                  hackney_propertyareapatch2_x002e_hackney_estateaddress= "Colville Estate Felton Street",
                  contact1_x002e_hackney_responsible= false,
                  estateOfficerId= "41fdf60c-5efd-e811-a96f-002248072fe8",
                  contact1_x002e_hackney_title= "Mr",
                  OfficerFullName= "Sadelle Agyekum",
                  OfficerEmailAddress= "sadelle.agyekum@hackney.gov.uk",
                  OfficerFirstName= "Sadelle",
                  contact1_x002e_firstname= "Santiago",
                  contact1_x002e_address1_composite= "FLAT 19 ROSEMARY HOUSE\r\nCOLVILLE ESTATE\r\nHACKNEY\r\nLONDON N1 5PL",
                  contact1_x002e_lastname= "Tabares Galvis",
                  OfficerLastName= "Agyekum",
                  contact1_x002e_fullname= "Santiago Tabares Galvis",
                  contact1_x002e_address1_postalcode= "N1 5PL",
                  hackney_propertyareapatch2_x002e_hackney_neighbourhoodofficedesc= "Shoreditch Neighbourhood",
                  contact1_x002e_address1_line3= "HACKNEY",
                  contact1_x002e_address1_line2= "COLVILLE ESTATE",
                  contact1_x002e_address1_line1= "FLAT 19 ROSEMARY HOUSE",
                  contact1_x002e_hackney_personno= "2"
                },
                new {
                  housing_tenure= "NON",
                  accountid= "28470685-e436-e911-a972-002248072fe8",
                  createdon= DateTime.Parse("2019-02-22T20:57:58Z"),
                  hackney_propertyareapatch2_x002e_hackney_estateaddress= "Colville Estate Felton Street",
                  contact1_x002e_hackney_responsible= false,
                  estateOfficerId= "41fdf60c-5efd-e811-a96f-002248072fe8",
                  contact1_x002e_hackney_title= "Mr",
                  OfficerFullName= "Sadelle Agyekum",
                  OfficerEmailAddress= "sadelle.agyekum@hackney.gov.uk",
                  OfficerFirstName= "Sadelle",
                  contact1_x002e_firstname= "Mateo",
                  contact1_x002e_address1_composite= "FLAT 19 ROSEMARY HOUSE\r\nCOLVILLE ESTATE\r\nHACKNEY\r\nLONDON N1 5PL",
                  contact1_x002e_lastname= "Tabares Perez",
                  OfficerLastName= "Agyekum",
                  contact1_x002e_fullname= "Mateo Tabares Perez",
                  contact1_x002e_address1_postalcode= "N1 5PL",
                  hackney_propertyareapatch2_x002e_hackney_neighbourhoodofficedesc= "Shoreditch Neighbourhood",
                  contact1_x002e_address1_line3= "HACKNEY",
                  contact1_x002e_address1_line2= "COLVILLE ESTATE",
                  contact1_x002e_address1_line1= "FLAT 19 ROSEMARY HOUSE",
                  contact1_x002e_hackney_personno= "3"
                }
            };
        }
    }
}