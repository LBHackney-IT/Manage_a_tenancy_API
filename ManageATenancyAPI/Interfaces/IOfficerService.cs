using ManageATenancyAPI.Models.Housing.NHO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Interfaces
{
    public interface IOfficerService
    {
        Task<OfficerDetails> GetOfficerDetails(string emailAddress);
        Task<IList<NewTenancyResponse>> GetNewTenanciesForHousingOfficer(string id, DateTime lastCheckDate);
        Task UpdateLastNewTenancyCheckDate(string id, DateTime date);
    }
}