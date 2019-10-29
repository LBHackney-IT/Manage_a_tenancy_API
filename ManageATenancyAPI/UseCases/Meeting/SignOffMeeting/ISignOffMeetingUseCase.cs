using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.UseCases.Meeting.SignOffMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.SignOffMeeting
{
    public interface ISignOffMeetingUseCase
    {
        Task<SignOffMeetingOutputModel> ExecuteAsync(SignOffMeetingInputModel request, IMeetingClaims claims, CancellationToken cancellationToken);
    }
}
