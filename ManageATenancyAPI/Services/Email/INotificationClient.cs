﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Notify.Models.Responses;

namespace ManageATenancyAPI.Services.Email
{
    public interface INotificationClient
    {
        Task<EmailNotificationResponse> SendEmailAsync(
            string emailAddress,
            string templateId,
            Dictionary<string, object> personalisation = null,
            string clientReference = null,
            string emailReplyToId = null);
    }
}