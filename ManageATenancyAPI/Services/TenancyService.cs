﻿using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models.Housing.NHO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Services
{
    public interface IClock
    {
        DateTime Now();
    }
    
    public interface ILastRetrieved
    {
        void UpdateLastRun(DateTime timestamp);
        DateTime GetLastRun();
    }
    
    public class TenancyService : ITenancyService
    {
        private readonly HttpClient _client;
        private readonly IHackneyHousingAPICall _manageATenancyAPI;
        private readonly ILastRetrieved _lastRetrieved;
        private readonly IClock _clock;

        public TenancyService(IHackneyHousingAPICallBuilder apiCallBuilder, IHackneyHousingAPICall apiCall,
            IHackneyGetCRM365Token accessToken, ILastRetrieved lastRetrieved, IClock clock)
        {
            _manageATenancyAPI = apiCall;
            _lastRetrieved = lastRetrieved;
            _clock = clock;

            var token = accessToken.getCRM365AccessToken().Result;
            _client = apiCallBuilder.CreateRequest(token).Result;
        }

        public async Task<IEnumerable<NewTenancyResponse>> GetNewTenancies()
        {
            var lastRun = _lastRetrieved.GetLastRun();
            var query = HousingAPIQueryBuilder.GetNewTenanciesSinceDate(lastRun);
            _lastRetrieved.UpdateLastRun(_clock.Now());
            
            var result = await _manageATenancyAPI.getHousingAPIResponse(_client, query, null);

            if (result == null)
            {
                return null;
            }

            if (!result.IsSuccessStatusCode)
            {
                throw new TenancyServiceException();
            }

            var response = JsonConvert.DeserializeObject<dynamic>(await result.Content.ReadAsStringAsync());

            if (response == null || response["value"] == null)
            {
                return null;
            }

            var value = response["value"].ToObject<List<dynamic>>();

            var newTenancies = new List<NewTenancyResponse>();
            foreach (var entry in value)
            {
                var accountId = (string) entry["accountid"];
                var housingTenure = (string) entry["housing_tenure"];
                var fullAddress = (string) entry["contact1_x002e_address1_composite"];
                var existingNewTenancy = newTenancies.FirstOrDefault(x =>
                    x.AccountId == accountId &&
                    x.HousingTenure == housingTenure &&
                    x.FullAddress == fullAddress
                );

                if (existingNewTenancy != null)
                {
                    if (existingNewTenancy.Contacts.FirstOrDefault(x =>
                            x.ContactId == entry["contact1_x002e_contactid"].ToString()) != null)
                    {
                        continue;
                    }

                    var newContact = CreateNewTenancyContact(entry);

                    //ensure that a responsible contact is always first in the list
                    if (newContact.Responsible)
                    {
                        existingNewTenancy.Contacts.Insert(0, newContact);
                    }
                    else
                    {
                        existingNewTenancy.Contacts.Add(newContact);
                    }
                }
                else
                {
                    newTenancies.Add(CreateNewTenancyResponse(entry));
                }
            }

            return newTenancies;
        }

        private static NewTenancyContact CreateNewTenancyContact(dynamic entry)
        {
            return  new NewTenancyContact
            {
                FirstName = entry["contact1_x002e_firstname"],
                LastName = entry["contact1_x002e_lastname"],
                Responsible = entry["contact1_x002e_hackney_responsible"],
                Title = entry["contact1_x002e_hackney_title"],
                ContactId = entry["contact1_x002e_contactid"]
            };
        }

        private static NewTenancyResponse CreateNewTenancyResponse(dynamic entry)
        {
            return new NewTenancyResponse
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
                OfficerId = entry["estateOfficerId"],
                OfficerName = entry["OfficerFullName"],
                HouseholdId = entry["_hackney_household_accountid_value"],
                PatchId = entry["hackney_estateofficerpatch3_x002e_hackney_estateofficerpatchid"],
                TagReference = entry["housing_tag_ref"],
                AreaId = entry["hackney_propertyareapatch2_x002e_hackney_areaname"],
                ManagerId = entry["hackney_propertyareapatch2_x002e_hackney_managerpropertypatchid"],
                Contacts = new List<NewTenancyContact>
                {
                    new NewTenancyContact
                    {
                        FirstName = entry["contact1_x002e_firstname"],
                        LastName = entry["contact1_x002e_lastname"],
                        Responsible = entry["contact1_x002e_hackney_responsible"],
                        Title = entry["contact1_x002e_hackney_title"],
                        ContactId = entry["contact1_x002e_contactid"]
                    }
                }
            };
        }
    }
}