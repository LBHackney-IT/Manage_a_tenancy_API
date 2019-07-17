using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Gateways.SendEscalationEmailGateway
{
    public class SendEscalationEmailGateway : ISendEscalationEmailGateway
    {
        public Task SendEscalationEmailAsync(SendEscalationEmailInputModel inputModel, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}