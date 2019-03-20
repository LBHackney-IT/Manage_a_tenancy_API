using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models.Housing.NHO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Services
{
    public class OfficerService : IOfficerService
    {
        private readonly HttpClient _client;
        private readonly IHackneyHousingAPICall _manageATenancyAPI;

        public OfficerService(IHackneyHousingAPICallBuilder apiCallBuilder, IHackneyHousingAPICall apiCall, IHackneyGetCRM365Token accessToken)
        {
            _manageATenancyAPI = apiCall;

            var token = accessToken.getCRM365AccessToken().Result;
            _client = apiCallBuilder.CreateRequest(token).Result;

        }

        public async Task<IList<NewTenancyResponse>> GetNewTenanciesForHousingOfficer(OfficerDetails officer)
        {
            var checkFrom = officer.LastNewTenancyCheck == null
                ? DateTime.Today
                : officer.LastNewTenancyCheck.Value;

            var query = HousingAPIQueryBuilder.GetNewTenanciesForHousingOfficer(officer.Id, checkFrom);

            var newCheckDate = DateTime.Now;

            var response = await HandleApiGetCall(query);

            var newTenancies = new List<NewTenancyResponse>();
            foreach (var entry in response)
            {
                var accountId = (string)entry["accountid"];
                var housingTenure = (string)entry["housing_tenure"];
                var fullAddress = (string)entry["contact1_x002e_address1_composite"];
                var existingNewTenancy = newTenancies.FirstOrDefault(x =>
                    x.AccountId == accountId &&
                    x.HousingTenure == housingTenure &&
                    x.FullAddress == fullAddress
                );
                if (existingNewTenancy != null)
                {
                    var newContact = new NewTenancyContact
                    {
                        FirstName = entry["contact1_x002e_firstname"],
                        LastName = entry["contact1_x002e_lastname"],
                        Responsible = entry["contact1_x002e_hackney_responsible"],
                        Title = entry["contact1_x002e_hackney_title"]
                    };

                    var existingContact = existingNewTenancy.Contacts.FirstOrDefault(x =>
                        x.FirstName == newContact.FirstName &&
                        x.LastName == newContact.LastName &&
                        x.Title == newContact.Title &&
                        x.Responsible == newContact.Responsible);

                    if (existingContact != null)
                        continue;

                    existingNewTenancy.Contacts.Add(newContact);
                }
                else
                {
                    var newTenancy = new NewTenancyResponse
                    {
                        AccountCreatedOn = entry["createdon"],
                        AccountId = entry["accountid"],
                        HousingTenure = entry["housing_tenure"],
                        AddressLine1 = entry["contact1_x002e_address1_line1"],
                        AddressLine2 = entry["contact1_x002e_address1_line2"],
                        AddressLine3 = entry["contact1_x002e_address1_line3"],
                        EstateAddress = entry["hackney_propertyareapatch2_x002e_hackney_estateaddress"],
                        FullAddress = entry["contact1_x002e_address1_composite"],
                        NeighbourhoodOffice = entry["hackney_propertyareapatch2_x002e_hackney_neighbourhoodofficedesc"],
                        PostCode = entry["contact1_x002e_address1_postalcode"],
                        Contacts = new List<NewTenancyContact>
                        {
                            new NewTenancyContact
                            {
                                FirstName = entry["contact1_x002e_firstname"],
                                LastName = entry["contact1_x002e_lastname"],
                                Responsible = entry["contact1_x002e_hackney_responsible"],
                                Title = entry["contact1_x002e_hackney_title"]
                            }
                        }
                    };
                    newTenancies.Add(newTenancy);
                }
            }

            await UpdateLastNewTenancyCheckDate(officer.Id, newCheckDate);

            return newTenancies.OrderBy(x => x.AccountCreatedOn).ToList();
        }

        public async Task<OfficerDetails> GetOfficerDetails(string emailAddress)
        {
            var query = HousingAPIQueryBuilder.GetHousingOfficerDetails(emailAddress);

            var response = await HandleApiGetCall(query);

            var count = response.Count;

            if (count == 0)
                return null;

            if (count != 1)
                throw new InvalidOperationException($"Expected 1 officer, found {count}");

            var officer = response[0];
            
            return officer != null ? OfficerDetails.Create(officer) : null;
        }

        public async Task UpdateLastNewTenancyCheckDate(string id, DateTime date)
        {
            var updateObj = new JObject {
                { "hackney_lastnewtenancycheckdate", date }
            };

            var query = HousingAPIQueryBuilder.UpdateHousingOfficerDetails(id);

            var response = await _manageATenancyAPI.SendAsJsonAsync(_client, HttpMethod.Patch, query, updateObj);

            if (!response.IsSuccessStatusCode)
            {
                throw new TenancyServiceException();
            }
        }

        private async Task<IList<dynamic>> HandleApiGetCall(string query)
        {
            var result = await _manageATenancyAPI.getHousingAPIResponse(_client, query, null);

            if (result != null)
            {
                if (!result.IsSuccessStatusCode)
                {
                    throw new TenancyServiceException();
                }

                var response = JsonConvert.DeserializeObject<dynamic>(await result.Content.ReadAsStringAsync());

                if (response == null || response["value"] == null)
                    return null;

                var value = response["value"].ToObject<List<object>>();
                return value;
            }

            return null;
        }
    }
}