using System;
using System.Buffers.Text;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting;
using ManageATenancyAPI.Interfaces.Housing;

namespace ManageATenancyAPI.UseCases.Meeting.GetMeeting
{
    public class GetEtraMeetingUseCase: IGetEtraMeetingUseCase
    {
        private readonly IETRAMeetingsAction _etraMeetingsAction;
        private readonly IJpegPersistenceService _jpegPersistenceService;

        public GetEtraMeetingUseCase(IETRAMeetingsAction etraMeetingsAction, IJpegPersistenceService jpegPersistenceService)
        {
            _etraMeetingsAction = etraMeetingsAction;
            _jpegPersistenceService = jpegPersistenceService;
        }
        public async Task<GetEtraMeetingOutputModel> ExecuteAsync(GetEtraMeetingInputModel request, CancellationToken cancellationToken)
        {
            var getEtraMeetingOutputModel = await _etraMeetingsAction.GetMeetingV2Async(request.MeetingId, cancellationToken).ConfigureAwait(false);
            getEtraMeetingOutputModel.Issues = await _etraMeetingsAction.GetETRAIssuesForMeeting(request.MeetingId, cancellationToken).ConfigureAwait(false);

            if (getEtraMeetingOutputModel?.SignOff != null &&
                getEtraMeetingOutputModel?.SignOff.SignatureId != Guid.Empty)
            { 
                var bytes = await _jpegPersistenceService.GetAsync(getEtraMeetingOutputModel?.SignOff?.SignatureId.ToString()).ConfigureAwait(false);
                string base64Signature = System.Convert.ToBase64String(bytes);
                getEtraMeetingOutputModel.SignOff.Signature = base64Signature;
            }
                

            return getEtraMeetingOutputModel;
        }
    }
}