using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Gateways.SendEscalationEmailGateway
{
    public interface ISendEscalationEmailGateway
    {
        Task<SendEscalationEmailOutputModel> SendEscalationEmailAsync(SendEscalationEmailInputModel inputModel, CancellationToken cancellationToken);
    }
}
