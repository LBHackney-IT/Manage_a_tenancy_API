using ManageATenancyAPI.Models;
using System.Collections.Generic;

namespace ManageATenancyAPI.Interfaces
{
    public interface ICitizenIndexRepository
    {
        List<CIPerson> SearchCitizenIndex(string firstname, string surname, string addressline12, string postcode);
    }
}
