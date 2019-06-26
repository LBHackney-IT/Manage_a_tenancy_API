using System;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Gateways.SaveEtraMeeting;
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

        public Task<SaveETRAMeetingOutputModel> ExecuteAsync(SaveETRAMeetingInputModel request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}