using System;

namespace ManageATenancyAPI.Interfaces.Housing
{
    public class CreateETRAMeetingActionResponse
    {
        public Guid? IncidentId { get; set; }
        public Guid? InteractionId { get; set; }
        public string TicketNumber { get; set; }
        public string AnnotationId { get; set; }
    }
}