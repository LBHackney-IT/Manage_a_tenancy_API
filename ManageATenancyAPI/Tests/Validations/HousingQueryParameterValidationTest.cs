using ManageATenancyAPI.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing;
using ManageATenancyAPI.Models.Housing.NHO;
using Xunit;

namespace ManageATenancyAPI.Tests.Validations
{
    public class HousingQueryParameterValidationTest
    {
        [Theory]
        [InlineData("913316106", "NR7 2RE", false)]
        [InlineData("228316106", "NR7 2RE", true)]
        [InlineData("299316106", "NR7 2RE", true)]
        [InlineData("", "NR7 2RE", false)]
        [InlineData("299316106", "NR7", false)]
        [InlineData("", "", false)]
        [InlineData("299316106", "NR7 1yuytg", false)]
        public void return_a_boolean_if_Lease_Account_Property_Reference_is_valid(string propertyReferenceNumber, string postcode, bool expected)
        {
            var propertyReference = new HousingQueryParameterValidator();
            var result = propertyReference.ValidatePropertyReferenceAndPostcode(propertyReferenceNumber, postcode);
            Assert.Equal(expected, result.Valid);
        }

        [Theory]
        [InlineData("00001/01", true)]
        [InlineData("000015789", false)]
        [InlineData("000016/02", true)]
        public void return_a_boolean_if_Transactions_Tag_Reference_is_valid(string tagReferenceNumber, bool expected)
        {
            var tagReference = new HousingQueryParameterValidator();
            var result = tagReference.ValidateTagReference(tagReferenceNumber);
            Assert.Equal(expected, result.Valid);
        }

        [Fact]
        public void return_true_if_contact_object_passed_when_creating_contact_is_valid()

        {
           var request = new Contact
            {
                LastName = "test last name",
                FirstName = "test first name",
                Email = "test email",
                Address1 = "maurice bishop house",
                City = "london",
                Telephone1 = "0987654321",
                HousingId = "12452",
                CreatedByOfficer = "de98e4b6-15dc-e711-8115-11111111",
                PostCode = "e81hh"
            };

            var validator = new HousingQueryParameterValidator();
            var result = validator.validateCreateContactRequest(request);

            Assert.Equal(true, result.Valid);
        }


        [Fact]
        public void return_false_if_contact_object_passed_when_creating_contact_is_valid()

        {
            var request = new Contact();
         
            var validator = new HousingQueryParameterValidator();
            var result = validator.validateCreateContactRequest(request);

            Assert.Equal(false, result.Valid);
        }

        [Fact]
        public void return_true_if_contact_object_passed_when_updating_contact_is_valid()

        {
            var request = new Contact
            {
                LastName = "test last name",
                FirstName = "test first name",
                Email = "test email",
                UpdatedByOfficer = "e64fee7c-2bba-e711-8106-1111111111"
            };
            var id = "10496cc2-97e5-e711-8110-70106faaf8c110496cc2-97e5-e711-8110-70105bbbbbbb";
            var validator = new HousingQueryParameterValidator();
            var result = validator.validateUpdateAccountRequest(id,request);

            Assert.Equal(true, result.Valid);
        }
        [Fact]
        public void return_false_if_contact_object_passed_when_updating_contact_is_valid()

        {
            var request = new Contact();
            var id = "10496cc2-97e5-e711-8110-70106faaf8c110496cc2-97e5-e711-8110-70105bbbbbbb";
            var validator = new HousingQueryParameterValidator();
            var result = validator.validateUpdateAccountRequest(id, request);

            Assert.Equal(false, result.Valid);
        }

        [Fact]
        public void return_true_if_service_request_is_valid()

        {
            var request = new CRMServiceRequest
            {
                Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                ContactId = "463adffe-61a5-db11-882c-000000000000",
                Title = "Tenancy Management",
                Description = "Enquiry Created By Estate Officer",
                RequestCallback = true

            };
            
            var validator = new HousingQueryParameterValidator();
            var result = validator.ValidateServicerequest(request);

            Assert.Equal(true, result.Valid);
        }
        [Fact]
        public void return_false_if_service_request_is_valid()

        {
            var request = new CRMServiceRequest();
            var validator = new HousingQueryParameterValidator();
            var result = validator.ValidateServicerequest(request);

            Assert.Equal(false, result.Valid);
        }
        [Fact]
        public void return_true_if_tenancy_Management_interaction_object_passed_when_creating_tmi_is_valid()
        {
            var request = new TenancyManagement
            {
                contactId = "463adffe-61a5-db11-882c-000000000000",
                enquirySubject = "100000005",
                estateOfficerId = "284216e9-d365-e711-80f9-70106aaaaaaa",
                subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                adviceGiven = "Advice Housing Repair Inquiry",
                estateOffice = "5",
                source = "1",
                natureofEnquiry = "3",
                managerId= "c1f72d01-28dc-e711-1678-70106faa6a11",
                areaName="2",
                officerPatchId= "c1f72d01-2345-e711-8115-70106faa6a16",
                processType= "0",
                ServiceRequest = new CRMServiceRequest
                {
                    Subject = "c1f72d01-28dc-e711-8115-70106aaaaaaa",
                    ContactId = "463adffe-61a5-db11-882c-000000000000",
                    Title = "Tenancy Management",
                    Description = "Enquiry Created By Estate Officer"
                }

            };

            var validator = new HousingQueryParameterValidator();
            var result = validator.ValidateTenancyManagementinteractionrequest(request);

            Assert.True(result.Valid);
        }


        [Fact]
        public void return_false_if_tenancy_Management_interaction_object_passed_when_creating_tmi_is_valid()

        {
            var request = new TenancyManagement();

            var validator = new HousingQueryParameterValidator();
            var result = validator.ValidateTenancyManagementinteractionrequest(request);

            Assert.Equal(false, result.Valid);
        }

        [Fact]
        public void return_true_if_get_group_tray__request_is_valid()

        {
            var officeId = "1";

            var validator = new HousingQueryParameterValidator();
            var result = validator.ValidateGetAreaTrayIneractions(officeId);

            Assert.Equal(true, result.Valid);
        }
        [Fact]
        public void return_false_if_get_group_tray__request_is_valid()

        {
            var officeId =string.Empty;
            var validator = new HousingQueryParameterValidator();
            var result = validator.ValidateGetAreaTrayIneractions(officeId);

            Assert.Equal(false, result.Valid);
        }

      

        [Fact]
        public void return_true_if_get_interactions__request_is_valid()

        {
            var contactId = "463adffe-61a5-db11-882c-000000000000";
            var type = "contact";

            var validator = new HousingQueryParameterValidator();
            var result = validator.ValidateGetTenancyManagementInteraction(contactId, type);

            Assert.Equal(true, result.Valid);
        }
        [Fact]
        public void return_false_if_get_interactions__request_is_invalid()

        {
            var contactId = "463adffe-61a5-db11-882c-000000000000";
            var type = "";

            var validator = new HousingQueryParameterValidator();
            var result = validator.ValidateGetTenancyManagementInteraction(contactId, type);


            Assert.Equal(false, result.Valid);
        }

        [Fact]
        public void return_false_if_update_patch_or_area_is_invalid()
        {
            var request = new OfficerAreaPatch
            {
                officerId = "be77dd44-b005-e811-811c-7111111",
                updatedByOfficer = "be77dd44-b005-e811-811c-22222",
               
            };
            var validator = new HousingQueryParameterValidator();
            var result = validator.ValidateUpdateOfficerPatchOrAreaManager(request);


            Assert.Equal(false, result.Valid);
        }
        [Fact]
        public void return_false_if_update_patch_or_area_is_valid()
        {
            var request = new OfficerAreaPatch
            {
                officerId = "be77dd44-b005-e811-811c-7111111",
                patchId = "be77dd44-b005-e811-811c-0000000",
                updatedByOfficer = "be77dd44-b005-e811-811c-22222",
                isUpdatingPatch = true
            };
            var validator = new HousingQueryParameterValidator();
            var result = validator.ValidateUpdateOfficerPatchOrAreaManager(request);


            Assert.Equal(true, result.Valid);
        }

        [Fact]
        [Trait("OfficerAccountActionsTest", "Validation Test")]
        public void return_false_if_estateofficeraccount_child_object_is_not_valid()
        {
            var request = new EstateOfficerAccount();

            var validator = new HousingQueryParameterValidator();

            var result = validator.ValidateEstateOfficerAccount(request);

            Assert.Equal(false, result.Valid);
        }

        [Fact]
        [Trait("OfficerAccountActionsTest", "Validation Test")]
        public void return_false_if_estateofficeraccount_properties_object_is_not_valid()
        {
            var request = new EstateOfficerAccount()
            {
                OfficerAccount = new OfficerAccount() { HackneyFirstname = "Test First", HackneyLastname = "", HackneyEmailaddress = "flast@test.com" },
                OfficerLoginAccount = new OfficerLoginAccount() { HackneyUsername = "Test user Name", HackneyPassword = "HackneyTestPassword" }
            };


            var validator = new HousingQueryParameterValidator();

            var result = validator.ValidateEstateOfficerAccount(request);

            Assert.Equal(false, result.Valid);
        }


        [Fact]
        [Trait("OfficerAccountActionsTest", "Validation Test")]
        public void return_false_if_estateofficeraccount_id_are_empty_or_null()
        {            
            var validator = new HousingQueryParameterValidator();

            var result = validator.ValidateOfficerAccountIds(null, "");

            Assert.Equal(false, result.Valid);
        }
    }
}
