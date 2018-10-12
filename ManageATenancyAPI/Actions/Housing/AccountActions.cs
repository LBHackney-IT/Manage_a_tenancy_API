using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Services.Housing;
using Newtonsoft.Json;
using System.Linq;
using ManageATenancyAPI.Models.Housing;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Models;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace ManageATenancyAPI.Actions
{
    public class AccountActions
    {

        private HttpClient _client;
        private readonly HackneyHousingAPICallBuilder _apiCallBuilder;
        private ILoggerAdapter<AccountActions> _logger;
        private IHackneyHousingAPICall _hackneyLeaseAccountApi;
        private IHackneyHousingAPICallBuilder _hackneyLeaseAccountApiBuilder;
        private IHackneyGetCRM365Token _accessToken;
        private readonly IHackneyUHWWarehouseService _accountsrepositoryApiCall;

        public AccountActions(ILoggerAdapter<AccountActions> logger, IHackneyHousingAPICallBuilder apiCallBuilder, IHackneyHousingAPICall apiCall, IHackneyUHWWarehouseService accountsrepositoryApiCall,IHackneyGetCRM365Token accessToken)
        {
            _client = new HttpClient();
            _accessToken = accessToken;
            _hackneyLeaseAccountApiBuilder = apiCallBuilder;
            _hackneyLeaseAccountApi = apiCall;
            _logger = logger;
            _accountsrepositoryApiCall = accountsrepositoryApiCall;

        }

        public async Task<object> GetAccountsByParisReferenceAndPostcode(string parisReference, string postcode)
        {
            HttpResponseMessage result = null;
            try
            {
                _logger.LogInformation($"Getting Accounts Details for parisReference {parisReference} and post code {postcode}  ");


                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _client = _hackneyLeaseAccountApiBuilder.CreateRequest(accessToken).Result;

                var query = HousingAPIQueryBuilder.getAccountsByParisReferenceAndPostcodeQuery(parisReference, postcode);

                result = _hackneyLeaseAccountApi.getHousingAPIResponse(_client, query, parisReference).Result;
                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new AccountServiceException();
                    }
                    var accountRetrieveResponse = JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                    if (accountRetrieveResponse?["value"] != null)
                    {

                        dynamic accountResponse = accountRetrieveResponse["value"];
                        var accountlist = new List<AccountsAndAddress>();
                        foreach (var accountDetail in accountResponse)
                        {
                            accountlist.Add(buildAccount(accountDetail));
                        }

                        return new
                        {
                            results = accountlist
                        };
                    }
                    return null;
                }
                else
                {
                    _logger.LogError($" Accounts Details Missing for parisReference {parisReference} and post code {postcode} ");
                    throw new MissingResultException();
                }

            }
            catch (Exception ex)
            {
                if (result == null)
                {
                    _logger.LogError($" Accounts Details Missing for parisReference {parisReference} and post code {postcode} ");
                    throw new MissingResultException();
                }
                else
                {
                    _logger.LogError($" Accounts Details getting AccountServiceException for parisReference {parisReference} and post code {postcode} ");
                    throw new AccountServiceException();
                }
            }

        }

        public async Task<object> GetAccountDetailsByParisorTagReference(string parisReference)
        {
            HttpResponseMessage result = null;
            try
            {
                _logger.LogInformation($"Getting Accounts Details for parisReference {parisReference}");


                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _client = _hackneyLeaseAccountApiBuilder.CreateRequest(accessToken).Result;

                var query = HousingAPIQueryBuilder.getAccountDetailsByTagorParisReferenceQuery(parisReference);

                result = _hackneyLeaseAccountApi.getHousingAPIResponse(_client, query, parisReference).Result;
                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new AccountServiceException();
                    }

                    var accountDetailsRetrieveResponse = JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                    if (accountDetailsRetrieveResponse?["value"] != null)
                    {

                        List<JToken> accountResponse = accountDetailsRetrieveResponse["value"].ToList();

                        return new
                        {
                            results = buildAccountDetails(accountResponse)
                        };


                    }
                    else
                    {
                        return new
                        {
                            results = new object() { }

                        };
                    }

                }
                else
                {
                    _logger.LogError($" Accounts Details Missing for parisReference {parisReference}");
                    throw new MissingResultException();
                }

            }
            catch (Exception ex)
            {
                if (result == null)
                {
                    _logger.LogError($" Accounts Details Missing for parisReference {parisReference} ");
                    throw new MissingResultException();
                }
                else
                {
                    _logger.LogError($" Accounts Details getting AccountServiceException for parisReference {parisReference} ");
                    throw new AccountServiceException();
                }
            }

        }
    
     
        public async Task<object> GetTagReferencenumber(string hackneyhomesId)
        {
            try
            {
                _logger.LogInformation($"Getting Tag Reference number for hackneyhomesId {hackneyhomesId}");

                var result = await _accountsrepositoryApiCall.GetTagReferencenumber(hackneyhomesId);
                if (result != null)
                {
                    var tagReferenceList = new List<string> { result.ToString() };

                    return new
                    {
                        results = tagReferenceList
                    };
                }


                return new
                {
                    results = new object[] { }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Getting Tag Reference number return null results for hackneyhomesId {hackneyhomesId} ");
                throw new MissingResultException();
            }

        }

        public async Task<object> GetAccountDetailsByContactId(string contactid)
        {
            HttpResponseMessage result = null;
            try
            {
                _logger.LogInformation($"Getting Accounts Details for contactid {contactid}");

                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _client = _hackneyLeaseAccountApiBuilder.CreateRequest(accessToken).Result;

                var query = HousingAPIQueryBuilder.getAAccountDetailsByContactIdQuery(contactid);

                result = _hackneyLeaseAccountApi.getHousingAPIResponse(_client, query, contactid).Result;


                var accountRetrieveResponse = JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new AccountServiceException();
                    }

                    var accountDetailslist =new AccountDetails();
                    var accountDetailsRetrieveResponse = JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                    if (accountDetailsRetrieveResponse?["value"] != null && accountDetailsRetrieveResponse.Count > 0)
                    {
                        dynamic accountResponse = accountDetailsRetrieveResponse["value"].FirstOrDefault();

                        if (accountResponse != null)
                        {
                            accountDetailslist=buildAccountDetailsbyContactId(accountResponse);
                        }

                        
                        return new
                        {
                            results = accountDetailslist
                        };


                    }
                    else
                    {
                        return new
                        {
                            results = accountDetailslist

                        };
                    }

                }
                else
                {
                    _logger.LogError($" Accounts Details Missing for contactid {contactid}");
                    throw new MissingResultException();
                }

            }
            catch (Exception ex)
            {
                if (result == null)
                {
                    _logger.LogError($" Accounts Details Missing for contactid {contactid} ");
                    throw new MissingResultException();
                }
                else
                {
                    _logger.LogError($" Accounts Details getting AccountServiceException for contactid {contactid} ");
                    throw new AccountServiceException();
                }
            }

        }


        private object buildAccount(dynamic accountResponse)
        {
            return new AccountsAndAddress()
            {
                address = accountResponse.aj_x002e_housing_short_address != null ? accountResponse.aj_x002e_housing_short_address.ToString().Trim() : null,
                postcode = accountResponse.aj_x002e_housing_post_code != null ? accountResponse.aj_x002e_housing_post_code.ToString().Trim() : null,
                parisReferenceNumber = accountResponse.housing_u_saff_rentacc != null ? accountResponse.housing_u_saff_rentacc.ToString().Trim() : null,
                addressType = accountResponse.aj_x002e_addresstypecode != null ? accountResponse.aj_x002e_addresstypecode : null

            };
        }
        private object buildAccountDetails(List<JToken> accountResponse)
        {
            var groupinfo = (from response in accountResponse
                             group response by new
                             {
                                 accountid = response["accountid"],
                                 tagReferenceNumber = response["housing_tag_ref"],
                                 benefit = response["housing_anticipated"],
                                 propertyReferenceNumber = response["housing_prop_ref"],
                                 currentBalance = response["housing_cur_bal"],
                                 rent = response["housing_rent"],
                                 housingReferenceNumber = response["housing_house_ref"],
                                 directdebit = response["housing_directdebit"],
                             })
                .Select(grp => new
                {
                    grp.Key.accountid,
                    grp.Key.tagReferenceNumber,
                    grp.Key.benefit,
                    grp.Key.propertyReferenceNumber,
                    grp.Key.currentBalance,
                    grp.Key.rent,
                    grp.Key.housingReferenceNumber,
                    grp.Key.directdebit,
                    Tenants = (from tenantsResponse in grp.ToList()
                               group tenantsResponse by new
                               {
                                   personNumber = tenantsResponse["contact1_x002e_hackney_personno"],
                                   responsible = tenantsResponse["contact1_x002e_hackney_responsible"],
                                   title = tenantsResponse["contact1_x002e_hackney_title"],
                                   forename = tenantsResponse["contact1_x002e_firstname"],
                                   surname = tenantsResponse["contact1_x002e_lastname"],

                               }).Select(tenantsResponse => new
                               {
                                   tenantsResponse.Key.personNumber,
                                   tenantsResponse.Key.responsible,
                                   tenantsResponse.Key.title,
                                   tenantsResponse.Key.forename,
                                   tenantsResponse.Key.surname,

                               }),
                    Addresses = (from addressResponse in grp.ToList()
                                 group addressResponse by new
                                 {
                                     postCode = addressResponse["contact1_x002e_address1_postalcode"],
                                     shortAddress = addressResponse["contact1_x002e_address1_line1"].ToString() +" "+ addressResponse["contact1_x002e_address1_line2"].ToString() + " " + addressResponse["contact1_x002e_address1_line3"].ToString(),
                                     addressTypeCode = addressResponse["customeraddress2_x002e_addresstypecode"]

                                 }).Select(addressResponse => new
                                 {
                                     addressResponse.Key.postCode,
                                     addressResponse.Key.shortAddress,
                                     addressResponse.Key.addressTypeCode,

                                 }),
                });

            var accountDetailList = new List<dynamic>();

            foreach (dynamic response in groupinfo)
            {

                dynamic accountDetailsObj = new ExpandoObject();
                accountDetailsObj.accountid = response.accountid;
                accountDetailsObj.tagReferenceNumber = response.tagReferenceNumber;
                accountDetailsObj.benefit = response.benefit;
                accountDetailsObj.propertyReferenceNumber = response.propertyReferenceNumber;
                accountDetailsObj.currentBalance = response.currentBalance;
                accountDetailsObj.rent = response.rent;
                accountDetailsObj.housingReferenceNumber = response.housingReferenceNumber;
                accountDetailsObj.directdebit = response.directdebit;
                accountDetailsObj.ListOfTenants = new List<ExpandoObject>();
                accountDetailsObj.ListOfAddresses = new List<ExpandoObject>();

                foreach (var tenantsResponse in response.Tenants)
                {
                    dynamic tenants = new ExpandoObject();
                    tenants.personNumber = tenantsResponse.personNumber;
                    tenants.responsible = tenantsResponse.responsible;
                    tenants.title = tenantsResponse.title;
                    tenants.forename = tenantsResponse.forename;
                    tenants.surname = tenantsResponse.surname;

                    accountDetailsObj.ListOfTenants.Add(tenants);


                }
                foreach (var addressesResponse in response.Addresses)
                {
                    dynamic addresses = new ExpandoObject();
                    addresses.postCode = addressesResponse.postCode;
                    addresses.shortAddress = addressesResponse.shortAddress;
                    addresses.addressTypeCode = addressesResponse.addressTypeCode;
                    accountDetailsObj.ListOfAddresses.Add(addresses);
                }
                accountDetailList.Add(accountDetailsObj);

            }
            return accountDetailList;
        }


        private List<object> buildDebItemsObject(List<JToken> responseList)
        {
            var debtItemObjectList = new List<dynamic>();
            foreach (dynamic response in responseList)
            {
                dynamic debItemObject = new ExpandoObject();
                debItemObject.debItemId = response["housing_debitemid"] != null ? response["housing_debitemid"] : null;
                debItemObject.debItemCode = response["housing_deb_code"] != null ? response["housing_deb_code"] : null;
                debItemObject.debtItemValue = response["housing_deb_value"] != null ? response["housing_deb_value"] : null;
                debItemObject.effectiveDate = response["housing_eff_date"] != null ? response["housing_eff_date"] : null;
                debItemObject.propertyReference = response["housing_prop_ref"] != null ? response["housing_prop_ref"] : null;
                debItemObject.tagReference = response["housing_tag_ref"] != null ? response["housing_tag_ref"] : null;
                debItemObject.termDate = response["housing_term_date"] != null ? response["housing_term_date"] : null;
                debtItemObjectList.Add(debItemObject);
            }
            return debtItemObjectList;
        }

        private object buildAccountDetailsbyContactId(dynamic accountResponse)
        {
            return new AccountDetails()
            {
                accountid = accountResponse["accountid"] != null ? accountResponse["accountid"].ToString().Trim() : null,
                tagReferenceNumber = accountResponse["housing_tag_ref"] != null ? accountResponse["housing_tag_ref"].ToString().Trim() : null,
                paymentReferenceNumber = accountResponse["housing_u_saff_rentacc"] != null ? accountResponse["housing_u_saff_rentacc"].ToString().Trim() : null,
                benefit = accountResponse["housing_anticipated"] != null ? accountResponse["housing_anticipated"].ToString().Trim() : null,
                propertyReferenceNumber = accountResponse["housing_prop_ref"] != null ? accountResponse["housing_prop_ref"].ToString().Trim() : null,
                currentBalance = accountResponse["housing_cur_bal"] != null ? accountResponse["housing_cur_bal"].ToString().Trim() : null,
                rent = accountResponse["housing_rent"] != null ? accountResponse["housing_rent"].ToString().Trim() : null,
                housingReferenceNumber = accountResponse["housing_house_ref"] != null ? accountResponse["housing_house_ref"].ToString().Trim() : null,
                directdebit = accountResponse["housing_directdebit"] != null ? accountResponse["housing_directdebit"].ToString().Trim() : null,
                tenuretype = accountResponse["housing_tenure"] != null ? GetTenureTypeCode(accountResponse["housing_tenure"].ToString().Trim()) : null,
                tenancyStartDate = accountResponse["housing_cot"] != null ? accountResponse["housing_cot"].ToString().Trim() : null,
                isAgreementTerminated = accountResponse["housing_terminated"],
                accountType = accountResponse["housing_accounttype"] != null ? accountResponse["housing_accounttype"].ToString().Trim() : null,
                agreementType = accountResponse["housing_agr_type"] != null ? accountResponse["housing_agr_type"].ToString().Trim() : null
            };
        }
        
        private static string GetTenureTypeCode(string tenure)
        {
            string tenuredescription= string.Empty;

            if (tenure == "ASY")
            {
                tenuredescription = "Asylum seeker";
            }
            else if (tenure == "DEC")
            {
                tenuredescription = "Temp Decant";
            }
            else if (tenure == "FRE")
            {
                tenuredescription = "Freehold";
            }
            else if (tenure == "INT")
            {
                tenuredescription = "Introductory";
            }
            else if (tenure == "LEA")
            {
                tenuredescription = "Leasehold (RTB)";
            }
            else if (tenure == "LTA")
            {
                tenuredescription = "License temporay account";
            }
            else if (tenure == "MPA")
            {
                tenuredescription = "Mesne profit account";
            }
            else if (tenure == "NON")
            {
                tenuredescription = "Non-secure";
            }
            else if (tenure == "PVG")
            {
                tenuredescription = "Private garage";
            }
            else if (tenure == "RSL")
            {
                tenuredescription = "Landlord";
            }
            else if (tenure == "RTM")
            {
                tenuredescription = "Rent to mortgage";
            }
            else if (tenure == "SEC")
            {
                tenuredescription = "Secure";
            }
            else if (tenure == "SLL")
            {
                tenuredescription = "Short life lease";
            }
            else if (tenure == "TAF")
            {
                tenuredescription = "Tenants association flat";
            }
            else if (tenure == "TBB")
            {
                tenuredescription = "Temporary B&B";
            }
            else if (tenure == "TGA")
            {
                tenuredescription = "Tenant garage";
            }
            else if (tenure == "THL")
            {
                tenuredescription = "Temporary hostel lease";
            }
            else if (tenure == "THO")
            {
                tenuredescription = "Temporary hostel";
            }
            else if (tenure == "TLA")
            {
                tenuredescription = "Temporary annex";
            }
            else if (tenure == "TPL")
            {
                tenuredescription = "Temporary private let";
            }
            else if (tenure == "THL")
            {
                tenuredescription = "Temporary hostel lease";
            }
            else if (tenure == "TRA")
            {
                tenuredescription = "Temporary Traveller";
            }
            else if (tenure == "COM")
            {
                tenuredescription = "Commercial let";
            }
            else if (tenure == "DEC")
            {
                tenuredescription = "Temporay decant";
            }
            else if (tenure == "FRS")
            {
                tenuredescription = "Freehold (service charge)";
            }
            else if (tenure == "HPH")
            {
                tenuredescription = "HPH(SLA)";
            }
            else if (tenure == "LHS")
            {
                tenuredescription = "Leasehold 100% stair";
            }
            else if (tenure == "SHO")
            {
                tenuredescription = "Shared ownership";
            }
            else if (tenure == "SPS")
            {
                tenuredescription = "Private sale leasehold";
            }
            else if (tenure == "SPT")
            {
                tenuredescription = "Sheltered accommodation";

            }
            else if (tenure == "SSE")
            {
                tenuredescription = "Share equity";
            }
            else
            {
                tenuredescription = "Temp Traveller";
            }
            return tenuredescription;
         }
            private AccountsAndNotifications buildAccountsAndNotification(dynamic accountResponse)
        {
            return new AccountsAndNotifications()
            {
                currentBalance = accountResponse["housing_cur_bal"] != null ? accountResponse["housing_cur_bal"].ToString().Trim() : null,
                telephone = accountResponse["housing_rentaccountnotification1_x002e_housing_phone"] != null ? accountResponse["housing_rentaccountnotification1_x002e_housing_phone"].ToString().Trim() : null,
                areNotificationsOn = accountResponse["housing_rentaccountnotification1_x002e_housing_isnotification"] != null ? accountResponse["housing_rentaccountnotification1_x002e_housing_isnotification"] : false,
                paymentAgreementId = accountResponse["housing_paymentagreement2_x002e_housing_paymentagreementid"] != null ? accountResponse["housing_paymentagreement2_x002e_housing_paymentagreementid"] : null,
                paymentAgreementEndDate = accountResponse["housing_paymentagreement2_x002e_housing_aragdet_enddate"] != null ? accountResponse["housing_paymentagreement2_x002e_housing_aragdet_enddate"].ToString("yyyy-MM-dd") : null
            };
        }
    }

    public class AccountServiceException : System.Exception
    {
    }

    public class MissingResultException : System.Exception { }


}
