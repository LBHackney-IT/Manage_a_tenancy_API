using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models;

namespace ManageATenancyAPI.Interfaces.Housing
{
    public interface IHousingQueryParameterValidator
    {
        ValidationResult ValidatePropertyReferenceAndPostcode(string propertyReferenceNumber,string postcode);

        ValidationResult ValidateTagReference(string tagReference);
    }
}
