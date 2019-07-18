using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;

namespace ManageATenancyAPI.Gateways.SendEscalationEmailGateway
{
    public class SendEscalationEmailGateway : ISendEscalationEmailGateway
    {
        private readonly IGetServiceAreaInformationGateway _getServiceAreaInformationGateway;

        public SendEscalationEmailGateway(IGetServiceAreaInformationGateway getServiceAreaInformationGateway)
        {
            _getServiceAreaInformationGateway = getServiceAreaInformationGateway;
        }

        public Task<SendEscalationEmailOutputModel> SendEscalationEmailAsync(SendEscalationEmailInputModel inputModel, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public interface IGetServiceAreaInformationGateway
    {
        Task<IList<TRAIssueServiceArea>> GetServiceAreaEmails(CancellationToken cancellationToken);
    }

    public class GetServiceAreaInformationGateway : IGetServiceAreaInformationGateway
    {
        public Task<IList<TRAIssueServiceArea>> GetServiceAreaEmails(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}