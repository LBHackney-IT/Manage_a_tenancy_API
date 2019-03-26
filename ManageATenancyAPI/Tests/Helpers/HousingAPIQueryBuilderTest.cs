using ManageATenancyAPI.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ManageATenancyAPI.Formatters;
using ManageATenancyAPI.Helpers.Housing;
using Xunit;

namespace ManageATenancyAPI.Tests.Helpers
{
    public class HousingAPIQueryBuilderTest
    {
        [Fact]
        public async Task test_if_fetchXML_query_is_built_correctly()
        {
            string query = HousingAPIQueryBuilder.getAccountsByParisReferenceAndPostcodeQuery("12345678", "E8 2LN");

            StringBuilder testQuery = new StringBuilder();
            testQuery.Append("/api/data/v8.2/accounts?fetchXml=");
            testQuery.Append(
                "<fetch version = '1.0' output-format = 'xml-platform' mapping = 'logical' distinct = 'true' >");
            testQuery.Append("<entity name = 'account'>");
            testQuery.Append("<attribute name = 'housing_u_saff_rentacc'/>");
            testQuery.Append("<filter type = 'and' >");
            testQuery.Append("<condition attribute = 'housing_u_saff_rentacc' operator= 'eq' value = '12345678' />");
            testQuery.Append("</filter >");
            testQuery.Append("<link-entity name = 'customeraddress' from = 'parentid' to = 'accountid' alias = 'aj' >");
            testQuery.Append("<attribute name = 'housing_post_code' />");
            testQuery.Append("<attribute name = 'housing_short_address' /><attribute name = 'addresstypecode' />");
            testQuery.Append("<filter type = 'and' >");
            testQuery.Append("<condition attribute = 'housing_post_code' operator= 'eq' value = 'E8 2LN'/>");
            testQuery.Append("</filter>");
            testQuery.Append("</link-entity>");
            testQuery.Append("</entity>");
            testQuery.Append("</fetch>");


            Assert.Equal(query, testQuery.ToString());

        }


        [Fact]
        public async Task test_if_fetchXML_query_is_built_correctly_for_account_paymentagreement()
        {
            string query = HousingAPIQueryBuilder.GetHousingPaymentagreementQuery("12345678");

            StringBuilder testQuery = new StringBuilder();
            testQuery.Append(
                "/api/data/v8.2/housing_paymentagreements?$select=housing_aragdet_amount,housing_aragdet_frequency&$filter=housing_tag_ref eq" +
                " " + "'" + 12345678 + "'");


            Assert.Equal(query, testQuery.ToString());

        }


        [Fact]
        public async Task test_if_fetchXML_query_for_get_transactions_is_built_correctly()
        {
            string queryTest = HousingAPIQueryBuilder.getTransactionsByTagReference("000000/01");

            StringBuilder query = new StringBuilder();
            query.Append(
                "/api/data/v8.2/housing_rtranses?$select=housing_real_value,housing_tag_ref,housing_prop_ref,housing_house_ref," +
                "housing_prop_ref,housing_tag_ref,housing_trans_type&$orderby=housing_postdate desc,housing_real_value," +
                "housing_deb_desc&$filter=");
            query.Append("housing_tag_ref eq '000000/01'");
            query.Append("and Microsoft.Dynamics.CRM.Between(PropertyName='housing_postdate',PropertyValues=[");
            query.Append(
                "%22" + DateTimeFormatter.FormatDateTimeToUtc(new DateTime(DateTime.Today.AddYears(-1).Year, 4, 1)) +
                "%22" + "," +
                "%22" + DateTimeFormatter.FormatDateTimeToUtc(DateTime.Today.AddDays(1)) + "%22])");


            Assert.Equal(queryTest, query.ToString());
        }

        [Fact]
        public async Task test_if_query_for_create_account_is_built_correctly()
        {
            string queryTest = HousingAPIQueryBuilder.PostContactQuery();
            StringBuilder query = new StringBuilder();
            query.Append(
                "/api/data/v8.2/contacts?$select=contactid,address1_city,address1_country,address1_line1,address1_line2,address1_line3,address1_postalcode,birthdate,emailaddress1,firstname,fullname,_hackney_createdby_value,jobtitle,lastname,telephone1,telephone2,hackney_uprn,createdon,hackney_larn,hackney_hackneyhomesid");

            Assert.Equal(queryTest, query.ToString());

        }

        [Fact]
        public async Task test_if_query_for_update_contact_is_built_correctly()
        {
            string queryTest =
                HousingAPIQueryBuilder.updateContactQuery(
                    "10496cc2-97e5-e711-8110-70106faaf8c110496cc2-97e5-e711-8110-70105bbbbbbb");
            StringBuilder query = new StringBuilder();
            query.Append("api/data/v8.2/contacts(10496cc2-97e5-e711-8110-70106faaf8c110496cc2-97e5-e711-8110-70105bbbbbbb)?$select=firstname,lastname,fullname,birthdate,emailaddress1,address1_line3,address1_line1,address1_line2,address1_city,address1_postalcode,telephone1,telephone2,telephone3");
            Assert.Equal(queryTest, query.ToString());

        }

        [Fact]
        public async Task test_if_query_for_create_service_request_is_built_correctly()
        {
            string queryTest = HousingAPIQueryBuilder.PostIncidentQuery();
            StringBuilder query = new StringBuilder();
            query.Append(
                "/api/data/v8.2/incidents()?$select=_customerid_value,description,_subjectid_value,ticketnumber,title");

            Assert.Equal(queryTest, query.ToString());

        }

        [Fact]
        public async Task test_if_query_for_update_tmi_is_built_correctly()
        {
            string queryTest = HousingAPIQueryBuilder.PostTenancyManagementInteractionQuery();
            StringBuilder query = new StringBuilder();
            query.Append(
                "/api/data/v8.2/hackney_tenancymanagementinteractionses?$select=_hackney_contactid_value,hackney_enquirysubject,_hackney_estateofficer_createdbyid_value,hackney_handleby,_hackney_incidentid_value,hackney_name,hackney_natureofenquiry,_hackney_subjectid_value,hackney_tenancymanagementinteractionsid");

            Assert.Equal(queryTest, query.ToString());

        }

        [Fact]
        public async Task test_if_query_for_get_group_tray_is_built_correctly()
        {
            string queryTest = HousingAPIQueryBuilder.getAreaTrayInteractions("1");
            var fetchXml = $@"/api/data/v8.2/hackney_tenancymanagementinteractionses?fetchXml=
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
                  <condition attribute='hackney_areaname' operator='eq' value='1'/>
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
            Assert.Equal(queryTest, fetchXml);

        }

        [Fact]
        public async Task test_if_query_for_create_notification_is_built_correctly()
        {
            string queryTest = HousingAPIQueryBuilder.PostNotificationQuery();
            StringBuilder query = new StringBuilder();
            query.Append(
                "/api/data/v8.2/housing_rentaccountnotifications?$select=housing_tag_ref,housing_isnotification,housing_phone,housing_email,housing_accounttype,housing_rentaccountnotificationid");

            Assert.Equal(queryTest, query.ToString());

        }

        [Fact]
        public async Task test_if_query_for_update_notification_is_built_correctly()
        {
            var notificationId = "38017ccd-73fb-e711-8114-7010000000";
            string queryTest = HousingAPIQueryBuilder.UpdateNotificationQuery(notificationId);
            StringBuilder query = new StringBuilder();
            query.Append(
                "/api/data/v8.2/housing_rentaccountnotifications(38017ccd-73fb-e711-8114-7010000000)?$select=housing_tag_ref,housing_isnotification,housing_phone,housing_email,housing_accounttype");

            Assert.Equal(queryTest, query.ToString());

        }

        [Fact]
        public async Task test_if_query_for_authenticate_officers_is_built_correctly()
        {
            string queryTest = HousingAPIQueryBuilder.getAuthenticatedUserQuery("", "");
            StringBuilder query = new StringBuilder();
            var fetchData = new
            {
                hackney_password = "",
                hackney_username = ""
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
            query.Append("/api/data/v8.2/hackney_estateofficerlogins?fetchXml=" + fetchXml);
            Assert.Equal(queryTest, query.ToString());

        }

        [Fact]
        public async Task test_if_query_for_get_interactions_is_built_correctly()
        {
            string queryTest =
                HousingAPIQueryBuilder.getTenancyInteractionDeatils("463adffe-61a5-db11-772c-0014c260c5faeeeee", "contact");

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
                <filter><condition attribute='hackney_contactid' operator='eq' value='463adffe-61a5-db11-772c-0014c260c5faeeeee' />
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
                    <link-entity name='contact' from='contactid' to='hackney_contactid' >
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
            Assert.Equal(queryTest, query.ToString());
        }

        [Fact]
        public async Task test_if_query_for_get_areapatch_is_built_correctly()
        {
            var postcode = "N16 5DH";
            var UPRN = "123456789";
            string queryTest = HousingAPIQueryBuilder.GetAreaPatch(postcode,UPRN);
            var query = new StringBuilder();

            var postCode = new
            {
                hackney_postcode = postcode
            };
            var llpgRef = new
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
                      <condition attribute='hackney_llpgref' operator='eq' value='{llpgRef.hackney_llpgref}' />
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

            Assert.Equal(query.ToString(), queryTest);

        }

        [Fact]
        public async Task test_if_query_for_Get_CRMCitizenSearch_is_built_correctly()
        {
            var searchquery = "89-90 TEST HOUSE MARE STREET HACKNEY LONDON HACKNEY E8 2LN".Split(' ').Select(searchpattern => searchpattern.Trim()).ToArray();            
            string queryTest = HousingAPIQueryBuilder.getCRMCitizenSearch(string.Empty, string.Empty, searchquery , null);
            StringBuilder query = new StringBuilder();
            query.Append("api/data/v8.2/contacts?$select=contactid,firstname,lastname,fullname,jobtitle,birthdate,emailaddress1,telephone1,telephone2,telephone3,hackney_larn,hackney_uprn,address1_name,address1_line1,address1_line2,address1_line3, address1_city,address1_postalcode,hackney_cautionaryalert,hackney_propertycautionaryalert,hackney_responsible,hackney_hackneyhomesid&$filter=contains(address1_name, '" + searchquery[0] + "') and contains(address1_name, '" + searchquery[1] + "') and contains(address1_name, '" + searchquery[2] + "') and contains(address1_name, '" + searchquery[3] + "') and contains(address1_name, '" + searchquery[4] + "') and contains(address1_name, '" + searchquery[5] + "') and contains(address1_name, '" + searchquery[6] + "') and contains(address1_name, '" + searchquery[7] + "') and contains(address1_name, '" + searchquery[8] + "') and contains(address1_name, '" + searchquery[9] + "')");
            query.Append(
                "&$expand=parentcustomerid_account($select=accountid,housing_present,housing_accounttype),hackney_household_contactId($select=hackney_householdid),&$orderby=address1_name asc,fullname desc");
            Assert.Equal(query.ToString(), queryTest);

        }

        
        [Fact]
        public async Task test_if_query_for_get_accounts_and_notifications_is_built_correctly()
        {
            string queryTest =
                HousingAPIQueryBuilder.getAccountsAndNotifications("1");

            StringBuilder query = new StringBuilder();
            query.Append("/api/data/v8.2/accounts?fetchXml=");
            query.Append(
                "<fetch><entity name='account' ><attribute name='housing_accounttype' /><attribute name='housing_cur_bal' />");
                query.Append(
                    "<filter type='and' ><condition attribute='housing_accounttype' operator='neq' value='2' /></filter>");
            query.Append(
                "<link-entity name='housing_rentaccountnotification' from='housing_rentaccountnotification' to='accountid' link-type='inner' >");
            query.Append("<attribute name='housing_phone' /><attribute name='housing_isnotification' /></link-entity>");
            query.Append(
                "<link-entity name='housing_paymentagreement' from='housing_account_paymentagreemenid' to='accountid' link-type='outer' >");
            query.Append(
                "<attribute name='housing_aragdet_enddate' /><attribute name='housing_paymentagreementid' /></link-entity></entity></fetch>");


            Assert.Equal(queryTest, query.ToString());
        }

        [Fact]
        public async Task test_if_query_for_get_account_details_by_payment_reference_is_built_correcty()
        {
            var reference = "111111111";
            string queryTest =
                HousingAPIQueryBuilder.getAccountDetailsByTagorParisReferenceQuery(reference);
            StringBuilder query = new StringBuilder();
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

            Assert.Equal(queryTest, query.ToString());
        }

        [Fact]
        public async Task test_if_query_for_get_account_details_by_tag_reference_is_built_correcty()
        {
            var reference = "12345/01";
            string queryTest =
                HousingAPIQueryBuilder.getAccountDetailsByTagorParisReferenceQuery(reference);
            StringBuilder query = new StringBuilder();
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

            Assert.Equal(queryTest, query.ToString());
            Assert.Equal(queryTest, query.ToString());
        }



        [Fact]
        public async Task test_if_query_for_update_incident_is_built_correctly()
        {
            var incidentId = "2a7912b3-b6e0-e711-810e-70106bbbbbbb";
            string queryTest = HousingAPIQueryBuilder.updateIncidentQuery(incidentId);

            string queryBuilt = "api/data/v8.2/incidents(2a7912b3-b6e0-e711-810e-70106bbbbbbb)";

            Assert.Equal(queryTest, queryBuilt);

        }

        [Fact]
        public async Task test_if_query_for_remove_association_of_manager_with_interaction_is_built_correctly()
        {
            var incidentId = "2a7912b3-b6e0-e711-810e-70106bbbbbbb";
            var organizationURL = "testUrl";
            var managerId = "ba7922b3-b6e0-e711-110e-70106faaf8c2";
            string queryTest = HousingAPIQueryBuilder.deleteAssociationOfManagerWithInteraction(incidentId, managerId, organizationURL);

            string queryBuilt = "api/data/v8.0/hackney_tenancymanagementinteractionses(2a7912b3-b6e0-e711-810e-70106bbbbbbb)/hackney_managerpropertypatchid/$ref?$id=testUrlapi/data/v8.2/hackney_estatemanagerareas(ba7922b3-b6e0-e711-110e-70106faaf8c2)";

            Assert.Equal(queryTest, queryBuilt);

        }

        [Fact]
        public async Task test_if_query_for_remove_association_of_patch_with_interaction_is_built_correctly()
        {
            var incidentId = "2a7912b3-b6e0-e711-810e-70106bbbbbbb";
            var organizationURL = "testUrl";
            var patchId = "ba7922b3-b6e0-e711-110e-70106faaf8c2";
            string queryTest = HousingAPIQueryBuilder.deleteAssociationOfPatchWithInteraction(incidentId, patchId, organizationURL);

            string queryBuilt = "api/data/v8.0/hackney_tenancymanagementinteractionses(2a7912b3-b6e0-e711-810e-70106bbbbbbb)/hackney_estateofficerpatchid/$ref?$id=testUrlapi/data/v8.2/hackney_estateofficerpatchs(ba7922b3-b6e0-e711-110e-70106faaf8c2)";

            Assert.Equal(queryTest, queryBuilt);

        }


        [Fact]
        public async Task test_if_query_for_get_cautionary_contact_alert_is_built_correctly()
        {
            var uprn = "1234567";
            string queryTest = HousingAPIQueryBuilder.getContactCautionaryAlert(uprn);

            string queryBuilt =
                "api/data/v8.2/hackney_cautionaryalerts?$select=_hackney_contactid_value,hackney_cautionaryalerttype,createdon,hackney_uprn&$filter=hackney_uprn eq '1234567'";

            Assert.Equal(queryTest, queryBuilt);

        }


        [Fact]
        public async Task test_if_query_for_create_cautionary_contact_alert_is_built_correctly()
        {
           string queryTest = HousingAPIQueryBuilder.postCautionaryAlert();

            string queryBuilt =
                "/api/data/v8.2/hackney_cautionaryalerts?$select=hackney_cautionaryalerttype,_hackney_contactid_value, hackney_uprn,hackney_cautionaryalertid";

            Assert.Equal(queryTest, queryBuilt);
        }

        [Fact]
        public async Task test_if_query_for_remove_cautionary_contact_alert_is_built_correctly()
        {
            string cautionaryAlertId = "123456789";
            string queryTest = HousingAPIQueryBuilder.deleteCautionaryAlert(cautionaryAlertId);

            string queryBuilt =
                "/api/data/v8.2/hackney_cautionaryalerts(123456789)";

            Assert.Equal(queryTest, queryBuilt);
        }

        [Fact]
        public async Task test_if_query_for_get_all_contacts_by_urpn_is_built_correctly()
        {
            string uprn = "123456789";
            string queryTest = HousingAPIQueryBuilder.getAllContactsByUprn(uprn);

            string queryBuilt =
                "/api/data/v8.2/contacts?$select=contactid,hackney_cautionaryalert,hackney_propertycautionaryalert&$filter=hackney_uprn eq '123456789'";

            Assert.Equal(queryTest, queryBuilt);
        }

        [Fact]
        public async Task test_if_query_for_update_patch_is_built_correctly()
        {
            string patchId = "123456789";
            string queryTest = HousingAPIQueryBuilder.updateOfficerAssociatedWithPatch(patchId);

            string queryBuilt =
                "/api/data/v8.2/hackney_estateofficerpatchs(123456789)?$select=hackney_patchid, hackney_name";

            Assert.Equal(queryTest, queryBuilt);
        }

        [Fact]
        public async Task test_if_query_for_update_area_manager_is_built_correctly()
        {
            string areamanagerId = "123456789";
            string queryTest = HousingAPIQueryBuilder.updateOfficerAssociatedWithAreaAsManager(areamanagerId);

            string queryBuilt =
                "/api/data/v8.2/hackney_estatemanagerareas(123456789)?$select=hackney_estatemanagerareaid,_hackney_managerareaid_value,hackney_name";

            Assert.Equal(queryTest, queryBuilt);
        }

        [Fact]
        public async Task test_if_query_for_remove_association_of_patch_with_officer_is_built_correctly()
        {
            var patchId = "2a7912b3-b6e0-e711-810e-11223344555";
            var organizationURL = "testUrl";
            var officerid = "ba7922b3-b6e0-e711-110e-11223344555";
            string queryTest = HousingAPIQueryBuilder.deleteAssociationOfOfficerWithPatch(patchId, officerid, organizationURL);

            string queryBuilt = "api/data/v8.2/hackney_estateofficerpatchs(2a7912b3-b6e0-e711-810e-11223344555)/hackney_patchid/$ref?$id=testUrlapi/data/v8.2/hackney_estateofficers(ba7922b3-b6e0-e711-110e-11223344555)";

            Assert.Equal(queryTest, queryBuilt);

        }
        [Fact]
        public async Task test_if_query_for_remove_association_of_area_with_officer_is_built_correctly()
        {
            var areamanagerId = "2a7912b3-b6e0-e711-810e-11223344555";
            var organizationURL = "testUrl";
            var officerid = "ba7922b3-b6e0-e711-110e-11223344555";
            string queryTest = HousingAPIQueryBuilder.deleteAssociationOfOfficerWithArea(areamanagerId, officerid, organizationURL);

            string queryBuilt = "api/data/v8.2/hackney_estatemanagerareas(2a7912b3-b6e0-e711-810e-11223344555)/hackney_managerareaid/$ref?$id=testUrlapi/data/v8.2/hackney_estateofficers(ba7922b3-b6e0-e711-110e-11223344555)";

            Assert.Equal(queryTest, queryBuilt);

        }



        [Fact]
        public async Task test_if_query_for_get_all_unassigned_officers_is_built_correctly()
        {
            string queryTest =
                HousingAPIQueryBuilder.getAllOfficersThatAreNotAssignedToPatchOrArea();

            StringBuilder query = new StringBuilder();
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


            Assert.Equal(queryTest, query.ToString());
        }
    }
}
