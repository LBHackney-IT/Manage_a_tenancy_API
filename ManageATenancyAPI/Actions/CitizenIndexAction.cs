using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Services.Housing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Actions
{
    public class CitizenIndexAction
    {
        private HttpClient _client;
        private readonly IHackneyGetCRM365Token _accessToken;
        private readonly HackneyHousingAPICallBuilder _apiCallBuilder;
        private readonly ILoggerAdapter<HackneyHousingAPICallBuilder> _apiBuilderLoggerAdapter;
        private readonly ILoggerAdapter<CitizenIndexAction> _logger;        
        private readonly IHackneyHousingAPICall _ManageATenancyAPI;
        private readonly IHackneyHousingAPICallBuilder _hackneyAccountApiBuilder;
        private readonly ICitizenIndexRepository _citizenIndexRepository;

        public CitizenIndexAction(ILoggerAdapter<CitizenIndexAction> logger, IHackneyHousingAPICallBuilder apiCallBuilder, IHackneyHousingAPICall apiCall, ICitizenIndexRepository citizenIndexRepository,IHackneyGetCRM365Token accessToken)
        {
            _client = new HttpClient();
            _accessToken = accessToken;
            _hackneyAccountApiBuilder = apiCallBuilder;
            _ManageATenancyAPI = apiCall;
            _logger = logger;
            _citizenIndexRepository = citizenIndexRepository;
        }

        public async Task<object> CitizenIndexSearch(string firstname, string surname, string addressline12, string postcode,bool IsAdvanceSearch)
        {

            addressline12 = addressline12.Trim().ToUpper();
            var addressSearch = addressline12.Split(' ').Select(searchpattern => searchpattern.Trim()).ToArray();

            _logger.LogInformation($" CitizenIndexSearch Request Started.");
            
            
            var citizenIndexresults = new List<CIPerson>();            
            
           
            try
            {
                if (IsAdvanceSearch)
                {
                    _logger.LogInformation($" CitizenIndexSearch Request Started for SQl Execution");
                     citizenIndexresults = _citizenIndexRepository.SearchCitizenIndex(firstname, surname, addressline12, postcode);
                    _logger.LogInformation($" CitizenIndexSearch Request Ended for SQl Execution");

                    if (citizenIndexresults != null)
                    {
                        _logger.LogInformation($" CitizenIndexSearch citiZenIndexresults Count {citizenIndexresults.Count}");
                    }
                }

                var crmContactsresults = await GetCRMContacts(firstname, surname, addressSearch, postcode);

                if (crmContactsresults != null && crmContactsresults.Count > 0)
                {
                    _logger.LogInformation($" CitizenIndexSearch CRMContactsresults Count {crmContactsresults.Count}");

                    if (IsAdvanceSearch)
                    {
                        var mergedcitizenResults = new List<CIPerson>(crmContactsresults);
                        mergedcitizenResults.AddRange(citizenIndexresults
                            .Where(p1 => crmContactsresults.All(p2 => p1.CrmContactId != p2.CrmContactId && p1.LARN != p2.LARN)));

                        return new
                        {
                            results = mergedcitizenResults
                        };
                    }
                    return new
                    {
                        results = crmContactsresults
                    };
                }

                return new
                {
                    results = citizenIndexresults
                };
                
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }   

        protected virtual async Task<List<CIPerson>> GetCRMContacts(string firstname, string surname, string[] address, string postcode)
        {
            try
            {
                _logger.LogInformation($"CitizenIndexSearch CRM 365 Search Started  ");



                var accessToken = _accessToken.getCRM365AccessToken().Result;
                _client = _hackneyAccountApiBuilder.CreateRequest(accessToken).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "return=representation");
                _client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
                
                var query = HousingAPIQueryBuilder.getCRMCitizenSearch(firstname, surname, address, postcode);
                _logger.LogInformation($" CitizenIndexSearch Query is  {query}");
                _logger.LogError($" CitizenIndexSearch Request ended for  CRM 365");

                var response = _ManageATenancyAPI.getHousingAPIResponse(_client, query, "").Result;
                if (response != null)
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new CitizenIndexServiceException();
                    }

                    var crmCitizenResponse =
                        JsonConvert.DeserializeObject<JObject>(response.Content.ReadAsStringAsync().Result);
                    if(crmCitizenResponse!=null)
                    {
                        var accountResponse = crmCitizenResponse["value"].ToList();

                        return prepareCRMCitizenResponseResultObject(accountResponse);
                    }
                }
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
            return null;
        }

        private List<CIPerson> prepareCRMCitizenResponseResultObject(List<JToken> responseList)
        {
            var list = responseList;
            return list?.Cast<dynamic>()
                .Select(response =>
                {
                    var tenant = new CIPerson
                    {
                        HackneyhomesId = response.hackney_hackneyhomesid,
                        CrmContactId = Guid.Parse(response.contactid.ToString()),
                        Title = response.jobtitle?.ToString(),
                        FirstName = response.firstname?.ToString(),
                        Surname = response.lastname?.ToString(),
                        AddressLine1 = response.address1_line1?.ToString(),
                        AddressLine2 = response.address1_line2?.ToString(),
                        AddressLine3 = response.address1_line3?.ToString(),
                        AddressCity = response.address1_city?.ToString(),
                        AddressCountry = response.address1_country?.ToString(),
                        PostCode = response.address1_postalcode?.ToString(),
                        FullAddressSearch = response.address1_name?.ToString(),
                        LARN = response.hackney_larn?.ToString(),
                        DateOfBirth = response.birthdate?.ToString(),
                        UPRN = response.hackney_uprn?.ToString(),
                        FullAddressDisplay = response.address1_composite?.ToString(),
                        SystemName = "CRM365",
                        CautionaryAlert = response.hackney_cautionaryalert,
                        PropertyCautionaryAlert = response.hackney_propertycautionaryalert,
                        EmailAddress = response.emailaddress1,
                        Telephone1 = response.telephone1,
                        Telephone2 = response.telephone2,
                        Telephone3 = response.telephone3,
                        IsActiveTenant = response.parentcustomerid_account != null ? response.parentcustomerid_account.housing_present : false,
                        Accounttype = response.parentcustomerid_account != null ? response.parentcustomerid_account.housing_accounttype : null,
                        HouseholdId = response.hackney_household_contactId != null ? response.hackney_household_contactId.hackney_householdid : null,
                        MainTenant = response.hackney_responsible
                    };
                    return tenant;
                }).Where(tenant => tenant.IsActiveTenant).ToList();
         }


        public class CitizenIndexServiceException : System.Exception
        {
        }
        public class CitizenIndexNullResponseException : System.Exception
        {
        }
    }
}
