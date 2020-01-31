using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using ETRAMeeting = ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeeting.ETRAMeeting;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingAttendance
{
    public class SaveEtraMeetingAttendanceGateway : ISaveEtraMeetingAttendanceGateway
    {
        private readonly IETRAMeetingsAction _etraMeetingsAction;

        public SaveEtraMeetingAttendanceGateway(IETRAMeetingsAction etraMeetingsAction)
        {
            _etraMeetingsAction = etraMeetingsAction;
        }

        public async Task<bool> CreateEtraAttendance(ETRAMeeting meeting, MeetingAttendees attendees, CancellationToken cancellationToken)
        {
            var recordEtraMeetingAttendanceRequest = new RecordETRAMeetingAttendanceRequest
            {
                Councillors = attendees?.Councillors,
                OtherCouncilStaff = attendees?.HackneyStaff,
                TotalAttendees = attendees.Attendees 
            };

            var response = await _etraMeetingsAction.RecordETRAMeetingAttendance(meeting.Id.ToString(), recordEtraMeetingAttendanceRequest).ConfigureAwait(false);

            return response.Recorded;
        }
    }
}