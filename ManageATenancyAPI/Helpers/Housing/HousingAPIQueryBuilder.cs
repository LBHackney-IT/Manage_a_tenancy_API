using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ManageATenancyAPI.Formatters;


namespace ManageATenancyAPI.Helpers.Housing
{
    public static class HousingAPIQueryBuilder
    {

        public static string getAccountsByParisReferenceAndPostcodeQuery(string parisReference, string postcode)
        {

            StringBuilder query = new StringBuilder();
            query.Append("/api/data/v8.2/accounts?fetchXml=");
            query.Append(
                "<fetch version = '1.0' output-format = 'xml-platform' mapping = 'logical' distinct = 'true' >");
            query.Append("<entity name = 'account'>");
            query.Append("<attribute name = 'housing_u_saff_rentacc'/>");
            query.Append("<filter type = 'and' >");
            query.Append("<condition attribute = 'housing_u_saff_rentacc' operator= 'eq' value = '" +
                         parisReference.Trim() + "' />");
            query.Append("</filter >");
            query.Append("<link-entity name = 'customeraddress' from = 'parentid' to = 'accountid' alias = 'aj' >");
            query.Append("<attribute name = 'housing_post_code' />");
            query.Append("<attribute name = 'housing_short_address' /><attribute name = 'addresstypecode' />");
            query.Append("<filter type = 'and' >");
            query.Append(
                "<condition attribute = 'housing_post_code' operator= 'eq' value = '" + postcode.Trim() + "'/>");
            query.Append("</filter>");
            query.Append("</link-entity>");
            query.Append("</entity>");
            query.Append("</fetch>");
            return query.ToString();

        }

        public static string getAccountDetailsByTagorParisReferenceQuery(string reference)
        {
            var query = new StringBuilder();
            var fetchXml = $@"
            <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
              <entity name='account'>
                <attribute name='accountid' />
                <attribute name='housing_tag_ref' />
                <attribute name='housing_anticipated' />
                <attribute name='housing_prop_ref' />
                <attribute name='housing_cur_bal' />
                <attribute name='housing_rent' />
                <attribute name='housing_house_ref' />
                <attribute name='housing_directdebit' />
                <filter type='and'>";
                if (!reference.Contains("/"))
                {
                    fetchXml = fetchXml + "<condition attribute = 'housing_u_saff_rentacc' operator= 'eq'  value='" + reference + "' />";
                }
                else
                {
                    fetchXml = fetchXml + "<condition attribute = 'housing_tag_ref' operator= 'eq' value='" + reference + "' />"; 
                }
                fetchXml = fetchXml + $@"
                </filter>
               <link-entity name='contact' from='parentcustomerid' to='accountid' link-type='inner' >
                  <attribute name='hackney_responsible' />
                  <attribute name='hackney_personno' />
                  <attribute name='hackney_title' />
                  <attribute name='firstname' />
                  <attribute name='lastname' />
                  <attribute name='address1_postalcode' />
                  <attribute name='address1_line3' />
                  <attribute name='address1_line1' />
                  <attribute name='address1_line2' />
                </link-entity>
              </entity>
            </fetch>";

            query.Append("/api/data/v8.2/accounts?fetchXml=" + fetchXml);
            return query.ToString();

        }

        public static string GetHousingPaymentagreementQuery(string TagRef)
        {
            return
                "/api/data/v8.2/housing_paymentagreements?$select=housing_aragdet_amount,housing_aragdet_frequency&$filter=housing_tag_ref eq" +
                " " + "'" + TagRef + "'";
        }

        public static string getTransactionsByTagReference(string tagReference)
        {

            new DateTime(DateTime.Today.AddYears(-1).Year, 4, 1);


            StringBuilder query = new StringBuilder();
            query.Append(
                "/api/data/v8.2/housing_rtranses?$select=housing_real_value,housing_tag_ref,housing_prop_ref,housing_house_ref," +
                "housing_prop_ref,housing_tag_ref,housing_trans_type&$orderby=housing_postdate desc,housing_real_value," +
                "housing_deb_desc&$filter=");
            query.Append("housing_tag_ref eq '" + tagReference.Trim() + "'");
            query.Append("and Microsoft.Dynamics.CRM.Between(PropertyName='housing_postdate',PropertyValues=[");
            query.Append(
                "%22" + DateTimeFormatter.FormatDateTimeToUtc(new DateTime(DateTime.Today.AddYears(-1).Year, 4, 1)) +
                "%22" + "," +
                "%22" + DateTimeFormatter.FormatDateTimeToUtc(DateTime.Today.AddDays(1)) + "%22])");
            return query.ToString();
        }

        
        public static string getAuthenticatedUserQuery(string username, string password)
        {
            var query = new StringBuilder();
            var fetchData = new
            {
                hackney_password = password,
                hackney_username = username
            };
            var fetchXml = $@"
            <fetch top='1'>
              <entity name='hackney_estateofficerlogin'>
                <attribute name='hackney_username' />
                <attribute name='hackney_officerloginid' />
                <attribute name='hackney_password' />
                <attribute name='hackney_estateofficerloginid' />
                <filter type='and'>
                  <condition attribute='hackney_password' operator='eq' value='{fetchData.hackney_password}'/>
                  <condition attribute='hackney_username' operator='eq' value='{fetchData.hackney_username}'/>
                  <condition attribute = 'statecode' operator= 'eq' value = '0'/>
                </filter>
                <link-entity name='hackney_estateofficer' from='hackney_estateofficerid' to='hackney_officerloginid'>
                  <attribute name='hackney_lastname' />
                  <attribute name='hackney_estateofficerid' />
                  <attribute name='hackney_firstname' />
                  <attribute name = 'hackney_name' />
                  <link-entity name='hackney_estateofficerpatch' from='hackney_patchid' to='hackney_estateofficerid' link-type='outer'>
                    <attribute name='hackney_estateofficerpatchid'  alias='officerPatchId' />
                    <link-entity name='hackney_propertyareapatch' from='hackney_estateofficerpropertypatchid' to='hackney_estateofficerpatchid' link-type='outer'>
                      <attribute name='hackney_areaname' alias='OfficerAreaId' />
                      <attribute name='hackney_managerpropertypatchid' alias='OfficermanagerId' />
                    </link-entity>
                  </link-entity>
                  <link-entity name='hackney_estatemanagerarea' from='hackney_managerareaid' to='hackney_estateofficerid' link-type='outer'>
                    <attribute name='hackney_estatemanagerareaid' alias='managerId' />
                    <link-entity name='hackney_propertyareapatch' from='hackney_managerpropertypatchid' to='hackney_estatemanagerareaid' link-type='outer'>
                      <attribute name='hackney_areaname' alias='AreaId' />
                    </link-entity>
                  </link-entity>
                </link-entity>
              </entity>
            </fetch>";
            query.Append("/api/data/v8.2/hackney_estateofficerlogins?fetchXml="+ fetchXml);
            return query.ToString();

        }

        public static string PostTenancyManagementInteractionQuery()
        {
            return
                "/api/data/v8.2/hackney_tenancymanagementinteractionses?$select=_hackney_contactid_value,hackney_enquirysubject,_hackney_estateofficer_createdbyid_value,hackney_handleby,_hackney_incidentid_value,hackney_name,hackney_natureofenquiry,_hackney_subjectid_value,hackney_tenancymanagementinteractionsid";
        }
        public static string PostETRAMeetingQuery()
        {
            return
                "/api/data/v8.2/hackney_tenancymanagementinteractionses?$select=hackney_tenancymanagementinteractionsid";
        }

        public static string PostIncidentQuery()
        {
            return
                "/api/data/v8.2/incidents()?$select=_customerid_value,description,_subjectid_value,ticketnumber,title";


        }

        public static string updateContactQuery(string id)
        {
            return "api/data/v8.2/contacts("+id+")?$select=firstname,lastname,fullname,birthdate,emailaddress1,address1_line3,address1_line1,address1_line2,address1_city,address1_postalcode,telephone1,telephone2,telephone3";

        }

        public static string closeIncidentQuery(string serviceRequestId)
        {
            return "/api/data/v8.2/incidentresolutions(" + new Guid(serviceRequestId.Trim()) + ")";
        }

        public static string updateInteractionQuery(string interactionId)
        {
            return "/api/data/v8.2/hackney_tenancymanagementinteractionses(" + new Guid(interactionId.Trim()) + ")";
        }

        public static string getTenancyInteractionDeatils(string contactId, string personType)
        {
            StringBuilder query = new StringBuilder();
      
            var fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' ><entity name='hackney_tenancymanagementinteractions' >
                <attribute name='hackney_contactid' />
                <attribute name='hackney_handleby' />
                <attribute name='hackney_areaname' />
                <attribute name='hackney_name' />
                <attribute name='statecode' />
                <attribute name='hackney_estateofficerpatchid' />
                <attribute name='hackney_natureofenquiry' />
                <attribute name='createdon' />
                <attribute name='hackney_estateofficer_createdbyid' />
                <attribute name='hackney_estateofficer_updatedbyid' />
                <attribute name='hackney_tenancymanagementinteractionsid' />
                <attribute name='hackney_enquirysubject' />
                <attribute name='hackney_managerpropertypatchid' />
                <attribute name='hackney_contactid' />
                <attribute name='hackney_handlebyname' />
                <attribute name='hackney_incidentid' />
                <attribute name='hackney_transferred' />
                <attribute name='hackney_process_stage' />
                <attribute name='hackney_household_interactionid' />
                <filter>";
            if (personType == "contact")
            {
                fetchXml = fetchXml + "<condition attribute='hackney_contactid' operator='eq' value='" + contactId +
                           "' />";
            }
            else if (personType == "manager")
            {
                fetchXml = fetchXml + "<condition attribute='hackney_managerpropertypatchid' operator='eq' value='" + contactId +
                             "' />";
            }
            else if (personType == "officer")
            {
                fetchXml = fetchXml + "<condition attribute='hackney_estateofficerpatchid' operator='eq' value='" + contactId +
                             "' />";
            }
            fetchXml = fetchXml + $@"
                        </filter>
                    <link-entity name='incident' from='incidentid' to='hackney_incidentid' link-type='inner' >
                        <attribute name='housing_requestcallback' />
                        <attribute name='incidentid' />
                        <link-entity name='annotation' from='objectid' to='incidentid' link-type='outer' >
                            <attribute name='subject' />
                            <attribute name='createdby' />
                            <attribute name='notetext' />
                            <attribute name='createdon' />
                            <attribute name='annotationid' />
                        </link-entity>
                    </link-entity>
                    <link-entity name='contact' from='contactid' to='hackney_contactid' link-type='outer' >
                        <attribute name='birthdate' />
                        <attribute name='address1_line3' />
                        <attribute name='address1_postalcode' />
                        <attribute name='address1_city' />
                        <attribute name='emailaddress1' />
                        <attribute name='telephone1' />
                        <attribute name='address1_line1' />
                        <attribute name='hackney_larn' />
                        <attribute name='hackney_uprn' />
                        <attribute name='address1_line2' />
                    </link-entity>
                    <link-entity name='hackney_estateofficerpatch' from='hackney_estateofficerpatchid' to='hackney_estateofficerpatchid' link-type='outer'>
                      <link-entity name='hackney_estateofficer' from='hackney_estateofficerid' to='hackney_patchid' link-type='outer'>
                        <attribute name='hackney_name' alias='OfficerFullName' />
                        <attribute name='hackney_lastname' alias='OfficerLastName' />
                        <attribute name='hackney_firstname' alias='OfficerFirstName' />
                      </link-entity>
                    </link-entity>
                    <link-entity name='hackney_estatemanagerarea' from='hackney_estatemanagerareaid' to='hackney_managerpropertypatchid' link-type='outer'>
                      <link-entity name='hackney_estateofficer' from='hackney_estateofficerid' to='hackney_managerareaid' link-type='outer'>
                        <attribute name='hackney_name' alias='ManagerFullName' />
                        <attribute name='hackney_lastname' alias='ManagerLastName' />
                        <attribute name='hackney_firstname' alias='ManagerFirstName' />
                      </link-entity>
                    </link-entity></entity></fetch>";
            query.Append("/api/data/v8.2/hackney_tenancymanagementinteractionses?fetchXml=" + HttpUtility.UrlEncode(fetchXml.Trim()));
            return query.ToString();
        }

        public static string getAreaTrayInteractions(string AreaId)
        {
            var query = new StringBuilder();

            var fetchData = new
            {
                hackney_areaname = AreaId
            };
            var fetchXml = $@"
            <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
              <entity name='hackney_tenancymanagementinteractions'>
                <attribute name='hackney_contactid' />
                <attribute name='hackney_handleby' />
                <attribute name='hackney_areaname' />
                <attribute name='hackney_name' />
                <attribute name='statecode' />
                <attribute name='hackney_estateofficerpatchid' />
                <attribute name='hackney_natureofenquiry' />
                <attribute name='createdon' />
                <attribute name='hackney_estateofficer_createdbyid' />
                <attribute name='hackney_estateofficer_updatedbyid' />
                <attribute name='hackney_tenancymanagementinteractionsid' />
                <attribute name='hackney_enquirysubject' />
                <attribute name='hackney_managerpropertypatchid' />
                <attribute name='hackney_contactid' />
                <attribute name='hackney_handlebyname' />
                <attribute name='hackney_incidentid' />
                <attribute name='hackney_transferred' />
                <attribute name='hackney_process_stage' />
                <attribute name='hackney_household_interactionid' />
                <filter>
                  <condition attribute='hackney_areaname' operator='eq' value='{fetchData.hackney_areaname}'/>
                </filter>
                <link-entity name='incident' from='incidentid' to='hackney_incidentid' link-type='inner'>
                  <attribute name='housing_requestcallback' />
                  <attribute name='incidentid' />
                  <link-entity name='annotation' from='objectid' to='incidentid' link-type='outer'>
                    <attribute name='subject' />
                    <attribute name='createdby' />
                    <attribute name='notetext' />
                    <attribute name='createdon' />
                    <attribute name='annotationid' />
                  </link-entity>
                </link-entity>
                <link-entity name='contact' from='contactid' to='hackney_contactid'>
                  <attribute name='birthdate' />
                  <attribute name='address1_line3' />
                  <attribute name='address1_postalcode' />
                  <attribute name='address1_city' />
                  <attribute name='emailaddress1' />
                  <attribute name='telephone1' />
                  <attribute name='address1_line1' />
                  <attribute name='hackney_larn' />
                  <attribute name='hackney_uprn' />
                  <attribute name='address1_line2' />
                </link-entity>
                <link-entity name='hackney_estateofficerpatch' from='hackney_estateofficerpatchid' to='hackney_estateofficerpatchid' link-type='outer'>
                  <link-entity name='hackney_estateofficer' from='hackney_estateofficerid' to='hackney_patchid' link-type='outer'>
                    <attribute name='hackney_name' alias='OfficerFullName' />
                    <attribute name='hackney_lastname' alias='OfficerLastName' />
                    <attribute name='hackney_firstname' alias='OfficerFirstName' />
                  </link-entity>
                </link-entity>
                <link-entity name='hackney_estatemanagerarea' from='hackney_estatemanagerareaid' to='hackney_managerpropertypatchid' link-type='outer'>
                  <link-entity name='hackney_estateofficer' from='hackney_estateofficerid' to='hackney_managerareaid' link-type='outer'>
                    <attribute name='hackney_name' alias='ManagerFullName' />
                    <attribute name='hackney_lastname' alias='ManagerLastName' />
                    <attribute name='hackney_firstname' alias='ManagerFirstName' />
                  </link-entity>
                </link-entity>
              </entity>
            </fetch>";

            query.Append("/api/data/v8.2/hackney_tenancymanagementinteractionses?fetchXml=" + fetchXml);

            return query.ToString();

        }

        public static string PostContactQuery()
        {
            return
                "/api/data/v8.2/contacts?$select=contactid,address1_city,address1_country,address1_line1,address1_line2,address1_line3,address1_postalcode,birthdate,emailaddress1,firstname,fullname,_hackney_createdby_value,jobtitle,lastname,telephone1,telephone2,hackney_uprn,createdon,hackney_larn,hackney_hackneyhomesid";
        }

        public static string PostNotificationQuery()
        {
            return
                "/api/data/v8.2/housing_rentaccountnotifications?$select=housing_tag_ref,housing_isnotification,housing_phone,housing_email,housing_accounttype,housing_rentaccountnotificationid";

        }

        public static string UpdateNotificationQuery(string notificationId)
        {
            return "/api/data/v8.2/housing_rentaccountnotifications(" + notificationId +
                   ")?$select=housing_tag_ref,housing_isnotification,housing_phone,housing_email,housing_accounttype";

        }

        public static string getAccountNotificationSummary(string tagReference)
        {
            StringBuilder query = new StringBuilder();
            query.Append("/api/data/v8.2/accounts?fetchXml=");
            query.Append(
                "<fetch version = '1.0' output-format = 'xml-platform' mapping = 'logical' distinct = 'true' >");
            query.Append("<entity name='account' >");
            query.Append("<attribute name='housing_directdebit' />");
            query.Append("<attribute name='housing_u_saff_rentacc' />");
            query.Append("<attribute name='accountid' />");
            query.Append("<filter>");
            query.Append("<condition attribute='housing_tag_ref' operator='eq' value='" + tagReference + "' />");
            query.Append("</filter>");
            query.Append(
                "<link-entity name='housing_rentaccountnotification' from='housing_rentaccountnotification' to='accountid' link-type='outer' >");
            query.Append("<attribute name='housing_phone' />");
            query.Append("<attribute name='housing_email' />");
            query.Append("<attribute name='housing_rentaccountnotificationid' />");
            query.Append("<attribute name='housing_isnotification' />");
            query.Append("</link-entity>");
            query.Append("</entity>");
            query.Append("</fetch>");

            return query.ToString();
        }

        public static string getDebItemSummary(string tagReference)
        {
            return
                "api/data/v8.2/housing_debitems?$select=housing_deb_code,housing_deb_value,housing_eff_date,housing_prop_ref,housing_tag_ref,housing_term_date&$filter=housing_tag_ref eq '" +
                tagReference + "'";
        }

        public static string getCRMCitizenSearch(string firstname, string surname, string[] address, string postcode)
        {

            var queryParams =
                "?$select=contactid,firstname,lastname,fullname,jobtitle,birthdate,emailaddress1,telephone1,telephone2,telephone3,hackney_larn,hackney_uprn,address1_name,address1_line1,address1_line2,address1_line3, address1_city,address1_postalcode,hackney_cautionaryalert,hackney_propertycautionaryalert,hackney_hackneyhomesid&";
            queryParams += "$filter=";
            bool fliter = false;
            if ((!string.IsNullOrEmpty(firstname) && !string.IsNullOrWhiteSpace(firstname)) &&
                (!string.IsNullOrEmpty(surname) && !string.IsNullOrWhiteSpace(surname)))
            {
                queryParams += "contains(firstname, '" + firstname + "') and contains(lastname, '" + surname + "')";
                fliter = true;
            }
            else if ((string.IsNullOrEmpty(firstname) || string.IsNullOrWhiteSpace(firstname)) &&
                     (!string.IsNullOrEmpty(surname) && !string.IsNullOrWhiteSpace(surname)))
            {
                queryParams += "contains(lastname, '" + surname + "')";
                fliter = true;
            }
            else if ((!string.IsNullOrEmpty(firstname) && !string.IsNullOrWhiteSpace(firstname)) &&
                     (string.IsNullOrEmpty(surname) || string.IsNullOrWhiteSpace(surname)))
            {
                queryParams += "contains(firstname, '" + firstname + "')";
                fliter = true;
            }
            if ((!string.IsNullOrEmpty(postcode) && !string.IsNullOrWhiteSpace(postcode)))
            {
                if (fliter)
                {
                    queryParams += "and contains(address1_postalcode, '" + postcode + "')";

                }
                else
                {
                    queryParams += "contains(address1_postalcode, '" + postcode + "')";
                }
                fliter = true;
            }
            if (address.Length > 0)
            {
                if (address.Length == 1 && fliter && !string.IsNullOrEmpty(address[0]))
                {
                    queryParams += "and contains(address1_name, '" + address + "')";
                }
                else if (address.Length == 1 && !fliter && !string.IsNullOrEmpty(address[0]))
                {
                    queryParams += "contains(address1_name, '" + address + "')";
                }
                else if (address.Length > 1 && !fliter)
                {
                    for (int i = 0; i < address.Length; i++)
                    {
                        if (i == 0)
                        {
                            if (!string.IsNullOrEmpty(address[i]))
                            {
                                queryParams += "contains(address1_name, '" + address[i] + "')";
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(address[i]))
                            {
                                queryParams += " and contains(address1_name, '" + address[i] + "')";
                            }
                        }
                    }

                }
                else if (address.Length > 1 && fliter)
                {
                    foreach (var addresssearch in address)
                    {
                        if (!string.IsNullOrEmpty(addresssearch))
                        {
                            queryParams += " and contains(address1_name, '" + addresssearch + "')";
                        }
                    }
                }

            }
            return "api/data/v8.2/contacts" + queryParams + "&$expand=parentcustomerid_account($select=accountid,housing_present,housing_accounttype),hackney_household_contactId($select=hackney_householdid),&$orderby=address1_name asc,fullname desc";
        }


        public static string GetAreaPatch(string postcode,string UPRN)
        {
            var query = new StringBuilder();

            var postCode = new
            {
                hackney_postcode = postcode
            };
            var uprnData = new
            {
                hackney_llpgref = UPRN
            };
            var fetchXml = $@"
               <fetch>
                  <entity name='hackney_propertyareapatch' >
                    <attribute name='hackney_llpgref' />
                    <attribute name='hackney_estateofficerpropertypatchid' />
                    <attribute name='hackney_propertyreference' />
                    <attribute name='hackney_areaname' />
                    <attribute name='hackney_ward' />
                    <attribute name='hackney_postcode' />
                    <attribute name='hackney_managerpropertypatchid' />
                    <filter type='and' >
                      <condition attribute='hackney_postcode' operator='eq' value='{postCode.hackney_postcode}' />
                      <condition attribute='hackney_llpgref' operator='eq' value='{uprnData.hackney_llpgref}' />
                    </filter>
                    <link-entity name='hackney_estateofficerpatch' from='hackney_estateofficerpatchid' to='hackney_estateofficerpropertypatchid' link-type='outer' >
                      <link-entity name='hackney_estateofficer' from='hackney_estateofficerid' to='hackney_patchid' link-type='outer' >
                        <attribute name='hackney_estateofficerid' alias='estateOfficerId' />
                        <attribute name='hackney_name' alias='OfficerFullName' />
                        <attribute name='hackney_lastname' alias='OfficerLastName' />
                        <attribute name='hackney_firstname' alias='OfficerFirstName' />
                      </link-entity>
                    </link-entity>
                    <link-entity name='hackney_estatemanagerarea' from='hackney_estatemanagerareaid' to='hackney_managerpropertypatchid' link-type='outer' >
                      <link-entity name='hackney_estateofficer' from='hackney_estateofficerid' to='hackney_managerareaid' link-type='outer' >
                        <attribute name='hackney_name' alias='ManagerFullName' />
                        <attribute name='hackney_lastname' alias='ManagerLastName' />
                        <attribute name='hackney_firstname' alias='ManagerFirstName' />
                      </link-entity>
                    </link-entity>
                  </entity>
                </fetch>";

            query.Append("/api/data/v8.2/hackney_propertyareapatchs?fetchXml=" + fetchXml);

            return query.ToString();
        }

        public static string getAccountsAndNotifications(string type)
        {
            StringBuilder query = new StringBuilder();
            query.Append("/api/data/v8.2/accounts?fetchXml=");
            query.Append(
                "<fetch><entity name='account' ><attribute name='housing_accounttype' /><attribute name='housing_cur_bal' />");
            if (type == "1")
            {
                query.Append(
                    "<filter type='and' ><condition attribute='housing_accounttype' operator='neq' value='2' /></filter>");
            }
            else if (type == "2")
            {
                query.Append(
                    "<filter type='and' ><condition attribute='housing_accounttype' operator='eq' value='2' /></filter>");
            }
            query.Append(
                "<link-entity name='housing_rentaccountnotification' from='housing_rentaccountnotification' to='accountid' link-type='inner' >");
            query.Append("<attribute name='housing_phone' /><attribute name='housing_isnotification' /></link-entity>");
            query.Append(
                "<link-entity name='housing_paymentagreement' from='housing_account_paymentagreemenid' to='accountid' link-type='outer' >");
            query.Append(
                "<attribute name='housing_aragdet_enddate' /><attribute name='housing_paymentagreementid' /></link-entity></entity></fetch>");

            return query.ToString();
        }

        public static string getAllOfficersPerArea(string area)
        {

            var query = new StringBuilder();

            var fetchData = new
            {
                hackney_areaname = area
            };

            var fetchXml = $@"<fetch version = '1.0' output-format = 'xml-platform' mapping = 'logical' distinct = 'true' >
              <entity name='hackney_propertyareapatch' >
               <attribute name='hackney_estateofficerpropertypatchid' />
               <attribute name='hackney_estateofficerpropertypatchidname' />
               <attribute name='hackney_managerpropertypatchid' />
               <attribute name='hackney_managerpropertypatchidname' />
               <order attribute='hackney_estateofficerpropertypatchidname'  />
               <filter type='and' >
               <condition attribute='hackney_areaname' operator='eq' value='{fetchData.hackney_areaname}' />
               </filter>
               <link-entity name='hackney_estateofficerpatch' from='hackney_estateofficerpatchid' to='hackney_estateofficerpropertypatchid' link-type='inner' >
                <filter>
                <condition attribute = 'statecode' operator= 'eq' value = '0'/>
                </filter>
                <attribute name='hackney_name' />
                <attribute name='hackney_patchidname' />
                <attribute name='hackney_patchid' />
                <attribute name='hackney_estateofficerpatchid' />
               </link-entity>
               <link-entity name='hackney_estatemanagerarea' from='hackney_estatemanagerareaid' to='hackney_managerpropertypatchid' link-type='inner' >
               <filter>
               <condition attribute = 'statecode' operator= 'eq' value = '0'/>
               </filter>
               <attribute name='hackney_name' />
               <attribute name='hackney_estatemanagerareaid' />
               <attribute name='hackney_managerareaidname' />
               <attribute name='hackney_managerareaid' />
               </link-entity>
               </entity>
               </fetch>";

            query.Append("/api/data/v8.2/hackney_propertyareapatchs?fetchXml=" + fetchXml);

            return query.ToString();
           
        }

        public static string updateIncidentQuery(string incidentId)
        {
            return "api/data/v8.2/incidents(" + new Guid(incidentId.Trim()) + ")";
        }

        public static string deleteAssociationOfManagerWithInteraction(string interactionId, string managerId, string organizationURL)
        {
            return
                "api/data/v8.0/hackney_tenancymanagementinteractionses(" + interactionId + ")/hackney_managerpropertypatchid/$ref?$id=" + organizationURL+"api/data/v8.2/hackney_estatemanagerareas(" +
                managerId + ")";
        }

        public static string deleteAssociationOfPatchWithInteraction(string interactionId, string patchId, string organizationURL)
        {
            return
                "api/data/v8.0/hackney_tenancymanagementinteractionses(" + interactionId + ")/hackney_estateofficerpatchid/$ref?$id=" + organizationURL + "api/data/v8.2/hackney_estateofficerpatchs(" +
                patchId + ")";
        }

        public static string getContactCautionaryAlert(string uprn)
        {
            return
                "api/data/v8.2/hackney_cautionaryalerts?$select=_hackney_contactid_value,hackney_cautionaryalerttype,createdon,hackney_uprn&$filter=hackney_uprn eq '" +
                uprn + "'";
        }

        public static string postCautionaryAlert()
        {
            return
                "/api/data/v8.2/hackney_cautionaryalerts?$select=hackney_cautionaryalerttype,_hackney_contactid_value, hackney_uprn,hackney_cautionaryalertid";
        }

        public static string getAllContactsByUprn(string uprn)
        {
            return "/api/data/v8.2/contacts?$select=contactid,hackney_cautionaryalert,hackney_propertycautionaryalert&$filter=hackney_uprn eq '"+ uprn + "'";
        }

        public static string deleteCautionaryAlert(string cautionaryAlertId)
        {
            return "/api/data/v8.2/hackney_cautionaryalerts(" + cautionaryAlertId + ")";
        }

        public static string PostEstateOfficerSelectQuery()
        {
            return
                "/api/data/v8.2/hackney_estateofficers?$select=hackney_estateofficerid,hackney_name,hackney_firstname,hackney_lastname,hackney_emailaddress,statecode,statuscode";
        }

        public static string UpdateEstateOfficerSelect(string officerId)
        {
            return
                "/api/data/v8.2/hackney_estateofficers(" + officerId + ")?$select=hackney_estateofficerid,hackney_name,hackney_firstname,hackney_lastname,statecode,statuscode";
        }

        public static string PostEstateOfficerLoginSelectQuery()
        {
            return
                "/api/data/v8.2/hackney_estateofficerlogins?$select=hackney_estateofficerloginid,hackney_username,statecode,statuscode";
        }

        public static string UpdateEstateOfficerLoginQuery(string officerId)
        {
            return
                "/api/data/v8.2/hackney_estateofficerlogins(" + officerId + ")?$select=hackney_estateofficerloginid,hackney_name,hackney_username,statecode,statuscode";
        }

        public static string GetEstateOfficerLoginDetails(string officerId)
        {
            var query = new StringBuilder();

            var fetchXml = $@"<fetch version='1.0' output-format='xml - platform' mapping='logical' distinct='true'>
                <entity name = 'hackney_estateofficer' > 
                    <attribute name = 'hackney_estateofficerid' />|
                    <attribute name = 'hackney_name' />
                    <attribute name = 'hackney_firstname />
                    <filter type = 'and >                   
                           <condition attribute = 'hackney_estateofficerid' operator= 'eq' value = '{officerId}'/>
                        </filter >
                    <link-entity name = 'hackney_estateofficerlogin' from = 'hackney_officerloginid' to = 'hackney_estateofficerid' alias = 'ae' link-type='inner' >  
                        <attribute name='hackney_username' />
                        <attribute name='hackney_officerloginid' />
                        <attribute name='hackney_password' />
                        <attribute name='hackney_estateofficerloginid' /> 
                        <attribute name='statecode' /> 
                        <attribute name='statuscode' /> 
                     </link-entity >
                  </ entity >
               </ fetch >";

            query.Append("/api/data/v8.2/hackney_estateofficerlogins?fetchXml=" + fetchXml);
            return query.ToString();
        }
        public static string updateOfficerAssociatedWithPatch(string patchId)
        {
            return "/api/data/v8.2/hackney_estateofficerpatchs(" + patchId + ")?$select=hackney_patchid, hackney_name";
        }
        public static string updateOfficerAssociatedWithAreaAsManager(string managerAreaId)
        {
            return "/api/data/v8.2/hackney_estatemanagerareas(" + managerAreaId + ")?$select=hackney_estatemanagerareaid,_hackney_managerareaid_value,hackney_name";
        }
        public static string deleteAssociationOfOfficerWithPatch(string patchId, string officerId, string organizationURL)
        {
            return
                "api/data/v8.2/hackney_estateofficerpatchs(" + patchId + ")/hackney_patchid/$ref?$id=" + organizationURL + "api/data/v8.2/hackney_estateofficers(" +
                officerId + ")";
        }
        public static string deleteAssociationOfOfficerWithArea(string areaId, string officerId, string organizationURL)
        {
            return
                "api/data/v8.2/hackney_estatemanagerareas(" + areaId + ")/hackney_managerareaid/$ref?$id=" + organizationURL + "api/data/v8.2/hackney_estateofficers(" +
                officerId + ")";
        }


        public static string getAllOfficersThatAreNotAssignedToPatchOrArea()
        {
            var query = new StringBuilder();

            var fetchXml = $@"<fetch >
              <entity name='hackney_estateofficer' >
                <attribute name='hackney_name' />
                <attribute name='hackney_lastname' />
                <attribute name='hackney_estateofficerid' />
                <attribute name='hackney_firstname' />
                <filter type='and' >
                  <condition entityname='hackney_estateofficerpatch' attribute='hackney_patchid' operator='null' />
                  <condition entityname='hackney_estatemanagerarea' attribute='hackney_managerareaid' operator='null' />
                  <condition attribute='statuscode' operator='eq' value='1' />
                </filter>
                <link-entity name='hackney_estatemanagerarea' from='hackney_managerareaid' to='hackney_estateofficerid' link-type='outer' >
                  <attribute name='hackney_name' />
                  <attribute name='hackney_estatemanagerareaid' />
                  <attribute name='hackney_updatedbyname' />
                  <attribute name='hackney_managerareaid' />
                </link-entity>
                <link-entity name='hackney_estateofficerpatch' from='hackney_patchid' to='hackney_estateofficerid' link-type='outer' >
                  <attribute name='hackney_name' />
                  <attribute name='hackney_patchid' />
                  <attribute name='hackney_estateofficerpatchid' />
                  <attribute name='hackney_patchname' />
                </link-entity>
              </entity>
            </fetch>";

            query.Append("/api/data/v8.2/hackney_estateofficers?fetchXml=" + fetchXml);

            return query.ToString();
        }

        public static string GetOfficerAccountDetails(string userName)
        {
            var query = new StringBuilder();

            var fetchXml = $@"<fetch version='1.0' output-format='xml - platform' mapping='logical' distinct='true'>
                <entity name = 'hackney_estateofficer' > 
                    <attribute name = 'hackney_estateofficerid' />|
                    <attribute name = 'hackney_name' />
                    <attribute name = 'hackney_firstname />                   
                    <link-entity name = 'hackney_estateofficerlogin' from = 'hackney_officerloginid' to = 'hackney_estateofficerid' alias = 'ae' link-type='inner' >  
                        <attribute name='hackney_username' />
                        <attribute name='hackney_officerloginid' />
                        <attribute name='hackney_password' />
                        <attribute name='hackney_estateofficerloginid' /> 
                        <attribute name='statecode' /> 
                        <attribute name='statuscode' /> 
                         <filter type = 'and >                   
                           <condition attribute = 'hackney_username' operator= 'eq' value = '{userName}'/>
                        </filter >
                     </link-entity >
                  </ entity >
               </ fetch >";

            query.Append("/api/data/v8.2/hackney_estateofficerlogins?fetchXml=" + fetchXml);
            return query.ToString();
        }

        public static string GetOfficerLoginUserNameQuery(string userName)
        {
            return
                "/api/data/v8.2/hackney_estateofficerlogins?$select=hackney_estateofficerloginid,hackney_username,statecode,statuscode&$filter=hackney_username eq '" + userName + "'";
        }

        public static string GetContactDetailsByContactId(string contactId)
        {
            return
                "/api/data/v8.2/contacts("+contactId+ ")?$select=emailaddress1,hackney_uprn,address1_line1,address1_line2,address1_line3,lastname,firstname,hackney_larn,address1_addressid,telephone1,telephone2,telephone3,address3_addressid,hackney_cautionaryalert,address1_name,hackney_propertycautionaryalert,housing_responsible,contactid,address2_addressid,address1_postalcode,housing_house_ref,hackney_title,address1_composite,birthdate,hackney_hackneyhomesid,_hackney_household_contactid_value,hackney_membersid,hackney_personno,_parentcustomerid_value,hackney_nextofkinname,hackney_nextofkinaddress,hackney_nextofkinrelationship,hackney_nextofkinotherphone,hackney_nextofkinemail,hackney_nextofkinmobile";
        }

        public static string GetAllContactsandDetailsByUprn(string uprn)
        {
            return "/api/data/v8.2/contacts?$select=contactid,birthdate,lastname,hackney_uprn,firstname,address1_line1,address1_line2,address1_line3,hackney_larn,fullname,hackney_title,hackney_cautionaryalert,hackney_propertycautionaryalert,address1_postalcode,address1_name,telephone1,telephone2,telephone3,emailaddress1,housing_house_ref,address1_composite,hackney_hackneyhomesid,housing_house_ref,hackney_relationship,hackney_extendedrelationship,hackney_age,hackney_disabled,hackney_responsible &$filter=hackney_uprn eq '" + uprn.Trim() + "'";
        }

        public static string getAAccountDetailsByContactIdQuery(string contactid)
        {

            var query = new StringBuilder();

            var fetchData = new
            {
                contactid
            };
            var fetchXml = $@"
            <fetch>
              <entity name='account'>
                <attribute name='housing_present' />
                <attribute name='housing_cot' />
                <attribute name='housing_prop_ref' />
                <attribute name='housing_house_ref' />
                <attribute name='housing_terminatedname' />
                <attribute name='housing_tenure' />
                <attribute name='housing_rent' />
                <attribute name='housing_accounttype' />
                <attribute name='housing_presentname' />
                <attribute name='housing_directdebit' />
                <attribute name='housing_agr_type' />
                <attribute name='housing_propertycount' />
                <attribute name='housing_tag_ref' />
                <attribute name='housing_eot' />
                <attribute name='housing_accounttypename' />
                <attribute name='housing_cur_bal' />
                <attribute name='housing_anticipated' />
                <attribute name='housing_terminated' />
                <attribute name='housing_directdebit' />
                <attribute name='accountid' />
                <attribute name='housing_u_saff_rentacc' />
                <link-entity name='contact' from='parentcustomerid' to='accountid'>
                    <filter>
                    <condition attribute='contactid' operator='eq' value='{fetchData.contactid}'/>
                  </filter>
                </link-entity>
              </entity>
            </fetch>";


            query.Append("/api/data/v8.2/accounts?fetchXml=" + fetchXml);

            return query.ToString();

        }
        public static string updateNextOfKinDetails(string contactID)
        {
            return "/api/data/v8.2/contacts(" + contactID + ")?$select=hackney_nextofkinname, hackney_nextofkinaddress,hackney_nextofkinrelationship,hackney_nextofkinotherphone,hackney_nextofkinemail,hackney_nextofkinmobile";
        }

        public static string getETRAIssues(string id, bool issuesPerMeeting)
        {
            StringBuilder query = new StringBuilder();

            var fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' ><entity name='hackney_tenancymanagementinteractions' >              
                <attribute name='hackney_areaname' />
                <attribute name='hackney_name' />
                <attribute name='statecode' />
                <attribute name='hackney_estateofficerpatchid' />
                <attribute name='hackney_natureofenquiry' />
                <attribute name='createdon' />
                <attribute name='hackney_estateofficer_createdbyid' />
                <attribute name='hackney_estateofficer_updatedbyid' />
                <attribute name='hackney_tenancymanagementinteractionsid' />
                <attribute name='hackney_enquirysubject' />
                <attribute name='hackney_managerpropertypatchid' />
                <attribute name='hackney_incidentid' />
                <attribute name='hackney_transferred' />
                <attribute name='hackney_process_stage' />
                <attribute name='hackney_issuelocation' />
                <attribute name='hackney_traid' />
                <filter>";
            if (issuesPerMeeting)
            {
                fetchXml = fetchXml + "<condition attribute='hackney_parent_interactionid' operator='eq' value='" + id +
                           "' />";
            }
            else 
            {
                fetchXml = fetchXml + "<condition attribute='hackney_traid' operator='eq' value='" + id +
                             "' />";
            }
           fetchXml = fetchXml + $@"
                        </filter>
                    <link-entity name='incident' from='incidentid' to='hackney_incidentid' link-type='inner' >
                        <attribute name='housing_requestcallback' />
                        <attribute name='incidentid' />
                        <link-entity name='annotation' from='objectid' to='incidentid' link-type='outer' >
                            <attribute name='subject' />
                            <attribute name='createdby' />
                            <attribute name='notetext' />
                            <attribute name='createdon' />
                            <attribute name='annotationid' />
                        </link-entity>
                    </link-entity>                 
                    <link-entity name='hackney_estateofficerpatch' from='hackney_estateofficerpatchid' to='hackney_estateofficerpatchid' link-type='outer'>
                      <link-entity name='hackney_estateofficer' from='hackney_estateofficerid' to='hackney_patchid' link-type='outer'>
                        <attribute name='hackney_name' alias='OfficerFullName' />
                        <attribute name='hackney_lastname' alias='OfficerLastName' />
                        <attribute name='hackney_firstname' alias='OfficerFirstName' />
                      </link-entity>
                    </link-entity>
                    <link-entity name='hackney_estatemanagerarea' from='hackney_estatemanagerareaid' to='hackney_managerpropertypatchid' link-type='outer'>
                      <link-entity name='hackney_estateofficer' from='hackney_estateofficerid' to='hackney_managerareaid' link-type='outer'>
                        <attribute name='hackney_name' alias='ManagerFullName' />
                        <attribute name='hackney_lastname' alias='ManagerLastName' />
                        <attribute name='hackney_firstname' alias='ManagerFirstName' />
                      </link-entity>
                    </link-entity></entity></fetch>";
            query.Append("/api/data/v8.2/hackney_tenancymanagementinteractionses?fetchXml=" + HttpUtility.UrlEncode(fetchXml.Trim()));
            return query.ToString();
        }
    }
}

