﻿namespace ManageATenancyAPI.Configuration
{
    public class EmailConfiguration
    {
        public string ApiKey { get; set; }
        public string FrontEndAppUrl { get; set; }
        public string TemplateId { get; set; }
        public string EscalationTemplateId { get; set; }
        public string ConfirmationTemplate { get; set; }
        public string AHMEscalationTemplate { get; set; }
        
    }
}