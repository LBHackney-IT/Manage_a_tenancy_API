using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingAttendance
{
    public interface ISaveEtraMeetingAttendanceGateway
    {
        Task<bool> CreateEtraAttendance(ETRAMeeting meeting, MeetingAttendees attendees, CancellationToken cancellationToken);
    }
}