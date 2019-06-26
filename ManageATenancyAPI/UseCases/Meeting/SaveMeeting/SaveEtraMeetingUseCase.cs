using System;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Gateways.SaveEtraMeeting;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting
{
    public class SaveEtraMeetingUseCase : ISaveEtraMeetingUseCase
    {
        private readonly ISaveEtraMeetingGateway _saveEtraMeetingGateway;

        public SaveEtraMeetingUseCase(ISaveEtraMeetingGateway saveEtraMeetingGateway)
        {
            _saveEtraMeetingGateway = saveEtraMeetingGateway;
        }

        public async Task<SaveETRAMeetingOutputModel> ExecuteAsync(SaveETRAMeetingInputModel request, IManageATenancyClaims claims, CancellationToken cancellationToken)
        {
            var etraMeeting = new ETRAMeeting
            {
                MeetingName = request.MeetingName,
                TraId = request.TRAId,
            };
            var meetingId = await _saveEtraMeetingGateway.CreateEtraMeeting(etraMeeting, claims, cancellationToken).ConfigureAwait(false);

            return new SaveETRAMeetingOutputModel
            {
                MeetingId = meetingId
            };
        }
    }
}