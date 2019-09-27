using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Services.Email;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Gateways.SendEscalationEmailGateway
{
    public class SendEscalationEmailGateway : ISendEscalationEmailGateway
    {
        private readonly INotificationClient _notificationClient;
        private readonly IOptions<EmailConfiguration> _config;
        private readonly ITraAction _traAction;

        public SendEscalationEmailGateway(INotificationClient notificationClient, IOptions<EmailConfiguration> config, ITraAction traAction)
        {
            _notificationClient = notificationClient;
            _config = config;
            _traAction = traAction;
        }

        public async Task<SendEscalationEmailOutputModel> SendEscalationEmailAsync(SendEscalationEmailInputModel inputModel, CancellationToken cancellationToken)
        {

            var tra = await _traAction.GetAsync(inputModel.Issue.TraId).ConfigureAwait(false);
            var personalization = new Dictionary<string, object>
            {
                {EmailKeys.EscalationEmail.ServiceAreaOfficerName, inputModel.ServiceArea.ServiceAreaOfficer},
                {EmailKeys.EscalationEmail.ServiceAreaManagerName, inputModel.ServiceArea.ServiceAreaManager},
                {EmailKeys.EscalationEmail.DateResponseWasDue, inputModel.Issue.DueDate},
                {EmailKeys.EscalationEmail.IssueType, inputModel.Issue.IssueName},
                {EmailKeys.EscalationEmail.Location, inputModel.Issue.Location.Name},
                {EmailKeys.EscalationEmail.IssueNotes, inputModel.Issue.Notes},
                {EmailKeys.EscalationEmail.ReferenceNumber, inputModel.Issue.TicketNumber},
                {EmailKeys.EscalationEmail.TraName, tra.Name},
                {EmailKeys.EscalationEmail.HousingOfficerName, inputModel.Issue.HousingOfficerName},
                {EmailKeys.EscalationEmail.Subject,$"Escalation of ETRA action enquiry: {tra.Name} ETRA meeting on[{inputModel.Issue.CreatedOn}] Action enquiry - Ref[{inputModel.Issue.TicketNumber}]" }
            };

            var outputModel = new SendEscalationEmailOutputModel();

            if (!string.IsNullOrEmpty(inputModel?.ServiceArea.ServiceAreaManagerEmail))
            { 
                await _notificationClient.SendEmailAsync(inputModel?.ServiceArea.ServiceAreaManagerEmail, _config?.Value.EscalationTemplateId, personalization).ConfigureAwait(false);
                outputModel.SentToServiceAreaOfficer = true;
            }

            if (!string.IsNullOrEmpty(inputModel?.ServiceArea.ServiceAreaOfficerEmail))
            {
                await _notificationClient.SendEmailAsync(inputModel?.ServiceArea.ServiceAreaOfficerEmail, _config?.Value.AHMEscalationTemplate, personalization).ConfigureAwait(false);
                outputModel.SentToServiceAreaManager = true;
            }

            if (!string.IsNullOrEmpty(inputModel?.AreaManagerDetails.Email))
            {
                await _notificationClient.SendEmailAsync(inputModel?.AreaManagerDetails.Email, _config?.Value.AHMEscalationTemplate, personalization).ConfigureAwait(false);
                outputModel.SentToServiceAreaManager = true;
            }

            return outputModel;
        }
    }
}