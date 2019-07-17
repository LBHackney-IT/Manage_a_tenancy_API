using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;

namespace ManageATenancyAPI.Gateways.GetTraIssuesThatNeedEscalating
{
    public interface IGetTraIssuesThatNeedEscalatingGateway
    {
        Task<IList<TRAIssue>> GetTraIssuesThatNeedEscalating(CancellationToken cancellationToken);
    }
}
