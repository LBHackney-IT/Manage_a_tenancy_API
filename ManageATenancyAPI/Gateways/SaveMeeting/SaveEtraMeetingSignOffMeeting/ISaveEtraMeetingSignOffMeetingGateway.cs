using System;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting
{
    public interface ISaveEtraMeetingSignOffMeetingGateway
    {
        Task<bool> SignOffMeetingAsync(Guid meetingId, SignOff signOff, CancellationToken cancellationToken);
    }
}