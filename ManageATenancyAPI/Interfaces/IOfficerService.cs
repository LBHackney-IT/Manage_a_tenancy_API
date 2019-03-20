using ManageATenancyAPI.Models.Housing.NHO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Interfaces
{
    public interface IOfficerService
    {
        Task<OfficerDetails> GetOfficerDetails(string emailAddress);
        Task<IList<NewTenancyResponse>> GetNewTenanciesForHousingOfficer(OfficerDetails officer);
        Task UpdateLastNewTenancyCheckDate(string id, DateTime date);
    }
}