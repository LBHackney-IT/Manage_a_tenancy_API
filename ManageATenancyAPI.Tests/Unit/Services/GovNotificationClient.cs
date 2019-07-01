﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Notify.Client;
using Notify.Models.Responses;

namespace ManageATenancyAPI.Tests.Unit.Services
{
    public class GovNotificationClient:INotificationClient
    {
        private NotificationClient _client;
        public GovNotificationClient(string apiKey)
        {
            _client = new NotificationClient(apiKey);
        }

        public async Task<EmailNotificationResponse> SendEmailAsync(string emailAddress, string templateId, Dictionary<string, object> personalisation = null,
            string clientReference = null, string emailReplyToId = null)
        {
            return await _client.SendEmailAsync(emailAddress, templateId, personalisation, clientReference, emailReplyToId).ConfigureAwait(false);
        }
    }
}