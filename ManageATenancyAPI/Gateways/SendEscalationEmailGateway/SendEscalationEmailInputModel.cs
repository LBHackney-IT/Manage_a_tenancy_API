using System;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;

namespace ManageATenancyAPI.Gateways.SendEscalationEmailGateway
{
    public class SendEscalationEmailInputModel
    {
        public GetEtraMeetingOutputModel Issue { get; set; }
        public string ResponseLink { get; set; }
        public DateTime DateResponseWasDue { get; set; } 
        
        public string HousingOfficerName { get; set; }
        public string NHOAddress { get; set; }
    }
}