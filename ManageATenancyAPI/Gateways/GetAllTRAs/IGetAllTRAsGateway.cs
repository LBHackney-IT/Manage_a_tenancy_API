using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Database.Models;

namespace ManageATenancyAPI.Gateways.GetAllTRAs
{
    public interface IGetAllTRAsGateway
    {
        Task<IList<TRA>> GetAllTRAsAsync(CancellationToken cancellationToken);
    }
}