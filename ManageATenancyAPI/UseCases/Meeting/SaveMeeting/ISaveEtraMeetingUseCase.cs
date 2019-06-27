using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting
{
    public interface ISaveEtraMeetingUseCase 
    {
        Task<SaveETRAMeetingOutputModel> ExecuteAsync(SaveETRAMeetingInputModel request, IManageATenancyClaims claims, CancellationToken cancellationToken);
    }
}