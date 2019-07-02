using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces.Housing;

namespace ManageATenancyAPI.UseCases.Meeting.GetMeeting
{
    public class GetEtraMeetingUseCase: IGetEtraMeetingUseCase
    {
        private readonly IETRAMeetingsAction _etraMeetingsAction;

        public GetEtraMeetingUseCase(IETRAMeetingsAction etraMeetingsAction)
        {
            _etraMeetingsAction = etraMeetingsAction;
        }
        public async Task<GetEtraMeetingOutputModel> ExecuteAsync(GetEtraMeetingInputModel request, CancellationToken cancellationToken)
        {
            var getEtraMeetingOutputModel = await _etraMeetingsAction.GetMeetingV2Async(request.MeetingId, cancellationToken).ConfigureAwait(false);
            getEtraMeetingOutputModel.Issues = await _etraMeetingsAction.GetETRAIssuesForMeeting(request.MeetingId, cancellationToken).ConfigureAwait(false);

            return getEtraMeetingOutputModel;
        }
    }
}