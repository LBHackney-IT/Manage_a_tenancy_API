using System;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Services.Email;

namespace ManageATenancyAPI.Gateways.SendEscalationEmailGateway
{
    public class SendEscalationEmailGateway : ISendEscalationEmailGateway
    {
        private readonly INotificationClient _notificationClient;

        public SendEscalationEmailGateway(INotificationClient notificationClient)
        {
            _notificationClient = notificationClient;
        }

        public async Task<SendEscalationEmailOutputModel> SendEscalationEmailAsync(SendEscalationEmailInputModel inputModel, CancellationToken cancellationToken)
        {

            throw new NotImplementedException();
        }
    }
}