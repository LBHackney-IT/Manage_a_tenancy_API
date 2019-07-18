using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.Gateways.GetTraIssuesThatNeedEscalating
{
    public interface IGetTraIssuesThatNeedEscalatingGateway
    {
        Task<IList<MeetingIssueOutputModel>> GetTraIssuesThatNeedEscalating(CancellationToken cancellationToken);
    }
}
