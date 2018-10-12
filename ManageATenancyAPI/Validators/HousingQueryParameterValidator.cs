using ManageATenancyAPI.Interfaces.Housing;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing;
using ManageATenancyAPI.Models.Housing.NHO;
using LBH.Utils;
using System.Reflection;
using System.Linq;

namespace ManageATenancyAPI.Validators
{
    internal class HousingQueryParameterValidator : IHousingQueryParameterValidator
    {
        public ValidationResult ValidatePropertyReferenceAndPostcode(string propertyReferenceNumber, string postcode)
        {
            var postcodeValidator = new PostcodeValidator();
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(propertyReferenceNumber))
            {
                validationResult.Valid = false;
                errorMessages.Add("Please provide propertyReferenceNumber");
            }
            else
            {
                string propertyReferencePattern = "^(228|299|229)[0-9]{6}$";
                if (!Regex.IsMatch(propertyReferenceNumber, propertyReferencePattern))
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Invalid parameter - propertyReferenceNumber is invalid.");
                }
            }
            if (!postcodeValidator.Validate(postcode))
            {
                validationResult.Valid = false;
                errorMessages.Add("Invalid parameter - postcode is invalid.");
            }
            validationResult.ErrorMessages = errorMessages;
            return validationResult;
        }

        public ValidationResult ValidateTagReference(string tagReference)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();
            if (tagReference != null)
            {
                tagReference.Trim();
                if (tagReference.Length > 11 || !tagReference.Contains("/"))
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Invalid parameter - tagReference is invalid");
                }
                else
                {
                    validationResult.Valid = true;
                }
                validationResult.ErrorMessages = errorMessages;
            }
            else
            {
                validationResult.Valid = false;
                errorMessages.Add("Invalid parameter - tagReference is empty");
                validationResult.ErrorMessages = errorMessages;
            }

            return validationResult;
        }

        public ValidationResult ValidatePaymentorTagReference(string referencenumber)
        {
            if (referencenumber != null && referencenumber.Contains("/"))
            {
                return ValidateTagReference(referencenumber);
            }
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(referencenumber))
            {
                validationResult.Valid = false;
                errorMessages.Add("Please provide referencenumber");
            }
            else
            {
                //If this API is to be used for Rent Account project, the following validation needs to be amended:
                string propertyReferencePattern = "^(228|299|229)[0-9]{6}$";
                if (!Regex.IsMatch(referencenumber, propertyReferencePattern))
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Invalid parameter - payment referenceNumber is invalid.");
                }
            }
            validationResult.ErrorMessages = errorMessages;
            return validationResult;
        }

        public ValidationResult validateCreateContactRequest(Contact contact)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();
            if (contact == null)
            {
                validationResult.Valid = false;
                errorMessages.Add("Object is empty");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(contact.CreatedByOfficer))
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Unauthorized");
                }
                if (string.IsNullOrWhiteSpace(contact.FirstName) || string.IsNullOrWhiteSpace(contact.LastName))
                {
                    validationResult.Valid = false;
                    errorMessages.Add("First name and last name must be provided.");
                }
            }
            validationResult.ErrorMessages = errorMessages;
            return validationResult;
        }
        public ValidationResult ValidateTenancyManagementinteractionrequest(TenancyManagement request)
        {
            var validationResult = new ValidationResult();

            if (request == null)
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide  a valid TenancyManagement interaction request");
                return validationResult;
            }
            if (string.IsNullOrWhiteSpace(request.contactId))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid ContactId");
            }
            if (request.ServiceRequest == null)
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide  a valid ServiceRequest for TenancyManagement interaction request");
                return validationResult;
            }
            if (string.IsNullOrWhiteSpace(request.enquirySubject))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid Subject for Tenancy Interaction");
            }
            if (string.IsNullOrWhiteSpace(request.areaName))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid Estate Office details for Tenancy Interaction");
            }
            if (string.IsNullOrWhiteSpace(request.officerPatchId) && string.IsNullOrWhiteSpace(request.managerId))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid Patch officer or a Manager");
            }
            if (string.IsNullOrWhiteSpace(request.estateOfficerId))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid Estate Officer details for Tenancy Interaction");
            }
            if (string.IsNullOrWhiteSpace(request.natureofEnquiry))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid Nature of Enquiry details for Tenancy Interaction");
            }
            if (string.IsNullOrWhiteSpace(request.source))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid Source details for Tenancy Interaction");
            }
            if (string.IsNullOrWhiteSpace(request.subject))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid Subject details for Tenancy Interaction");
            }
            if (string.IsNullOrWhiteSpace(request.ServiceRequest.Title))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid Title for ServiceRequest");
            }
            if (string.IsNullOrWhiteSpace(request.ServiceRequest.Description))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid Description for ServiceRequest");
            }
            if (string.IsNullOrWhiteSpace(request.processType))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid processType for ServiceRequest");
            }
            else
            {
                if (request.processType == "1" || request.processType == "2")
                {
                    if (string.IsNullOrWhiteSpace(request.householdId))
                    {
                        validationResult.Valid = false;
                        validationResult.ErrorMessages.Add(
                            "Please provide a valid householdId for Tenancy Management Process");
                    }
                }
                if (request.processType == "2")
                {
                    if (string.IsNullOrWhiteSpace(request.parentInteractionId))
                    {
                         validationResult.Valid = false;
                         validationResult.ErrorMessages.Add("Please provide a valid parentInteractionId for Tenancy Management Post Visit Action");

                    }
                    
                }
            }
            if (string.IsNullOrWhiteSpace(request.ServiceRequest.Subject))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid Subject for ServiceRequest");
            }

            return validationResult;
        }

        public ValidationResult ValidateServicerequest(CRMServiceRequest request)
        {
            var validationResult = new ValidationResult();

            if (request != null)
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    validationResult.Valid = false;
                    validationResult.ErrorMessages.Add("Please provide a valid Title for ServiceRequest");
                }
                if (Utils.NullToString(request.Description) != "" && string.IsNullOrWhiteSpace(request.Description))
                {
                    validationResult.Valid = false;
                    validationResult.ErrorMessages.Add("Please provide a valid Description for ServiceRequest");
                }
                if (Utils.NullToString(request.ContactId) != "" && string.IsNullOrWhiteSpace(request.ContactId))
                {
                    validationResult.Valid = false;
                    validationResult.ErrorMessages.Add("Please provide a valid ContactId for ServiceRequest");
                }
                if (Utils.NullToString(request.Subject) != "" && string.IsNullOrWhiteSpace(request.Subject))
                {
                    validationResult.Valid = false;
                    validationResult.ErrorMessages.Add("Please provide a valid Subject for ServiceRequest");
                }
            }
            else
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid ServiceRequest");
            }
            return validationResult;
        }
        public ValidationResult validateUpdateAccountRequest(string id, Contact contact)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();
            if (id == null)
            {
                validationResult.Valid = false;
                errorMessages.Add("Missing contact ID");
            }
            if (contact == null)
            {
                validationResult.Valid = false;
                errorMessages.Add("Object is empty");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(contact.UpdatedByOfficer))
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Unauthorized");
                }
            }
            validationResult.ErrorMessages = errorMessages;
            return validationResult;
        }

        public ValidationResult ValidatePatchTenancyManagementInteraction(TenancyManagement interaction)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();

            if (interaction != null && interaction.ServiceRequest!=null)
            {
                if (!string.IsNullOrWhiteSpace(interaction.ServiceRequest.Id))
                {
                    validationResult.Valid = true;
                }
                else
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Please provide a valid incidentId.");
                }
            }
            else
            {
                validationResult.Valid = false;
                errorMessages.Add("Please provide a valid TenancyManagemnt request.");
            }
            validationResult.ErrorMessages = errorMessages;
            return validationResult;
        }

        public ValidationResult ValidateGetTenancyManagementInteraction(string contactId, string personType)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();
            Guid guidOutput;
            bool isValid = Guid.TryParse(contactId, out guidOutput);

            if (isValid)
            {
                if (!string.IsNullOrEmpty(personType))
                {
                    if (personType == "contact" || personType == "manager" || personType == "officer")
                        validationResult.Valid = true;
                    else
                    {
                        validationResult.Valid = false;
                        errorMessages.Add("Please provide a valid personType.");
                    }
                }
                else
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Required variable personType is empty.");
                }
            }
            else
            {

                validationResult.Valid = false;
                errorMessages.Add("Please provide a valid contact.");
            }
            validationResult.ErrorMessages = errorMessages;
            return validationResult;
        }

        public ValidationResult ValidateGetAreaTrayIneractions(string OfficeId)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(OfficeId))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid office number for Group Tray");
            }

            validationResult.ErrorMessages = errorMessages;
            return validationResult;
        }

        public ValidationResult ValidateGetTenancyManagementIneraction(string ineractionId)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(ineractionId))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid ineractionId for Tenancy Management Ineraction");
            }

            validationResult.ErrorMessages = errorMessages;
            return validationResult;
        }

      
        public ValidationResult ValidateSearchRequest(string firstname, string surname, string addressline12, string postcode)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();

            if (string.IsNullOrEmpty(firstname) && string.IsNullOrEmpty(surname) && string.IsNullOrEmpty(addressline12) && string.IsNullOrEmpty(postcode))
            {
                validationResult.Valid = false;
                errorMessages.Add("Please provide atleast one search criteria");
            }
            validationResult.ErrorMessages = errorMessages;
            return validationResult;

        }

        public ValidationResult ValidateAccountType(string type)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();

            if (!string.IsNullOrEmpty(type))
            {
                if (type != "1" && type != "2")
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Please provide a valid account type");
                }
            }
            else
            {
                validationResult.Valid = false;
                errorMessages.Add("Please provide an account type");
            }
            validationResult.ErrorMessages = errorMessages;
            return validationResult;

        }

        public ValidationResult ValidateGetAllOfficersPerArea(string areaId)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(areaId))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid area number");
            }

            validationResult.ErrorMessages = errorMessages;
            return validationResult;
        }

        public ValidationResult ValidateTransferCallToPatchAndArea(TenancyManagement interaction)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();

            if (interaction != null )
            {
                if (string.IsNullOrWhiteSpace(interaction.interactionId))
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Please provide a valid Interaction Id.");
                }
                if (string.IsNullOrWhiteSpace(interaction.officerPatchId) && (string.IsNullOrWhiteSpace(interaction.managerId)))
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Please provide a valid officer PatchId.");
                }
                if (string.IsNullOrWhiteSpace(interaction.estateOfficerId))
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Please provide a valid estateOfficerId.");
                }
                if (string.IsNullOrWhiteSpace(interaction.areaName))
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Please provide a valid areaName.");
                }

            }
            else
            {
                validationResult.Valid = false;
                errorMessages.Add("Please provide a valid TenancyManagemnt request.");
            }
            validationResult.ErrorMessages = errorMessages;
            return validationResult;
        }

        public ValidationResult ValidateRemoveCautionaryAlert(CautionaryAlert cautionaryAlert)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();

            if (cautionaryAlert != null)
            {
                if (string.IsNullOrWhiteSpace(cautionaryAlert.contactId))
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Please provide a contact id.");
                }
                if (cautionaryAlert.cautionaryAlertIds.Count == 0)
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Please at least one cautionary alert id.");
                }
              
            }
            else
            {
                validationResult.Valid = false;
                errorMessages.Add("Please provide a valid 'Remove cautionary alert' request.");
            }
            validationResult.ErrorMessages = errorMessages;
            return validationResult;
        }

        public ValidationResult ValidateCreateCautionaryAlert(CautionaryAlert cautionaryAlert)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();

            if (cautionaryAlert != null)
            {
                if (string.IsNullOrWhiteSpace(cautionaryAlert.contactId))
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Please provide a contact id.");
                }
                if (cautionaryAlert.cautionaryAlertType.Count == 0)
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Please at least one cautionary alert type.");
                }
                if (string.IsNullOrWhiteSpace(cautionaryAlert.uprn))
                {
                    validationResult.Valid = false;
                    errorMessages.Add("Please provide a uprn.");
                }

            }
            else
            {
                validationResult.Valid = false;
                errorMessages.Add("Please provide a valid 'Create cautionary alert' request.");
            }
            validationResult.ErrorMessages = errorMessages;
            return validationResult;
        }

        public ValidationResult ValidatehackneyhomesId(string hackneyhomesId)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(hackneyhomesId))
            {
                validationResult.Valid = false;
                errorMessages.Add("Please provide hackneyhomesId");
            }
           
            validationResult.ErrorMessages = errorMessages;
            return validationResult;
        }

        public ValidationResult ValidateContactId(string contactid)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(contactid))
            {
                validationResult.Valid = false;
                errorMessages.Add("Please provide contactid");
            }

            validationResult.ErrorMessages = errorMessages;
            return validationResult;
        }

        public ValidationResult ValidateUpdateOfficerPatchOrAreaManager(OfficerAreaPatch officerPatch)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();
            if (officerPatch != null)
            {
                if (officerPatch.isUpdatingPatch)
                {
                    if (string.IsNullOrWhiteSpace(officerPatch.patchId))
                    {
                        validationResult.Valid = false;
                        validationResult.ErrorMessages.Add("Please provide a valid patch id");
                    }
                }
                if (!officerPatch.isUpdatingPatch)
                {
                    if (string.IsNullOrWhiteSpace(officerPatch.areamanagerId))
                    {
                        validationResult.Valid = false;
                        validationResult.ErrorMessages.Add("Please provide a valid area manager id");
                    }
                }
                if (string.IsNullOrWhiteSpace(officerPatch.officerId))
                {
                    validationResult.Valid = false;
                    validationResult.ErrorMessages.Add("Please provide a officer id");
                }
               
                if (string.IsNullOrWhiteSpace(officerPatch.updatedByOfficer))
                {
                    validationResult.Valid = false;
                    validationResult.ErrorMessages.Add("Please provide an id for officer who is updating the patch");
                }
            }
            else
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Object is empty");
            }
            validationResult.ErrorMessages = errorMessages;
            return validationResult;
        }

        public ValidationResult ValidateEstateOfficerAccount(EstateOfficerAccount estateOfficerAccount)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();
            
            if (!IsNullOrEmpty(estateOfficerAccount.OfficerAccount)) 
            {
                validationResult.Valid = false;
                errorMessages.Add("EstateOfficerAccount object or any of its state member is empty or null.");

            }else if(!IsNullOrEmpty(estateOfficerAccount.OfficerLoginAccount))
            {
                validationResult.Valid = false;
                errorMessages.Add("EstateOfficerLoginAccount object or any of its state member is empty or null.");
            }
            
            validationResult.ErrorMessages = errorMessages;
            return validationResult;

        }


        public ValidationResult ValidateOfficerAccountIds(string officerId, string officerLoginId)
        {
            var validationResult = new ValidationResult();
            var errorMessages = new List<string>();

            if (string.IsNullOrEmpty(officerId))
            {
                validationResult.Valid = false;
                errorMessages.Add("EstateOfficerAccount Id Can't be Null or Empty");

            }
            else if (string.IsNullOrEmpty(officerLoginId))
            {
                validationResult.Valid = false;
                errorMessages.Add("EstateOfficerLoginAccount  Id Can't be Null or Empty");
            }

            validationResult.ErrorMessages = errorMessages;
            return validationResult;

        }

        private bool IsNullOrEmpty(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return false;

            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(obj);
                    if (string.IsNullOrEmpty(value))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}