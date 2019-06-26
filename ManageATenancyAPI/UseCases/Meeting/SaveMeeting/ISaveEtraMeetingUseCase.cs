using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting
{
    public interface ISaveEtraMeetingUseCase 
    {
        Task<SaveETRAMeetingOutputModel> ExecuteAsync(SaveETRAMeetingInputModel request, CancellationToken cancellationToken);
    }
}