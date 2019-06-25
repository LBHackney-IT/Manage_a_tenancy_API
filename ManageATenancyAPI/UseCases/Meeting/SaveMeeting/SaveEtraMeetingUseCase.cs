using System;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting
{
    public class SaveEtraMeetingUseCase : ISaveEtraMeetingUseCase
    {

        public SaveEtraMeetingUseCase()
        {

        }

        public Task<SaveETRAMeetingOutputModel> ExecuteAsync(SaveETRAMeetingInputModel request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}