using ManageATenancyAPI.Models.Housing.NHO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Interfaces
{
    public interface ITenancyService
    {
        Task<IEnumerable<NewTenancyResponse>> GetNewTenancies();
    }
}