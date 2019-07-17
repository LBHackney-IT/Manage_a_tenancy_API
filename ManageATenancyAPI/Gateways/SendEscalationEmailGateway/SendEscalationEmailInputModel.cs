using System;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;

namespace ManageATenancyAPI.Gateways.SendEscalationEmailGateway
{
    public class SendEscalationEmailInputModel
    {
        public TRAIssue Issue { get; set; }
        public string ResponseLink { get; set; }
        public DateTime DateResponseWasDue { get; set; } 
        
        public string HousingOfficerName { get; set; }
        public string NHOAddress { get; set; }
    }
}