using System;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using ManageATenancyAPI.UseCases.Meeting.SignOffMeeting.Boundary;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting
{
    public interface ISaveEtraMeetingSignOffMeetingGateway
    {
        Task<SignOffMeetingOutputModel> SignOffMeetingAsync(Guid meetingId, SignOff signOff, CancellationToken cancellationToken);
    }
}