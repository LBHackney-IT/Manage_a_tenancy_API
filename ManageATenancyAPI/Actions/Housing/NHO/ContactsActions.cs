using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Tests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyPropertyAccountAPI.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public class ContactsActions
    {
        private HttpClient _client;
        private IHackneyGetCRM365Token _accessToken;
        private readonly ConnStringConfiguration _configuration;
        private readonly IHackneyHousingAPICallBuilder _apiCallBuilder;
        private readonly ILoggerAdapter<ContactsActions> _loggerAdapter;
        private IHackneyHousingAPICall _hackneyContactstApi;
        private IHackneyHousingAPICallBuilder _hackneyContactsApiBuilder;
        private static string _strCIUploadsConnString;
        public ContactsActions(ILoggerAdapter<ContactsActions> logger, IHackneyHousingAPICallBuilder apiCallBuilder,
            IHackneyHousingAPICall apiCall, IHackneyGetCRM365Token accessToken, IOptions<ConnStringConfiguration> config)
        {

            _client = new HttpClient();
            _accessToken = accessToken;
            _hackneyContactsApiBuilder = apiCallBuilder;
            _hackneyContactstApi = apiCall;
            _loggerAdapter = logger;
            _configuration = config?.Value;
            _strCIUploadsConnString = _configuration.CIUploads;
        }
        public async Task<object> CreateContact(Contact contact)
        {
            JObject createdContact = null;

            try
            {
                _loggerAdapter.LogInformation($"Saving new contact to CRM {contact}  ");


                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _client = _hackneyContactsApiBuilder.CreateRequest(accessToken).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "return=representation");

                string query =
                    HousingAPIQueryBuilder.PostContactQuery();

                JObject ctact = new JObject();
                if (contact.CrMcontactId != Guid.Empty)
                {
                    ctact["contactid"] = contact.CrMcontactId;
                }
                ctact["hackney_hackneyhomesid"] = contact.HousingId;
                ctact["firstname"] = contact.FirstName;
                ctact["lastname"] = contact.LastName;
                ctact["fullname"] = contact.FullName;
                ctact["birthdate"] = contact.DateOfBirth?.ToString($"yyyy-MM-dd").Trim();
                ctact["emailaddress1"] = contact.Email;
                ctact["telephone1"] = contact.Telephone1;
                ctact["telephone2"] = contact.Telephone2;
                ctact["hackney_larn"] = contact.LARN;
                ctact["hackney_createdby@odata.bind"] = "/hackney_estateofficers(" + contact.CreatedByOfficer + ")";
                ctact["address1_line1"] = contact.Address1;
                ctact["address1_line2"] = contact.Address2;
                ctact["address1_line3"] = contact.Address3;
                ctact["address1_city"] = contact.City;
                ctact["address1_postalcode"] = contact.PostCode;
                ctact["hackney_uprn"] = contact.UPRN;
                ctact["address1_composite"] = contact.FullAddressDisplay; //this will change after LLPG API is changed to return seperate fields

                //This field will be use for CRM 365 address search
                //if(string.IsNullOrEmpty(contact.Address1) && string.IsNullOrEmpty(contact.Address1)
                ctact["address1_name"] = contact.FullAddressSearch;

                var createResponse = _hackneyContactstApi.postHousingAPI(_client, query, ctact).Result;

                if (createResponse != null)
                {
                    if (!createResponse.IsSuccessStatusCode)
                    {
                        throw new ContactsServiceException();
                    }
                    //the contactid gets returned in the Location header of the response. This is because when posting to CRM, 
                    //sends 204 "No Content" status code and does not return any data. 
                    createdContact =
                        JsonConvert.DeserializeObject<JObject>(await createResponse.Content.ReadAsStringAsync());
                    var contactid = createdContact["contactid"];

                    if (contact.CrMcontactId == Guid.Empty)
                    {
                        contact.CrMcontactId = Guid.Parse(contactid.ToString());
                        if (!TestStatus.IsRunningInTests)
                        {
                            UploadToCIStaging(contact);
                        }
                    }

                    return new
                    {
                        contactid = createdContact["contactid"],
                        hackneyHomesId = createdContact["hackney_hackneyhomesid"],
                        firstName = createdContact["firstname"],
                        lastName = createdContact["lastname"],
                        fullName = createdContact["fullname"],
                        dateOfBirth = createdContact["birthdate"],
                        email = createdContact["emailaddress1"],
                        address1 = createdContact["address1_line1"],
                        address2 = createdContact["address1_line2"],
                        address3 = createdContact["address1_line3"],
                        city = createdContact["address1_city"],
                        postcode = createdContact["address1_postalcode"],
                        telephone1 = createdContact["telephone1"],
                        telephone2 = createdContact["telephone2"],
                        larn = createdContact["hackney_larn"],
                    };
                }
                else
                {
                    _loggerAdapter.LogError($" Account could not be created  {contact} ");
                    throw new CreateContactMissingResultException();
                }
                //log successful response

            }
            catch (Exception e)
            {
                //log error response
                throw e;
            }
        }

        public async Task<object> UpdateContact(string id, Contact contact)
        {
            JObject updatedContact = null;
            try
            {
                _loggerAdapter.LogInformation($"Updating contact with id {id} in CRM  ");


                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _client = _hackneyContactsApiBuilder.CreateRequest(accessToken).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "return=representation");

                string query =
                    HousingAPIQueryBuilder.updateContactQuery(id);

                JObject contactJObject = new JObject();
                if (contact.FirstName != null)
                {
                    contactJObject["firstname"] = contact.FirstName;
                }
                if (contact.LastName != null)
                {
                    contactJObject["lastname"] = contact.LastName;

                }
                if (contact.FirstName != null && contact.LastName != null)
                {
                    contactJObject["fullname"] = contact.FullName;
                }
                if (contact.DateOfBirth != DateTime.MinValue)
                {
                    contactJObject["birthdate"] = contact.DateOfBirth?.ToString($"yyyy-MM-dd").Trim();
                }
                if (contact.Email != null)
                {
                    contactJObject["emailaddress1"] = contact.Email;
                }
                if (contact.Address1 != null)
                {
                    contactJObject["address1_line1"] = contact.Address1;
                }
                if (contact.Address2 != null)
                {
                    contactJObject["address1_line2"] = contact.Address2;
                }
                if (contact.Address3 != null)
                {
                    contactJObject["address1_line3"] = contact.Address3;
                }
                if (contact.City != null)
                {
                    contactJObject["address1_city"] = contact.City;
                }
                if (contact.PostCode != null)
                {
                    contactJObject["address1_postalcode"] = contact.PostCode;
                }
                if (contact.Telephone1 != null)
                {
                    contactJObject["telephone1"] = contact.Telephone1;
                }
                if (contact.Telephone2 != null)
                {
                    contactJObject["telephone2"] = contact.Telephone2;
                }
                if (contact.Telephone3 != null)
                {
                    contactJObject["telephone3"] = contact.Telephone3;
                }
                contactJObject["hackney_updatedby@odata.bind"] =
                    "/hackney_estateofficers(" + contact.UpdatedByOfficer + ")";

                var createResponse =
                    _hackneyContactstApi.SendAsJsonAsync(_client, new HttpMethod("PATCH"), query, contactJObject)
                        .Result;

                // UploadToCIStaging(crmContact);  Uncomment when going Live. Should we update CI when CRM contact gets updated?
                if (createResponse != null)
                {
                    if (!createResponse.IsSuccessStatusCode)
                    {
                        throw new ContactsServiceException();
                    }
                    updatedContact =
                         JsonConvert.DeserializeObject<JObject>(await createResponse.Content.ReadAsStringAsync());
                    return new
                    {
                        firstName = updatedContact["firstname"],
                        lastName = updatedContact["lastname"],
                        dateOfBirth = updatedContact["birthdate"],
                        email = updatedContact["emailaddress1"],
                        address1 = updatedContact["address1_line1"],
                        address2 = updatedContact["address1_line2"],
                        address3 = updatedContact["address1_line3"],
                        city = updatedContact["address1_city"],
                        postcode = updatedContact["address1_postalcode"],
                        telephone1 = updatedContact["telephone1"],
                        telephone2 = updatedContact["telephone2"],
                        telephone3 = updatedContact["telephone3"]
                    };
                }
                else
                {
                    _loggerAdapter.LogError($" Account with id {id} could not be updated");
                    throw new UpdateContactMissingResultException();
                }
            }
            catch (Exception e)
            {
                //log error response
                throw e;
            }

        }

        public async Task<object> GetCautionaryAlert(string uprn)
        {
            HttpResponseMessage result = null;
            try
            {

                _loggerAdapter.LogInformation($"Getting cautionary alerts for URPN {uprn}  ");


                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _client = _hackneyContactsApiBuilder.CreateRequest(accessToken).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");

                string query =
                    HousingAPIQueryBuilder.getContactCautionaryAlert(uprn);

                result = _hackneyContactstApi.getHousingAPIResponse(_client, query, uprn).Result;
                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new ContactsServiceException();
                    }
                    JObject jRetrieveResponse =
                        JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                    if (jRetrieveResponse != null)
                    {

                        var getCautionaryAlertyResponse = jRetrieveResponse["value"].ToList();

                        return new
                        {
                            results = prepareCautionaryAlertResultObject(getCautionaryAlertyResponse)
                        };

                    }
                    return null;
                }
                else
                {
                    _loggerAdapter.LogError($" Data missing for cautionary alerts for contact with urpn  {uprn} ");
                    throw new GetCautionaryAlertMissingResultException();
                }
            }
            catch (Exception ex)
            {
                _loggerAdapter.LogError($" GetContactCautionaryAlert error method name -  GetCautionaryAlert()" + ex.Message);
                throw;
            }
        }


        public async Task<object> RemoveCautionaryAlerts([FromBody] CautionaryAlert cautionaryAlert)
        {
            //Used when user is trying to update cautionary alert. Updating means remove any existing and create new ones. 
            HttpResponseMessage createResponse = null;
            try
            {
                _loggerAdapter.LogInformation($"Deleting cautionary alerts for contact with id {cautionaryAlert.contactId} ");


                var accessToken = _accessToken.getCRM365AccessToken().Result;
                _client = _hackneyContactsApiBuilder.CreateRequest(accessToken).Result;

                foreach (string cautionaryAlertId in cautionaryAlert.cautionaryAlertIds)
                {
                    _loggerAdapter.LogInformation($"Deleting cautionary alert with id {cautionaryAlertId} ");
                    string query =
                        HousingAPIQueryBuilder.deleteCautionaryAlert(cautionaryAlertId);

                    createResponse = _hackneyContactstApi.deleteObjectAPIResponse(_client, query).Result;

                    if (createResponse != null)
                    {
                        if (!createResponse.IsSuccessStatusCode)
                        {
                            throw new RemoveCautionaryAlertServiceException();
                        }
                    }
                    else
                    {
                        _loggerAdapter.LogError(
                            $"Cautionary alert with id {cautionaryAlertId} could not be deleted ");
                        throw new RemoveCautionaryAlertMissingResultException();
                    }
                }
            }
            catch (Exception ex)
            {
                //log error response
                _loggerAdapter.LogError(
                    $"Remove cautionary alert threw exception {ex} while deleting existing cautionary alert records ");
                throw ex;
            }

            /*Remove contact's main cautionary alert flag and check if anyone else in the property has a cautionary alert to determine whether "propertyCautionaryAlert" flag is to be
             set to false or not */
            try
            {
                HttpResponseMessage result = null;

                string queryForGettingAllContactsByUprn =
                    HousingAPIQueryBuilder.getAllContactsByUprn(cautionaryAlert.uprn);


                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _client = _hackneyContactsApiBuilder.CreateRequest(accessToken).Result;

                result = _hackneyContactstApi
                    .getHousingAPIResponse(_client, queryForGettingAllContactsByUprn, cautionaryAlert.uprn).Result;

                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        _loggerAdapter.LogError(
                            $"Error while getting contacts by UPRN {cautionaryAlert.uprn} ");
                        throw new RemoveCautionaryAlertServiceException();

                    }
                    JObject jRetrieveResponse =
                        JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                    if (jRetrieveResponse != null)
                    {
                        List<JToken> contactsInTheSameProperty = jRetrieveResponse["value"].ToList();

                        //check if someone in the property has a cautionary alert against their name
                        int countOfContactsWithCautionaryAlerts = 0;
                        foreach (dynamic contact in contactsInTheSameProperty)
                        {
                            if (contact["hackney_cautionaryalert"] != null)
                            {
                                if ((bool)contact["hackney_cautionaryalert"])
                                {
                                    countOfContactsWithCautionaryAlerts++;
                                }
                            }
                        }
                        //if there is another person within the property who has a cautionary alert against their name and contact has no other cautionary alerts against their name
                        if (countOfContactsWithCautionaryAlerts > 1 && cautionaryAlert.cautionaryAlertIsToBeRemoved)
                        {
                            JObject contactJObject = new JObject();
                            contactJObject["hackney_cautionaryalert"] = false;
                            string updateContactsquery =
                                HousingAPIQueryBuilder.updateContactQuery(cautionaryAlert.contactId);
                            result =
                                _hackneyContactstApi.SendAsJsonAsync(_client, new HttpMethod("PATCH"),
                                        updateContactsquery, contactJObject)
                                    .Result;
                            if (!result.IsSuccessStatusCode)
                            {
                                _loggerAdapter.LogError(
                                    $"Error while updating field 'hackney_cautionaryalert' for contact with id {cautionaryAlert.contactId}");
                                throw new RemoveCautionaryAlertServiceException();
                            }


                        }
                        else if (countOfContactsWithCautionaryAlerts == 1 && cautionaryAlert.cautionaryAlertIsToBeRemoved)
                        { //flow for removing "propertycautionaryalert" flag if no one else in the property 
                            foreach (var contact in contactsInTheSameProperty)
                            {
                                JObject contactJObject = new JObject();
                                if (contact["contactid"].ToString() == cautionaryAlert.contactId)
                                {
                                    //update contact, who is assigned a cautionary alert
                                    contactJObject["hackney_propertycautionaryalert"] = false;
                                    contactJObject["hackney_cautionaryalert"] = false;

                                    string updateContactsquery =
                                        HousingAPIQueryBuilder.updateContactQuery(contact["contactid"]
                                            .ToString());
                                    result =
                                        _hackneyContactstApi.SendAsJsonAsync(_client, new HttpMethod("PATCH"),
                                                updateContactsquery, contactJObject)
                                            .Result;
                                }
                                else
                                {
                                    //update the other people in the property
                                    contactJObject["hackney_propertycautionaryalert"] = false;
                                    string updateContactsquery =
                                        HousingAPIQueryBuilder.updateContactQuery(contact["contactid"]
                                            .ToString());
                                    result =
                                        _hackneyContactstApi.SendAsJsonAsync(_client, new HttpMethod("PATCH"),
                                                updateContactsquery, contactJObject)
                                            .Result;
                                }

                                if (!result.IsSuccessStatusCode)
                                {
                                    _loggerAdapter.LogError(
                                            $"Error while updating field 'hackney_cautionaryalert' and 'hackney_propertycautionaryalert' for contact {contact}");
                                    throw new RemoveCautionaryAlertServiceException();
                                }
                            }
                        }
                    }

                }
                else
                {
                    _loggerAdapter.LogError(
                        $" Data missing for cautionary alerts for contact with urpn  {cautionaryAlert.uprn} ");
                    throw new RemoveCautionaryAlertMissingResultException();
                }

            }
            catch (Exception ex)
            {
                _loggerAdapter.LogError($"Error occurred during the execution of RemoveCautionaryAlert() " +
                                        ex.Message);
                throw;
            }

            return createResponse.StatusCode;

        }

        public async Task<object> GetContactDetailsByContactId(string contactId)
        {
            HttpResponseMessage result = null;
            try
            {

                _loggerAdapter.LogInformation($"Getting contact details for contact with id {contactId}  ");


                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _client = _hackneyContactsApiBuilder.CreateRequest(accessToken).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");

                string query =
                    HousingAPIQueryBuilder.GetContactDetailsByContactId(contactId);

                result = _hackneyContactstApi.getHousingAPIResponse(_client, query, contactId).Result;
                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new GetContactDetailsServiceException();
                    }
                    JObject jRetrieveResponse =
                        JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                    if (jRetrieveResponse != null)
                    {

                        return new
                        {
                            contactId = jRetrieveResponse["contactid"],
                            emailAddress = jRetrieveResponse["emailaddress1"],
                            uprn = jRetrieveResponse["hackney_uprn"],
                            addressLine1 = jRetrieveResponse["address1_line1"],
                            addressLine2 = jRetrieveResponse["address1_line2"],
                            addressLine3 = jRetrieveResponse["address1_line3"],
                            firstName = jRetrieveResponse["firstname"],
                            lastName = jRetrieveResponse["lastname"],
                            larn = jRetrieveResponse["hackney_larn"],
                            address1AddressId = jRetrieveResponse["address1_addressid"],
                            address2AddressId = jRetrieveResponse["address2_addressid"],
                            address3AddressId = jRetrieveResponse["address3_addressid"],
                            telephone1 = jRetrieveResponse["telephone1"],
                            telephone2 = jRetrieveResponse["telephone2"],
                            telephone3 = jRetrieveResponse["telephone3"],
                            cautionaryAlert = jRetrieveResponse["hackney_cautionaryalert"],
                            propertyCautionaryAlert = jRetrieveResponse["hackney_propertycautionaryalert"],
                            houseRef = jRetrieveResponse["housing_house_ref"],
                            title = jRetrieveResponse["hackney_title"],
                            fullAddressDisplay = jRetrieveResponse["address1_composite"],
                            fullAddressSearch = jRetrieveResponse["address1_name"],
                            postCode = jRetrieveResponse["address1_postalcode"],
                            dateOfBirth = jRetrieveResponse["birthdate"],
                            hackneyHomesId = jRetrieveResponse["hackney_hackneyhomesid"],
                            houseHoldId = jRetrieveResponse["_hackney_household_contactid_value"],
                            memberId = jRetrieveResponse["hackney_membersid"],
                            personno = jRetrieveResponse["hackney_personno"],
                            accountId = jRetrieveResponse["_parentcustomerid_value"],
                            nextOfKinName = jRetrieveResponse["hackney_nextofkinname"],
                            nextOfKinAddress = jRetrieveResponse["hackney_nextofkinaddress"],
                            nextOfKinRelationship = jRetrieveResponse["hackney_nextofkinrelationship"],
                            nextOfKinOtherPhone = jRetrieveResponse["hackney_nextofkinotherphone"],
                            nextOfKinEmail = jRetrieveResponse["hackney_nextofkinemail"],
                            nextOfKinMobile = jRetrieveResponse["hackney_nextofkinmobile"]
                        };

                    }
                    return null;
                }
                else
                {
                    _loggerAdapter.LogError($" Data missing for contact with id {contactId} ");
                    throw new GetContactDetailsMissingResultException();
                }
            }
            catch (Exception ex)
            {
                _loggerAdapter.LogError($" GetContactDetailsByContactId error method name -  GetContactDetailsByContactId()" + ex.Message);
                throw;
            }
        }

        public async Task<object> CreateCautionaryAlerts([FromBody] CautionaryAlert cautionaryAlert)
        {
            JObject createdAlert = null;
            //create the cautionary alert
            try
            {
                _loggerAdapter.LogInformation($"Creating a new cautionary alert for contact id {cautionaryAlert.contactId} with uprn {cautionaryAlert.uprn} ");

                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _client = _hackneyContactsApiBuilder.CreateRequest(accessToken).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "return=representation");

                string query =
                    HousingAPIQueryBuilder.postCautionaryAlert();


                foreach (string cautionaryAlertProperty in cautionaryAlert.cautionaryAlertType)
                {
                    JObject cautionaryAlertJObject = new JObject();
                    cautionaryAlertJObject["hackney_cautionaryalerttype"] = cautionaryAlertProperty;
                    cautionaryAlertJObject["hackney_contactid@odata.bind"] =
                        "/contacts(" + cautionaryAlert.contactId + ")";
                    cautionaryAlertJObject["hackney_uprn"] = cautionaryAlert.uprn;


                    var createResponse = _hackneyContactstApi.postHousingAPI(_client, query, cautionaryAlertJObject)
                        .Result;

                    if (createResponse != null)
                    {
                        if (!createResponse.IsSuccessStatusCode)
                        {
                            _loggerAdapter.LogError(
                                $"Error while creating cautionary alert of type {cautionaryAlertProperty} for contact with id {cautionaryAlert.contactId}");
                            throw new CreateCautionaryAlertServiceException();
                        }
                    }
                    else
                    {
                        _loggerAdapter.LogError(
                            $"Cautionary alert could not be created for contact with id {cautionaryAlert.contactId} ");
                        throw new CreateCautionaryAlertMissingResultException();
                    }
                }
            }
            catch (Exception e)
            {
                //log error response
                throw e;
            }

            //update contact's flag

            try
            {
                HttpResponseMessage result = null;

                string queryForGettingAllContactsByUprn =
                    HousingAPIQueryBuilder.getAllContactsByUprn(cautionaryAlert.uprn);


                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _client = _hackneyContactsApiBuilder.CreateRequest(accessToken).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "return=representation");

                result = _hackneyContactstApi
                    .getHousingAPIResponse(_client, queryForGettingAllContactsByUprn, cautionaryAlert.uprn).Result;
                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        _loggerAdapter.LogError(
                            $"Error while getting all contacts by UPRN {cautionaryAlert.uprn}");
                        throw new CreateCautionaryAlertServiceException();
                    }
                    JObject jRetrieveResponse =
                        JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                    if (jRetrieveResponse != null)
                    {
                        List<JToken> contactsInTheSameProperty = jRetrieveResponse["value"].ToList();

                        int countOfContactsWithCautionaryAlerts = 0;
                        foreach (dynamic contact in contactsInTheSameProperty)
                        {

                            bool contactCautionaryAlert = contact["hackney_cautionaryalert"] != null
                                ? (bool)contact["hackney_cautionaryalert"]
                                : false;

                            if (contactCautionaryAlert)
                            {
                                countOfContactsWithCautionaryAlerts++;
                            }
                        }
                        //if there is another person within the property who has a cautionary alert against their name
                        if (countOfContactsWithCautionaryAlerts > 1)
                        {
                            JObject contactJObject = new JObject();
                            contactJObject["hackney_cautionaryalert"] = true;
                            string updateContactsquery =
                                HousingAPIQueryBuilder.updateContactQuery(cautionaryAlert.contactId);
                            result =
                                _hackneyContactstApi.SendAsJsonAsync(_client, new HttpMethod("PATCH"),
                                        updateContactsquery, contactJObject)
                                    .Result;
                            if (!result.IsSuccessStatusCode)
                            {
                                _loggerAdapter.LogError(
                                    $"Error while updating field 'hackney_cautionaryalert' for contact with id {cautionaryAlert.contactId}");
                                throw new CreateCautionaryAlertServiceException();
                            }
                        }
                        else
                        {
                            foreach (var contact in contactsInTheSameProperty)
                            {
                                JObject contactJObject = new JObject();
                                if (contact["contactid"].ToString() == cautionaryAlert.contactId)
                                {
                                    //update contact, who is assigned a cautionary alert
                                    contactJObject["hackney_propertycautionaryalert"] = true;
                                    contactJObject["hackney_cautionaryalert"] = true;
                                    string updateContactsquery =
                                        HousingAPIQueryBuilder.updateContactQuery(contact["contactid"]
                                            .ToString());
                                    result =
                                        _hackneyContactstApi.SendAsJsonAsync(_client, new HttpMethod("PATCH"),
                                                updateContactsquery, contactJObject)
                                            .Result;
                                }
                                else
                                {
                                    //update the other people in the property
                                    contactJObject["hackney_propertycautionaryalert"] = true;
                                    string updateContactsquery =
                                        HousingAPIQueryBuilder.updateContactQuery(contact["contactid"]
                                            .ToString());
                                    result =
                                        _hackneyContactstApi.SendAsJsonAsync(_client, new HttpMethod("PATCH"),
                                                updateContactsquery, contactJObject)
                                            .Result;
                                }

                                if (!result.IsSuccessStatusCode)
                                {
                                    _loggerAdapter.LogError(
                                        $"Error while updating fields 'hackney_cautionaryalert' and 'hackney_propertycautionaryalert' for contact {contact}");
                                    throw new CreateCautionaryAlertServiceException();
                                }
                            }
                        }

                        return new
                        {
                            alertContactId = cautionaryAlert.contactId,
                            alertUprn = cautionaryAlert.uprn,
                            alertCautionaryAlertType = cautionaryAlert.cautionaryAlertType,
                            createdOn = DateTime.Today.ToString("yyyy-MM-dd")
                        };
                    }

                }
                else
                {
                    _loggerAdapter.LogError($" Data missing for cautionary alerts for contact with urpn  {cautionaryAlert.uprn} ");
                    throw new CreateCautionaryAlertMissingResultException();
                }
            }
            catch (Exception ex)
            {
                _loggerAdapter.LogError($" Error occured for error method name -  CreateCautionaryAlert()" +
                                        ex.Message);
                throw;
            }
            //change this
            return null;
        }

        public async Task<object> GetContactsByUprn(string uprn)
        {
            HttpResponseMessage result = null;
            var contactList = new List<dynamic>();
            try
            {
                _loggerAdapter.LogInformation($"Getting contact for uprn {uprn}  ");


                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _client = _hackneyContactsApiBuilder.CreateRequest(accessToken).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");

                string query =
                    HousingAPIQueryBuilder.GetAllContactsandDetailsByUprn(uprn);

                result = _hackneyContactstApi.getHousingAPIResponse(_client, query, uprn).Result;

                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new ContactsServiceException();
                    }
                    JObject jRetrieveContacts =
                        JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);

                    if (jRetrieveContacts != null)
                    {
                        List<JToken> contacts = jRetrieveContacts["value"].ToList();
                        foreach (dynamic contact in contacts)
                        {
                            dynamic contactObj = new ExpandoObject();

                            contactObj.contactId = contact["contactid"];
                            contactObj.emailAddress = contact["emailaddress1"];
                            contactObj.uprn = contact["hackney_uprn"];
                            contactObj.addressLine1 = contact["address1_line1"];
                            contactObj.addressLine2 = contact["address1_line2"];
                            contactObj.addressLine3 = contact["address1_line3"];
                            contactObj.firstName = contact["firstname"];
                            contactObj.lastName = contact["lastname"];
                            contactObj.fullName = contact["fullname"];
                            contactObj.larn = contact["hackney_larn"];
                            contactObj.telephone1 = contact["telephone1"];
                            contactObj.telephone2 = contact["telephone2"];
                            contactObj.telephone3 = contact["telephone3"];
                            contactObj.cautionaryAlert = contact["hackney_cautionaryalert"];
                            contactObj.propertyCautionaryAlert = contact["hackney_propertycautionaryalert"];
                            contactObj.houseRef = contact["housing_house_ref"];
                            contactObj.title = contact["hackney_title"];
                            contactObj.fullAddressDisplay = contact["address1_composite"];
                            contactObj.fullAddressSearch = contact["address1_name"];
                            contactObj.postCode = contact["address1_postalcode"];
                            contactObj.dateOfBirth = contact["birthdate"];
                            contactObj.hackneyHomesId = contact["hackney_hackneyhomesid"];
                            contactObj.disabled = contact["hackney_disabled"];
                            contactObj.relationship = contact["hackney_relationship"];
                            contactObj.extendedrelationship = contact["hackney_extendedrelationship"];
                            contactObj.responsible = contact["hackney_responsible"];
                            contactObj.age = contact["hackney_age"];
                            contactList.Add(contactObj);
                        }
                    }

                }

                return new
                {
                    results = contactList
                };

            }
            catch (Exception ex)
            {
                _loggerAdapter.LogError($" GetContactsByUprn error method name -  GetContactsByUprn()" + ex.Message);
                throw ex;
            }
        }
        public async Task<object> UpdateNextOfKin(NextOfKin nextOfKinDetails)
        {
            HttpResponseMessage result = null;
            dynamic nextOfKinObj = new ExpandoObject();
            try
            {
                _loggerAdapter.LogInformation($"Updating next of kin details by contact with ID {nextOfKinDetails.contactID} ");

                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _client = _hackneyContactsApiBuilder.CreateRequest(accessToken).Result;
             //   _client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
                _client.DefaultRequestHeaders.Add("Prefer", "return=representation");

                string query =
                    HousingAPIQueryBuilder.updateNextOfKinDetails(nextOfKinDetails.contactID.ToString());
                JObject request = new JObject();
                if (nextOfKinDetails.nextOfKinName != null)
                {
                    request["hackney_nextofkinname"] = nextOfKinDetails.nextOfKinName;
                }

                if (nextOfKinDetails.nextOfKinAddress != null)
                {
                    request["hackney_nextofkinaddress"] = nextOfKinDetails.nextOfKinAddress;

                }

                if (nextOfKinDetails.nextOfKinRelationship != null)
                {
                    request["hackney_nextofkinrelationship"] = nextOfKinDetails.nextOfKinRelationship;
                }

                if (nextOfKinDetails.nextOfKinOtherTelehone != null)
                {
                    request["hackney_nextofkinotherphone"] = nextOfKinDetails.nextOfKinOtherTelehone;
                }

                if (nextOfKinDetails.nextOfKinEmail != null)
                {
                    request["hackney_nextofkinemail"] = nextOfKinDetails.nextOfKinEmail;
                }

                if (nextOfKinDetails.nextOfKinMobile != null)
                {
                    request["hackney_nextofkinmobile"] = nextOfKinDetails.nextOfKinMobile;
                }

                result = _hackneyContactstApi.SendAsJsonAsync(_client, new HttpMethod("PATCH"), query, request).Result;

                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new ContactsServiceException();
                    }
                    JObject jRetrieveNextOfKin =
                        JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);

                    if (jRetrieveNextOfKin != null)
                    {
                        nextOfKinObj = new
                        {
                            nextOfKinName = jRetrieveNextOfKin["hackney_nextofkinname"],
                            nextOfKinAddress = jRetrieveNextOfKin["hackney_nextofkinaddress"],
                            nextOfKinRelationship = jRetrieveNextOfKin["hackney_nextofkinrelationship"],
                            nextOfKinOtherPhone = jRetrieveNextOfKin["hackney_nextofkinotherphone"],
                            nextOfKinEmail = jRetrieveNextOfKin["hackney_nextofkinemail"],
                            nextOfKinMobile = jRetrieveNextOfKin["hackney_nextofkinmobile"]
                        };
                    }

                }
                else
                {
                    throw new UpdateNextOfKinMissingResultException();
                }
                
            }
            catch (Exception ex)
            {
                _loggerAdapter.LogError($" UpdateNextOfKin error method name -  UpdateNextOfKin()" + ex.Message);
                throw ex;
            }

            return nextOfKinObj;

        }
        private static void UploadToCIStaging(Contact Contact)
        {
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("INSERT INTO CI_Imports (ID, FIRSTNAME, LASTNAME, EMAIL, BIRTHDATE, ADDRESS1, ADDRESS2, ADDRESS3, CITY, POSTCODE, TELEPHONE1, TELEPHONE2, TELEPHONE3, LARN, HOUSINGID, USN, DATEADDED, UPLOADED )");
            sbSQL.Append("VALUES (@ContactId,@FirstName,@LastName,@Email,@BirthDate,@Address1,@Address2,@Address3,@City,@PostCode,@Telephone1,@Telephone2,@Telephone3,@LARN,@HousingId,@USN,@DateAdded,@Uploaded)");

            try
            {
                // instance connection and command
                using (SqlConnection cn = new SqlConnection(_strCIUploadsConnString))
                using (SqlCommand cmd = new SqlCommand(sbSQL.ToString(), cn))
                {
                    // add parameters and their values
                    cmd.Parameters.Add("@ContactId", System.Data.SqlDbType.VarChar, 250).Value = Contact.CrMcontactId.ToString();
                    cmd.Parameters.Add("@FirstName", System.Data.SqlDbType.VarChar, 100).Value = !string.IsNullOrWhiteSpace(Contact.FirstName) ? Contact.FirstName : (object)DBNull.Value;
                    cmd.Parameters.Add("@LastName", System.Data.SqlDbType.VarChar, 100).Value = !string.IsNullOrWhiteSpace(Contact.LastName) ? Contact.LastName : (object)DBNull.Value;
                    cmd.Parameters.Add("@Email", System.Data.SqlDbType.VarChar, 250).Value = !string.IsNullOrWhiteSpace(Contact.Email) ? Contact.Email : (object)DBNull.Value;
                    cmd.Parameters.Add("@BirthDate", System.Data.SqlDbType.DateTime).Value = Contact.DateOfBirth != null && Contact.DateOfBirth != DateTime.MinValue ? Contact.DateOfBirth : (object)DBNull.Value;
                    cmd.Parameters.Add("@Address1", System.Data.SqlDbType.VarChar, 250).Value = !string.IsNullOrWhiteSpace(Contact.Address1) ? Contact.Address1 : (object)DBNull.Value;
                    cmd.Parameters.Add("@Address2", System.Data.SqlDbType.VarChar, 250).Value = !string.IsNullOrWhiteSpace(Contact.Address2) ? Contact.Address2 : (object)DBNull.Value;
                    cmd.Parameters.Add("@Address3", System.Data.SqlDbType.VarChar, 250).Value = !string.IsNullOrWhiteSpace(Contact.Address3) ? Contact.Address3 : (object)DBNull.Value;
                    cmd.Parameters.Add("@City", System.Data.SqlDbType.VarChar, 100).Value = !string.IsNullOrWhiteSpace(Contact.City) ? Contact.City : (object)DBNull.Value;
                    cmd.Parameters.Add("@PostCode", System.Data.SqlDbType.VarChar, 100).Value = !string.IsNullOrWhiteSpace(Contact.PostCode) ? Contact.PostCode : (object)DBNull.Value;
                    cmd.Parameters.Add("@Telephone1", System.Data.SqlDbType.VarChar, 100).Value = !string.IsNullOrWhiteSpace(Contact.Telephone1) ? Contact.Telephone1 : (object)DBNull.Value;
                    cmd.Parameters.Add("@Telephone2", System.Data.SqlDbType.VarChar, 100).Value = !string.IsNullOrWhiteSpace(Contact.Telephone2) ? Contact.Telephone2 : (object)DBNull.Value;
                    cmd.Parameters.Add("@Telephone3", System.Data.SqlDbType.VarChar, 100).Value = !string.IsNullOrWhiteSpace(Contact.Telephone3) ? Contact.Telephone3 : (object)DBNull.Value;
                    cmd.Parameters.Add("@LARN", System.Data.SqlDbType.VarChar, 100).Value = !string.IsNullOrWhiteSpace(Contact.LARN) ? Contact.LARN : (object)DBNull.Value;
                    cmd.Parameters.Add("@HousingId", System.Data.SqlDbType.VarChar, 100).Value = !string.IsNullOrWhiteSpace(Contact.HousingId) ? Contact.HousingId : (object)DBNull.Value;
                    cmd.Parameters.Add("@USN", System.Data.SqlDbType.VarChar, 100).Value = !string.IsNullOrWhiteSpace(Contact.USN) ? Contact.USN : (object)DBNull.Value;
                    cmd.Parameters.Add("@DateAdded", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@Uploaded", System.Data.SqlDbType.Bit, 100).Value = false;

                    // open connection, execute command and close connection
                    cn.Open();
                    int success = cmd.ExecuteNonQuery();
                    cn.Close();

                    if (success == 0)
                    {
                        //log failure
                        throw new Exception("Record not saved to database.");
                    }
                    else
                    {
                        //log success
                    }
                }
                // int success = SqlHelper.ExecuteNonQuery(_strCIUploadsConnString, CommandType.Text, sbSQL.ToString());

            }
            catch (Exception e)
            {
                //log error
                throw e;
            }
        }

        private List<dynamic> prepareCautionaryAlertResultObject(List<JToken> responseList)
        {
            var cautionaryAlertObjectList = new List<dynamic>();
            foreach (dynamic response in responseList)
            {
                dynamic cautinaryAlertObject = new ExpandoObject();
                cautinaryAlertObject.cautionaryAlertType =
                    response["hackney_cautionaryalerttype@OData.Community.Display.V1.FormattedValue"];
                cautinaryAlertObject.cautionaryAlertId = response["hackney_cautionaryalertid"];
                cautinaryAlertObject.contactId = response["_hackney_contactid_value"];
                cautinaryAlertObject.contactName = response["_hackney_contactid_value@OData.Community.Display.V1.FormattedValue"];
                cautinaryAlertObject.uprn = response["hackney_uprn"];
                cautinaryAlertObject.createdOn = response["createdon"] != null ? response["createdon"].ToString("yyyy-MM-dd HH:mm:ss") : null;

                cautionaryAlertObjectList.Add(cautinaryAlertObject);
            }
            return cautionaryAlertObjectList;
        }
    }
    public class ContactsServiceException : System.Exception
    {
    }
    public class UpdateContactMissingResultException : System.Exception { }

    public class CreateContactMissingResultException : System.Exception { }

    public class GetCautionaryAlertMissingResultException : System.Exception { }

    public class RemoveCautionaryAlertMissingResultException : System.Exception { }
    public class GetContactDetailsMissingResultException : System.Exception { }

    public class CreateCautionaryAlertMissingResultException : System.Exception { }
    public class CreateCautionaryAlertServiceException : System.Exception { }
    public class RemoveCautionaryAlertServiceException : System.Exception { }
    public class GetContactDetailsServiceException : System.Exception { }
    public class UpdateNextOfKinMissingResultException : System.Exception { }
}