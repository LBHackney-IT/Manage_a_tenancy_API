using System;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Interfaces.Housing;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using Newtonsoft.Json.Linq;

namespace ManageATenancyAPI.Services.Housing
{
    public class FakeHousingAPICall : IHackneyHousingAPICall
    {
        private AccountObject<AccountsAndAddress> _responseData;

        public async Task<HttpResponseMessage> getHousingAPIResponse(HttpClient client, string query, string parameter)
        {

            if (!string.IsNullOrWhiteSpace(query) && client != null)
            {

                Object testObject = null;
                HttpStatusCode httpStatusCode = HttpStatusCode.OK;
              
                //citizen index search surname lookup from query and updated the parameter to throw the 500 error 
                parameter = query.IndexOf("contains(lastname, 'throw500')") != -1 ? "throw500" : parameter;

                switch (parameter)
                {

                    case "228003470":
                        testObject = getAccountAndAddress();
                        break;
                    case "228009977":
                        testObject = getAccountDetails();
                        break;

                    case "101079/01":
                        testObject = getAccountAgreement();
                        break;
                    case "000000/01":
                        testObject = getTransactions();
                        break;
                    case "228000000":
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                        };
                    case "123456/78":
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                        };
                    case "uaccount":
                        testObject = getUserLogin();
                        break;
                    case "uacc":
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                        };
                    case "uacco":
                        string emptyLogin = JsonConvert.SerializeObject(testObject);
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(emptyLogin, System.Text.Encoding.UTF8, "application/json")
                        };
                    case "463adffe-61a5-db11-882c-000000000000":
                        testObject = getTenantIncidentDetail();
                        break;
                    case "00000000-0000-0000-0000-000000000000":
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                        };
                    case "5":
                        testObject = groupTrayResponseDetail();
                        break;
                    case "0912359/05":
                        testObject = getAccountNotification();
                        break;
                    case "0912987/05":
                        testObject = getDebItemDetail();
                        break;
                    case "0902359/04":
                        string emptyDebItem = JsonConvert.SerializeObject(testObject);
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(emptyDebItem, System.Text.Encoding.UTF8, "application/json")
                        };
                    case "1":
                        testObject = getAccountsAndNotification();
                        break;
                    case "2":
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                        };

                    case "N16 5DH":
                        testObject = GetAreaPatchResponseDetail();
                        break;
                    case "N16 5DC":
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                        };
                    case "throw500":
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                        };
                    case "3":
                        testObject = getAllOfficersPerArea();
                        break;
                    case "10":
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
                        };
                    case "11":
                        string emptyAreaOfficers = JsonConvert.SerializeObject(testObject);
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(emptyAreaOfficers, System.Text.Encoding.UTF8, "application/json")
                        };
                    case "99999999":
                        testObject = getCautionaryAlerts();
                        break;
                    case "1000089925":
                        testObject = getAllContactsByUPRN();
                        break;
                    case "notAssignedToPatchOrArea":
                        testObject = getAllUnassginedOfficers();
                        break;
                    case "Test user Name":
                        testObject = createuseraccount();
                        break;
                    case "Test user Name conflict":
                        testObject = useraccountexist();
                        break;
                }

                string jsonString = JsonConvert.SerializeObject(testObject);
                HttpResponseMessage responsMessage = new HttpResponseMessage(httpStatusCode) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

                return responsMessage;
            }
            else
            {
                var response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
        }

        private object getAllContactsByUPRN()
        {
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
            return alertsDictionary;
        }

        public async Task<HttpResponseMessage> SendAsJsonAsync<T>(HttpClient client, HttpMethod method, string requestUri, T value)
        {

            if (value.ToString().Contains("internal error"))
            {
                HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(String.Empty, System.Text.Encoding.UTF8, "application/json")
                };
                return responsMessage;
            }
           else if (value.ToString().Contains("test update contact name"))
            {
                var ctact = getContact();
                string jsonString = JsonConvert.SerializeObject(ctact);
                HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
                };
                return responsMessage;
            }
           else if (value.ToString().Contains("Enquiry Created By Estate Officer"))
            {
                var service = getServiceRequest();
                string jsonString = JsonConvert.SerializeObject(service);
                HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
                };
                return responsMessage;
            }
            else if (value.ToString().Contains("incidentid") || value.ToString().Contains("estate officer patch"))
            {

                HttpResponseMessage response = await getAnnotation();
                return response;
            }

           else if (value.ToString().Contains("2a7912b3-b6e0-e711-810e-70106bbbbbbb"))
            {

                HttpResponseMessage response = await getAnnotation();
                return response;
            }

           else if (value.ToString().Contains("2a7912b3-b6e0-e711-810e-70106bbbbbbb"))
            {

                HttpResponseMessage response = await getAnnotation();
                return response;
            }
           else if (value.ToString().Contains("testing update notification"))
            {
                var service = getUpdateNotification();
                string jsonString = JsonConvert.SerializeObject(service);
                HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
                };
                return responsMessage;
            }
           else if (value.ToString().Contains("23456788-8e23-e811-8120-70106f5678181"))
            {
                HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(String.Empty, System.Text.Encoding.UTF8, "application/json")
                };
                return responsMessage;
            }
            else if (value.ToString().Contains("OfficerId-e811-8120-70106f5678181"))
            {
                var updatedPatch = getUpdatedPatch();
                string jsonString = JsonConvert.SerializeObject(updatedPatch);
                HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
                };
                return responsMessage;
            }
            else if (value.ToString().Contains("statecode"))
            {
                var officerloginAccount = getupdateOfficerLoginAccountdisabled();
                string jsonString = JsonConvert.SerializeObject(officerloginAccount);
                HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
                };
                return responsMessage;
            }
            else //default
            {
                HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(String.Empty, System.Text.Encoding.UTF8, "application/json")
                };
                return responsMessage;
            }


            
        }

        public Task<bool> UpdateObject(HttpClient client, string requestUri, JObject updateObject)
        {

            if (updateObject != null)
                return Task.Run(() => true);
            else
                return Task.Run(() => false);
        }

        public async Task<HttpResponseMessage> deleteObjectAPIResponse(HttpClient client, string query)
        {

            if (query.Contains("9999"))
            {

                HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(String.Empty, System.Text.Encoding.UTF8, "application/json")
                };
                return responseMessage;
            }
            else
            {
                HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(String.Empty, System.Text.Encoding.UTF8, "application/json")
                };
                return responseMessage;
            }
        }


        #region Accounts

        private object getAccountAndAddress()
        {
            var Accountdictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["housing_u_saff_rentacc"] = "228003470";
            value["aj_x002e_housing_post_code"] = "N1 6JJ";
            value["aj_x002e_housing_short_address"] = "St Johns Estate  82 Crondall Court";
            listJObject.Add(value);
            Accountdictionary.Add("value", listJObject);

            return Accountdictionary;
        }

        #endregion

        #region Accounts

        private object getAccountDetails()
        {
            var accountDetails = new Dictionary<string, List<JObject>>();
            var listJObject = new List<JObject>();
            var value = new JObject();
            value["accountid"] = "93d621ae-46c6-e711-8111-70106ssssssss";
            value["housing_tag_ref"] = "010000/01";
            value["housing_anticipated"] = "0.0";
            value["housing_prop_ref"] = "00000008";
            value["housing_cur_bal"] = "564.35";
            value["housing_rent"] = "114.04";
            value["housing_house_ref"] = "010001";
            value["housing_directdebit"] = null;
            value["contact1_x002e_hackney_personno"] = null;
            value["contact1_x002e_hackney_responsible"] = false;
            value["contact1_x002e_hackney_title"] = "Mr";
            value["contact1_x002e_firstname"] = "TestA";
            value["contact1_x002e_lastname"] = "TestB";
            value["contact1_x002e_address1_postalcode"] = "E8 2HH";
            value["contact1_x002e_address1_line1"] = "Maurice";
            value["contact1_x002e_address1_line2"] = "Bishop";
            value["contact1_x002e_address1_line3"] = "House";
            value["customeraddress2_x002e_addresstypecode"] = 1;
            listJObject.Add(value);
            accountDetails.Add("value", listJObject);
            return accountDetails;
        }

        #endregion Accounts

        #region AccountAgreement

        private object getAccountAgreement()
        {
            var agreement = new Dictionary<string, List<JObject>>();
            var listJObject = new List<JObject>();
            var value = new JObject
            {
                ["housing_aragdet_amount"] = "4.18",
                ["housing_aragdet_frequency"] = "1",
                ["housing_paymentagreementid"] = "b1e26ae1 - 12d4 - e711 - 8114 - 70106faa6a11"
            };
            listJObject.Add(value);
            agreement.Add("value", listJObject);

            return agreement;
        }

        #endregion AccountAgreement

        #region Transactions

        private object getTransactions()
        {
            var Transactionsdictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["housing_tag_ref"] = "000000/01";
            value["housing_prop_ref"] = "00013000";
            value["housing_rtrans_sid"] = "155000000";
            value["housing_house_ref"] = "000015";
            value["housing_trans_type"] = "RHB";
            value["housing_postdate"] = "2016-10-08T23:00:00Z";
            value["housing_real_value"] = "-121.16";
            value["housing_rtransid"] = "1656129d-0a4a-e711-80f7-11111111111";
            value["housing_deb_desc"] = "";


            listJObject.Add(value);
            Transactionsdictionary.Add("value", listJObject);

            return Transactionsdictionary;
        }

        #endregion

        #region accountcreation
        private object createuseraccount()
        {
            var useraccount = new Dictionary<string, List<JObject>>();
            var listJObject = new List<JObject>();            
            useraccount.Add("value", null);
            return useraccount;
        }

        private object useraccountexist()
        {
            var useraccount = new Dictionary<string, List<JObject>>();
            var listJObject = new List<JObject>();
            var value = new JObject();
            value["@odata.etag"] = "W/\"22244265\"";
            value["hackney_estateofficerloginid"] = "9eb60620-cd1b-e811-8118-70106faa6a31";
            value["hackney_username"] = "tnolan";
            value["statecode"] = "0";
            value["statuscode"] = "1";
            listJObject.Add(value);
            useraccount.Add("value", listJObject);

            return useraccount;
        }
        #endregion



        public async Task<HttpResponseMessage> postHousingAPI(HttpClient client, string query, JObject jobject)
        {


            if (jobject.ToString().Contains("internal error"))
            {
                HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(String.Empty, System.Text.Encoding.UTF8, "application/json")
                };
                return responsMessage;
            }
            if (jobject.ToString().Contains("Test First"))
            {
                var officerAccount = getCreateOfficerLoginAccount();
                string jsonString = JsonConvert.SerializeObject(officerAccount);
                HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
                };
                return responsMessage;
            }
            if (jobject.ToString().Contains("Test user Name"))
            {
                var officerAccount = getCreateOfficerLoginAccount();
                string jsonString = JsonConvert.SerializeObject(officerAccount);
                HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
                };
                return responsMessage;
            }
            if (jobject.ToString().Contains("test first name"))
            {
                var contact = getCreateContact();
                string jsonString = JsonConvert.SerializeObject(contact);
                HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
                };
                return responsMessage;
            }
            if (jobject.ToString().Contains("ticketnumber"))
            {
                var tmi = getTenancyManagementRequest();
                string jsonString = JsonConvert.SerializeObject(tmi);
                HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
                };
                Guid guid = new Guid("c1f72d01-28dc-e711-8115-70106aaaaaaa");
                responsMessage.Headers.Add("OData-EntityId", "(" + guid + ")");
                return responsMessage;
            }
            if (jobject.ToString().Contains("notificationTest"))
            {
                var notification = getCreateNotification();
                string jsonString = JsonConvert.SerializeObject(notification);
                HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
                };
                return responsMessage;
            }
            if (jobject.ToString().Contains("1000089925"))
            {
                var alert = getCreateAlert();
                string jsonString = JsonConvert.SerializeObject(alert);
                HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json")
                };
                return responsMessage;
            }

            return null;

        }

        private object getCreateOfficerLoginAccount()
        {
            JObject result = JObject.Parse($"{{\"hackney_estateofficerid\": \"fa164f0b-a031-e811-811a-70106faaf8c1\",  \"hackney_name\": \"Test First And Last\",  \"hackney_firstname\": \"Test First\",  \"hackney_lastname\": \"Test_Last\",\"hackney_emailaddress\": \"flast@test.com\", \"statecode\": \"0\",\"statuscode\": \"1\", \"hackney_estateofficerloginid\": \"fb164f0b-a031-e811-811a-70106faaf8c1\", \"hackney_username\": \"Test user Name\", \"statecode\": \"0\",\"statuscode\": \"1\"}}");

            return result;
        }
        private object getDebItemDetail()
        {
            var debItemList = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();

            value["housing_deb_code"] = "DGR";
            value["housing_deb_value"] = 0.75;
            value["housing_eff_date"] = "2017-04-01T00:00:00Z";
            value["housing_prop_ref"] = "00013593";
            value["housing_tag_ref"] = "0912987/05";
            value["housing_term_date"] = "2018-03-31T00:00:00Z";
            value["housing_debitemid"] = "7c6f1a80-06fa-e711-8110-70106faaf8c1";

            listJObject.Add(value);
            debItemList.Add("value", listJObject);

            return debItemList;
        }
        private object getAccountNotification()
        {
            var accountNotification = new JObject();
            accountNotification["housing_directdebit"] = "0906759/0005";
            accountNotification["housing_u_saff_rentacc"] = "5705195010";
            accountNotification["accountid"] = "b2a6ad94-3c9d-e711-80ff-70106faa4841";
            accountNotification["housing_rentaccountnotification1_x002e_housing_rentaccountnotificationid"] = "8aaffb68-c9a2-e711-8102-e0071b7fe041";
            accountNotification["housing_rentaccountnotification1_x002e_housing_isnotification"] = true;
            accountNotification["housing_rentaccountnotification1_x002e_housing_phone"] = "8967543210";
            accountNotification["housing_rentaccountnotification1_x002e_housing_email"] = "test@test.com";

            var loginDictionary = new Dictionary<string, List<dynamic>>();
            List<dynamic> listJObject = new List<dynamic>();
            listJObject.Add(accountNotification);
            loginDictionary.Add("value", listJObject);
            return loginDictionary;
        }
        private object getCreateNotification()
        {
            return new
            {
                email = "test@test.com",
                telephone = "1234567890",
                areNotificationsOn = false,
                tagReference = "000000/01",
                accountType = 1
            };
        }

        private object getCreateAlert()
        {
            return new
            {
                alertContactId = "o999993-e716-e811-811e-788888aa6a11",
                alertUprn = "1000089925",
                alertCautionaryAlertType = new List<string> { "3" },
                createdOn = "2018-03-13"
            };
        }

       private object getUpdateNotification()
        {
            return new
            {
                email = "testing update notification",
                telephone = "1234567899",
                areNotificationsOn = true
            };
        }

        private object getServiceRequest()
        {
            var request = new
            {
                Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                ContactId = "463adffe-61a5-db11-882c-000000000000",
                Title = "Tenancy Management",
                Description = "Enquiry Created By Estate Officer",
                RequestCallback = true,
                incidentid = "incidentid",
                ticketnumber = "ticketnumber",
                annotationid = "63a0e5b9-88df-e311-b8e5-6c3be5ccccccc"
            };


            return request;

        }

        private object getTenancyManagementRequest()
        {
            var resoponse = new TenancyManagement
            {
                interactionId = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                contactId = "463adffe-61a5-db11-882c-000000000000",
                enquirySubject = "100000005",
                estateOfficerId = "284216e9-d365-e711-80f9-70106aaaaaaa",
                subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                adviceGiven = "Housing Repair Inquiry",
                estateOffice = "5",
                source = "1",
                natureofEnquiry = "3",
                officerPatchId = "be77dd44-b005-e811-811c-70106aaaaaa",
                areaName = "3",
                managerId = "ae7b4690-b005-e811-811c-70106fffffff",
                ServiceRequest = new CRMServiceRequest
                {
                    TicketNumber = "ticketnumber",
                    Id = "incidentid",
                    Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                    ContactId = "463adffe-61a5-db11-882c-000000000000",
                    Title = "Tenancy Management",
                    Description = "Enquiry Created By Estate Officer"
                }

            };

            return resoponse;

        }

        public JObject getCreateContact()
        {

            var ctact = new JObject();
            ctact["firstname"] = "test first name";
            ctact["lastname"] = "test last name";
            ctact["fullname"] = "test first name test last name";
            ctact["emailaddress1"] = "test email";
            ctact["housing_address1"] = "maurice bishop house";
            ctact["housing_address2"] = null;
            ctact["housing_address3"] = null;
            ctact["housing_address_city"] = "london";
            ctact["housing_address_postcode"] = "e81hh";
            ctact["housing_telephone1"] = "0987654321";
            ctact["housing_telephone2"] = null;
            ctact["housing_telephone3"] = null;
            ctact["housing_hackney_larn"] = null;
            ctact["contactid"] = "463adffe-61a5-db11-882c-000000000000";
            ctact["ownerid@odata.bind"] = "/systemusers(e1207267-40a8-e711-810c-70106faa6a11)";
            ctact["new_api_user@odata.bind"] = "/new_api_users(de98e4b6-15dc-e711-8115-11111111)";

            return ctact;
        }
        public JObject getContact()
        {
            var ctact = new JObject();
            ctact["firstname"] = "test first name";
            ctact["lastname"] = "test last name";
            ctact["fullname"] = "test first name test last name";
            ctact["emailaddress1"] = "test email";
            ctact["housing_contactupdatedbyapiuserid@odata.bind"] = "/new_api_users(e64fee7c-2bba-e711-8106-1111111111)";

            return ctact;
        }

        private async Task<HttpResponseMessage> getAnnotation()
        {
            var annotationDictionary = new Dictionary<string, string>();
            annotationDictionary.Add("annotationid", "63a0e5b9-88df-e311-b8e5-6c3be5ccccccc");

            string jsonString = JsonConvert.SerializeObject(annotationDictionary);
            HttpResponseMessage responsMessage = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };

            return responsMessage;

        }

        private object getUserLogin()
        {
            var loginDictionary = new Dictionary<string, List<dynamic>>();

            var loginResponse = new
            {
                hackney_estateofficerloginid = "login8f6a9cba",
                _hackney_officerloginid_value = "OfficerId70106faa6a31",
                hackney_password = "Password",
                hackney_username = "UserName",
                hackney_estateofficer1_x002e_hackney_estateofficerid = "EstateOfficerId454345",
                hackney_estateofficer1_x002e_hackney_lastname = "Smith",
                hackney_estateofficer1_x002e_hackney_firstname = "Test",
                officerPatchId = "PatchId7yo983o01",
                hackney_estateofficer1_x002e_hackney_name = "Test Smith",
                managerId = null as object,
                OfficermanagerId = "OfficermanagerId",
                OfficerAreaId = "AreaId1",
                AreaId = null as object,

            };

            List<dynamic> listJObject = new List<dynamic>();

            listJObject.Add(loginResponse);

            loginDictionary.Add("value", listJObject);

            return loginDictionary;
        }

        private object getTenantIncidentDetail()
        {
            var TenancyManagement = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();


            value["hackney_name"] = "CAS-00059-000000";
            value["statecode"] = 0;
            value["contact3_x002e_telephone1"] = "123";
            value["hackney_tenancymanagementinteractionsid"] = "d9f0fd60-b5e0-e711-810f-111111";
            value["_housing_contact_value"] = "463adffe-61a5-db11-882c-000000000000";
            value["createdon@OData.Community.Display.V1.FormattedValue"] = "14/12/2017 09:58";
            value["createdon"] = "2017-12-14T09:58:49Z";
            value["hackney_estateoffice@OData.Community.Display.V1.FormattedValue"] = "Homerton Housing Neighbourhood";
            value["hackney_enquirysubject@OData.Community.Display.V1.FormattedValue"] = "Apply for Joint Tenancy";
            value["hackney_enquirysubject"] = "100000005";
            value["hackney_natureofenquiry@OData.Community.Display.V1.FormattedValue"] = "Estate Managment";
            value["hackney_natureofenquiry"] = "3";
            value["_hackney_estateofficer_createdbyid_value"] = "284216e9-d365-e711-80f9-70106aaaaaaa";
            value["annotation2_x002e_annotationid"] = "b6521622-54e6-e711-8111-7010bbbbbbbb";
            value["annotation2_x002e_createdon@OData.Community.Display.V1.FormattedValue"] = "21/12/2017 13:37";
            value["annotation2_x002e_createdon"] = "2017-12-21T13:37:49Z";
            value["annotation2_x002e_notetext"] = "Testing closure  at 21/12/2017 13:37:18 by  Test dev";
            value["_hackney_estateofficer_createdbyid_value@OData.Community.Display.V1.FormattedValue"] = "Test Test";
            value["_hackney_incidentid_value"] = "39141263-b0e0-e711-810a-e0071bbbbbbb";
            value["_hackney_managerpropertypatchid_value"] = "AreaManager28645uyo980";
            value["ManagerFirstName"] = "Area";
            value["ManagerLastName"] = "Manager Name";
            value["_hackney_estateofficerpatchid_value"] = "OfficerPatch9684056oi046";
            value["OfficerFirstName"] = "Officer";
            value["OfficerLastName"] = "Patch Name";
            value["hackney_areaname@OData.Community.Display.V1.FormattedValue"] = "Homerton";
            value["hackney_handleby@OData.Community.Display.V1.FormattedValue"] = "Estate Officer";
            value["incident1_x002e_housing_requestcallback"] = false;
            value["_hackney_contactid_value"] = "ContactId9486954o93";
            value["_hackney_contactid_value@OData.Community.Display.V1.FormattedValue"] = "Contact name";
            value["contact3_x002e_address1_postalcode"] = "E8 2HH";
            value["contact3_x002e_address1_line1"] = "Maurice Bishop House";
            value["contact3_x002e_address1_line2"] = "Hackney";
            value["contact3_x002e_address1_line3"] = null;
            value["contact3_x002e_address1_city"] = "London";
            value["contact3_x002e_birthdate"] = "01/01/1950";
            value["contact3_x002e_emailaddress1"] = "test@test.com";
            value["contact3_x002e_hackney_larn"] = "LARN834210";
            value["contact3_x002e_createdon"] = "null";


            listJObject.Add(value);
            TenancyManagement.Add("value", listJObject);

            return TenancyManagement;
        }

        private object groupTrayResponseDetail()
        {
            var TenancyManagement = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();

            value["hackney_name"] = "CAS-00059-000000";
            value["statecode"] = 0;
            value["contact3_x002e_telephone1"] = "123";
            value["hackney_tenancymanagementinteractionsid"] = "d9f0fd60-b5e0-e711-810f-111111";
            value["_housing_contact_value"] = "463adffe-61a5-db11-882c-000000000000";
            value["createdon@OData.Community.Display.V1.FormattedValue"] = "14/12/2017 09:58";
            value["createdon"] = "2017-12-14T09:58:49Z";
            value["hackney_estateoffice@OData.Community.Display.V1.FormattedValue"] = "Homerton Housing Neighbourhood";
            value["hackney_enquirysubject@OData.Community.Display.V1.FormattedValue"] = "Apply for Joint Tenancy";
            value["hackney_enquirysubject"] = "100000005";
            value["hackney_natureofenquiry@OData.Community.Display.V1.FormattedValue"] = "Estate Managment";
            value["hackney_natureofenquiry"] = "3";
            value["_hackney_estateofficer_createdbyid_value"] = "284216e9-d365-e711-80f9-70106aaaaaaa";
            value["annotation2_x002e_annotationid"] = "b6521622-54e6-e711-8111-7010bbbbbbbb";
            value["annotation2_x002e_createdon@OData.Community.Display.V1.FormattedValue"] = "21/12/2017 13:37";
            value["annotation2_x002e_createdon"] = "2017-12-21T13:37:49Z";
            value["annotation2_x002e_notetext"] = "Testing closure  at 21/12/2017 13:37:18 by  Test dev";
            value["_hackney_estateofficer_createdbyid_value@OData.Community.Display.V1.FormattedValue"] = "Test Test";
            value["_hackney_incidentid_value"] = "39141263-b0e0-e711-810a-e0071bbbbbbb";
            value["_hackney_managerpropertypatchid_value"] = "AreaManager28645uyo980";
            value["ManagerFirstName"] = "Area";
            value["ManagerLastName"] = "Manager Name";
            value["_hackney_estateofficerpatchid_value"] = "OfficerPatch9684056oi046";
            value["OfficerFirstName"] = "Officer";
            value["OfficerLastName"] = "Patch Name";
            value["hackney_areaname@OData.Community.Display.V1.FormattedValue"] = "Homerton";
            value["hackney_handleby@OData.Community.Display.V1.FormattedValue"] = "Estate Officer";
            value["incident1_x002e_housing_requestcallback"] = false;
            value["_hackney_contactid_value"] = "ContactId9486954o93";
            value["_hackney_contactid_value@OData.Community.Display.V1.FormattedValue"] = "Contact name";
            value["contact3_x002e_address1_postalcode"] = "E8 2HH";
            value["contact3_x002e_address1_line1"] = "Maurice Bishop House";
            value["contact3_x002e_address1_line2"] = "Hackney";
            value["contact3_x002e_address1_line3"] = null;
            value["contact3_x002e_address1_city"] = "London";
            value["contact3_x002e_birthdate"] = "01/01/1950";
            value["contact3_x002e_emailaddress1"] = "test@test.com";
            value["contact3_x002e_hackney_larn"] = "LARN834210";
            value["accountCreatedOn"] = null;
            listJObject.Add(value);
            TenancyManagement.Add("value", listJObject);

            return TenancyManagement;
        }

        private object GetAreaPatchResponseDetail()
        {
            var areaPatchResponseObj = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["hackney_areaname"] = "1";
            value["hackney_areaname@OData.Community.Display.V1.FormattedValue"] = "Stamford Hill";
            value["hackney_propertyreference"] = "00000004";
            value["hackney_postcode"] = "N16 5DH";
            value["hackney_llpgref"] = "10000000000";
            value["hackney_ward"] = "2";
            value["hackney_ward@OData.Community.Display.V1.FormattedValue"] = "SpringField";
            value["_hackney_estateofficerpropertypatchid_value"] = "e2420bff-b6c9-4d42-80f3-b8dcccccccc";
            value["OfficerFirstName"] = "Test";
            value["OfficerLastName"] = "Officer";
            value["_hackney_managerpropertypatchid_value"] ="906ebd93-ee3c-47ee-8c88-46638bqqqqqq";
            value["ManagerFirstName"] = "Test";
            value["ManagerLastName"] = "Manager";
            listJObject.Add(value);
            areaPatchResponseObj.Add("value", listJObject);
            return areaPatchResponseObj;
        }

        private object getAccountsAndNotification()
        {
            var accountsAndNotificationsDictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["housing_cur_bal"] = "-28.80";
            value["housing_rentaccountnotification1_x002e_housing_phone"] = "123456789";
            value["housing_rentaccountnotification1_x002e_housing_isnotification"] = true;
            value["housing_paymentagreement2_x002e_housing_paymentagreementid"] = "paymentAgreementIdb847a815-de0b-e811-8114";
            value["housing_paymentagreement2_x002e_housing_aragdet_enddate"] = DateTime.Parse("2019-05-10T00:00:00Z");
            listJObject.Add(value);

            accountsAndNotificationsDictionary.Add("value", listJObject);

            return accountsAndNotificationsDictionary;
        }

        private object getAllOfficersPerArea()
        {
            var officerDictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["hackney_propertyareapatchid"] = "a692a27c-b205-e811-811c-70106wwwwww";
            value["_hackney_estateofficerpropertypatchid_value@OData.Community.Display.V1.FormattedValue"] = "Test Officer Patch";
            value["_hackney_estateofficerpropertypatchid_value"] = "be77dd44-b005-e811-811c-70106aaaaaa";
            value["hackney_name"] = "Central Panel";
            value["hackney_areaname@OData.Community.Display.V1.FormattedValue"] = "Central Panel";
            value["hackney_areaname"] = "3";
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

            return officerDictionary;
        }

        private object getCautionaryAlerts()
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

            return cautionaryAlertdictionary;
        }

        private object getUpdatedPatch()
        {
            var value = new JObject();
            value["hackney_estateofficerpatchid"] = "be77dd44-b005-e811-811c-0000000";
            value["hackney_name"] = "Patch 6-2";
          
            return value;
        }


        private object getAllUnassginedOfficers()
        {
            var dictionary = new Dictionary<string, List<JObject>>();
            List<JObject> listJObject = new List<JObject>();
            var value = new JObject();
            value["hackney_firstname"] = "Test First Name";
            value["hackney_lastname"] = "Test Last Name";
            value["hackney_name"] = "Test First Name Test Last Name";
            value["hackney_estateofficerid"] = "02345-o0e93o91o-545o902";

            listJObject.Add(value);

            dictionary.Add("value", listJObject);
            return dictionary;
        }

        private object getupdateOfficerLoginAccountdisabled()
        {
            JObject result = JObject.Parse($"{{\"hackney_estateofficerid\": \"fa164f0b-a031-e811-811a-70106faaf8c1\",  \"hackney_name\": \"Test First And Last\",  \"hackney_firstname\": \"Test First\",  \"hackney_lastname\": \"Test_Last\", \"hackney_emailaddress\": \"flast@test.com\", \"statecode\": \"1\",\"statuscode\": \"2\", \"hackney_estateofficerloginid\": \"fb164f0b-a031-e811-811a-70106faaf8c1\", \"hackney_username\": \"Test user Name\", \"statecode\": \"1\",\"statuscode\": \"2\"}}");

            return result;
        }
    }
}
