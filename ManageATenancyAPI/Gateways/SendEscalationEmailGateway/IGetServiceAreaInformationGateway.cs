using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;

namespace ManageATenancyAPI.Gateways.SendEscalationEmailGateway
{
    public interface IGetServiceAreaInformationGateway
    {
        Task<IList<TRAIssueServiceArea>> GetServiceAreaEmails(CancellationToken cancellationToken);
    }
}