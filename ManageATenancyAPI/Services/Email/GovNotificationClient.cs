using System.Collections.Generic;
using System.Threading.Tasks;
using ManageATenancyAPI.Configuration;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Models.Responses;

namespace ManageATenancyAPI.Tests.Unit.Services
{
    public class GovNotificationClient:INotificationClient
    {
        private NotificationClient _client;
        public GovNotificationClient(IOptions<EmailConfiguration> config)
        {
            _client = new NotificationClient(config?.Value.ApiKey);
        }

        public async Task<EmailNotificationResponse> SendEmailAsync(string emailAddress, string templateId, Dictionary<string, object> personalisation = null,
            string clientReference = null, string emailReplyToId = null)
        {
            return await _client.SendEmailAsync(emailAddress, templateId, personalisation, clientReference, emailReplyToId).ConfigureAwait(false);
        }
    }
}