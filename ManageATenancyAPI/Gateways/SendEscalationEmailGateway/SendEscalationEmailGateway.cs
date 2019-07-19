using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Services.Email;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Gateways.SendEscalationEmailGateway
{
    public class SendEscalationEmailGateway : ISendEscalationEmailGateway
    {
        private readonly INotificationClient _notificationClient;
        private readonly IOptions<EmailConfiguration> _config;

        public SendEscalationEmailGateway(INotificationClient notificationClient, IOptions<EmailConfiguration> config)
        {
            _notificationClient = notificationClient;
            _config = config;
        }

        public async Task<SendEscalationEmailOutputModel> SendEscalationEmailAsync(SendEscalationEmailInputModel inputModel, CancellationToken cancellationToken)
        {

            var personalization = new Dictionary<string, object>
            {
                {EmailKeys.EscalationEmail.ServiceAreaOfficerName, inputModel.ServiceArea.ServiceAreaOfficer},
                {EmailKeys.EscalationEmail.ServiceAreaManagerName, inputModel.ServiceArea.ServiceAreaManager},
                {EmailKeys.EscalationEmail.DateResponseWasDue, inputModel.DateResponseWasDue},
                {EmailKeys.EscalationEmail.IssueType, inputModel.Issue.IssueType},
                {EmailKeys.EscalationEmail.Location, inputModel.Issue.Location},
                {EmailKeys.EscalationEmail.IssueNotes, inputModel.Issue.Notes},
                {EmailKeys.EscalationEmail.HousingOfficerName, inputModel.HousingOfficerName},
            };

            var outputModel = new SendEscalationEmailOutputModel();

            if (!string.IsNullOrEmpty(inputModel?.ServiceArea.ServiceAreaManagerEmail))
            {
                await _notificationClient.SendEmailAsync(inputModel?.ServiceArea.ServiceAreaManagerEmail, _config?.Value.EscalationTemplateId, personalization).ConfigureAwait(false);
                outputModel.SentToServiceAreaOfficer = true;
            }

            if (!string.IsNullOrEmpty(inputModel?.ServiceArea.ServiceAreaOfficerEmail))
            {
                await _notificationClient.SendEmailAsync(inputModel?.ServiceArea.ServiceAreaOfficerEmail, _config?.Value.EscalationTemplateId, personalization).ConfigureAwait(false);
                outputModel.SentToServiceAreaManager = true;
            }

            if (!string.IsNullOrEmpty(inputModel?.AreaManagerDetails.Email))
            {
                await _notificationClient.SendEmailAsync(inputModel?.AreaManagerDetails.Email, _config?.Value.EscalationTemplateId, personalization).ConfigureAwait(false);
                outputModel.SentToServiceAreaManager = true;
            }

            return outputModel;
        }
    }
}