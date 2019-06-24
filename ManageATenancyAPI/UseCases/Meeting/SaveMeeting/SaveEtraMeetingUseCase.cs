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

        public async Task<EtraSaveMeetingsResponse> ExecuteAsync(EtraSaveMeetingsRequest request, CancellationToken cancellationToken)
        {
            var createEtraMeeting = _etraMeetingsAction.CreateETRAMeeting(etraMeeting).Result;
        }
    }
}