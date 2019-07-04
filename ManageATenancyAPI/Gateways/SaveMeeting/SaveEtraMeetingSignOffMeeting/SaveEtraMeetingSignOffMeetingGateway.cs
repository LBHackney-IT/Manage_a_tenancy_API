using System;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using ManageATenancyAPI.UseCases.Meeting.SignOffMeeting.Boundary;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting
{
    public class SaveEtraMeetingSignOffMeetingGateway: ISaveEtraMeetingSignOffMeetingGateway
    {
        private readonly IETRAMeetingsAction _etraMeetingsAction;
        private readonly IJpegPersistenceService _jpegPersistenceService;

        public SaveEtraMeetingSignOffMeetingGateway(IETRAMeetingsAction etraMeetingsAction, IJpegPersistenceService jpegPersistenceService)
        {
            _etraMeetingsAction = etraMeetingsAction;
            _jpegPersistenceService = jpegPersistenceService;
        }

        public async Task<SignOffMeetingOutputModel> SignOffMeetingAsync(Guid meetingId, SignOff signOff, CancellationToken cancellationToken)
        {
            var finaliseMeetingRequest = new FinaliseETRAMeetingRequest
            {
                Name = signOff.Name,
                Role = signOff.Role,
            };

            if (!string.IsNullOrEmpty(signOff.Signature))
            {
                var signatureGuid = Guid.NewGuid();
                var bytes = System.Convert.FromBase64String(signOff.Signature);
                await _jpegPersistenceService.SaveAsync(signatureGuid.ToString(), bytes).ConfigureAwait(false);
                finaliseMeetingRequest.SignatureId = signatureGuid;
                signOff.SignatureId = signatureGuid;
            }

            var response = await _etraMeetingsAction.FinaliseMeeting(meetingId.ToString(), finaliseMeetingRequest).ConfigureAwait(false);
            signOff.SignOffDate = response.SignOffDate;
            return new SignOffMeetingOutputModel
            {
                Id = meetingId,
                SignOff = signOff,
                IsSignedOff = response.IsFinalised
            };
        }
    }
}