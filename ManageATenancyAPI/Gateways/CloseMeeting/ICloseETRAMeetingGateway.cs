using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Gateways.CloseMeeting
{
    public interface ICloseETRAMeetingGateway
    {
        Task<bool> CloseMeetingInteraction(CloseMeetingInputModel inputModel, CancellationToken cancellationToken);

        Task<bool> CloseMeetingIncident(CloseMeetingInputModel inputModel, CancellationToken cancellationToken);
    }
}
