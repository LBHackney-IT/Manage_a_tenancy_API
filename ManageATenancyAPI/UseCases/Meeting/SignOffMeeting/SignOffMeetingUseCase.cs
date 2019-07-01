using System;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting;
using ManageATenancyAPI.UseCases.Meeting.SignOffMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.SignOffMeeting
{
    public class SignOffMeetingUseCase: ISignOffMeetingUseCase
    {
        private readonly ISaveEtraMeetingSignOffMeetingGateway _saveEtraMeetingSignOffMeetingGateway;

        public SignOffMeetingUseCase(ISaveEtraMeetingSignOffMeetingGateway saveEtraMeetingSignOffMeetingGateway)
        {
            _saveEtraMeetingSignOffMeetingGateway = saveEtraMeetingSignOffMeetingGateway;
        }

        public async Task<SignOffMeetingOutputModel> ExecuteAsync(SignOffMeetingInputModel request, CancellationToken cancellationToken)
        {
            var outputModel = await _saveEtraMeetingSignOffMeetingGateway.SignOffMeetingAsync(request.MeetingId, request?.SignOff, cancellationToken).ConfigureAwait(false);
            return outputModel;
        }
    }
}