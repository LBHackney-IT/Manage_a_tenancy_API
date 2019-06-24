using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Database.Models;

namespace ManageATenancyAPI.Gateways.GetAllTRAs
{


    public interface IGetAllTRAsGateway
    {
        Task<IList<TRA>> GetAllTRAs(CancellationToken cancellationToken);
    }

    public class GetAllTRAsGateway: IGetAllTRAsGateway
    {
        public async Task<IList<TRA>> GetAllTRAs(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
