using System;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces.Housing;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingAttendance
{
    public interface ISaveEtraMeetingFinaliseMeetingGateway
    {
        Task<bool> FinaliseMeeting(Guid meetingId, CancellationToken cancellationToken);
    }

    public class SaveEtraMeetingFinaliseMeetingGateway: ISaveEtraMeetingFinaliseMeetingGateway
    {
        private readonly IETRAMeetingsAction _etraMeetingsAction;

        public SaveEtraMeetingFinaliseMeetingGateway(IETRAMeetingsAction etraMeetingsAction)
        {
            _etraMeetingsAction = etraMeetingsAction;
        }

        public Task<bool> FinaliseMeeting(Guid meetingId, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}