﻿using System;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.Gateways.SendEscalationEmailGateway
{
    public class SendEscalationEmailInputModel
    {
        public EscalateMeetingIssueInputModel Issue { get; set; }

        public TRAIssueServiceArea ServiceArea { get; set; }

        public AreaManagerDetails AreaManagerDetails { get; set; }

        public string ResponseLink { get; set; }
        public DateTime DateResponseWasDue { get; set; }

        public string NHOAddress { get; set; }
    }
}