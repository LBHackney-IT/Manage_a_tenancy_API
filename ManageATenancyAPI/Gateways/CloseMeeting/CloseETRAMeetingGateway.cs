using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeeting;
using ManageATenancyAPI.Helpers;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Gateways.CloseMeeting
{
    public class CloseETRAMeetingGateway : ICloseETRAMeetingGateway
    {
        private readonly IETRAMeetingsAction _eTRAMeetingsAction;
        public CloseETRAMeetingGateway(IETRAMeetingsAction eTRAMeetingsAction)
        {
            _eTRAMeetingsAction = eTRAMeetingsAction;
        }
        public async Task<bool> CloseMeetingInteraction(CloseMeetingInputModel inputModel, CancellationToken cancellationToken)
        {
            var closeMeetingInputModel = new CloseETRAMeetingRequest();
            closeMeetingInputModel.InteractionId = inputModel.InteractionId;
            closeMeetingInputModel.MeetingStage = HackneyProcessStage.Completed;
            closeMeetingInputModel.UpdatedByOfficerId = inputModel.UpdatedByOfficer;
            _eTRAMeetingsAction.CloseMeetingInteraction(closeMeetingInputModel);
            return true;
        }

        public async Task<bool> CloseMeetingIncident(CloseMeetingInputModel inputModel, CancellationToken cancellationToken)
        {
            _eTRAMeetingsAction.CloseIncident("",inputModel.IncidentId);

            return true;
        }
    }

    public class CloseMeetingInputModel
    {
        public Guid InteractionId { get; set; }

        public Guid IncidentId { get; set; }
        public Guid UpdatedByOfficer { get; set; }
        public HackneyProcessStage MeetingStage { get; set; }

    }
}
