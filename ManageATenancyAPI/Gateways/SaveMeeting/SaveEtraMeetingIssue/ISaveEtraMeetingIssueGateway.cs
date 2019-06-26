using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using ETRAMeeting = ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeeting.ETRAMeeting;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingIssue
{
    public interface ISaveEtraMeetingIssueGateway
    {
        Task<MeetingIssueOutputModel> CreateEtraMeetingIssue(ETRAMeeting meeting, MeetingIssue meetingIssue, IManageATenancyClaims manageATenancyClaims, CancellationToken cancellationToken);
    }
}