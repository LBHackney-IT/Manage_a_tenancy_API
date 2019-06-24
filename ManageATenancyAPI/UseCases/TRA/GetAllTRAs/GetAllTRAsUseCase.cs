using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Controllers.v2;
using ManageATenancyAPI.Gateways.GetAllTRAs;

namespace ManageATenancyAPI.UseCases.TRA.GetAllTRAs
{
    public class GetAllTRAsUseCase: IGetAllTRAsUseCase
    {
        private readonly IGetAllTRAsGateway _gateway;

        public GetAllTRAsUseCase(IGetAllTRAsGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<GetAllTRAsOutputModel> ExecuteAsync(CancellationToken cancellationToken)
        { 
            await _gateway.GetAllTRAs(cancellationToken).ConfigureAwait(false);
            return new GetAllTRAsOutputModel();
        }
    }
}
