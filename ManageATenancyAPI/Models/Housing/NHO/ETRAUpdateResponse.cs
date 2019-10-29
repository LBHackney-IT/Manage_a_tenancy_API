using System;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class ETRAUpdateResponse
    {
        public Guid IncidentId { get; set; }
        public Guid InteractionId { get; set; }
        public Guid? AnnotationId { get; set; }
        public string Action { get; set; }
    }
}