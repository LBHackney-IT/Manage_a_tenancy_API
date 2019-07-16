using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Services.JWT;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Services.Email
{
    public class SendTraConfirmationEmailGateway:ISendTraConfirmationEmailGateway
    {
        private INotificationClient _client;
        private readonly IJWTService _jwtService;
        private readonly IOptions<EmailConfiguration> _config;
        private readonly ITraAction _traAction;

        public SendTraConfirmationEmailGateway(INotificationClient client, IJWTService jwtService, IOptions<EmailConfiguration> config, ITraAction traAction)
        {
            _client = client;
            _jwtService = jwtService;
            _config = config;
            _traAction = traAction;
        }

        public async Task<SendTraConfirmationEmailOutputModel> SendTraConfirmationEmailAsync(SendTraConfirmationEmailInputModel inputModel, CancellationToken cancellationToken)
        {
            var token = _jwtService.CreateManageATenancySingleMeetingToken(inputModel.MeetingId, inputModel.OfficerName, inputModel.TraId,
                Environment.GetEnvironmentVariable("HmacSecret"));

            var tra = await _traAction.GetAsync(inputModel.TraId).ConfigureAwait(false);
            if(tra == null || tra.Email.IsNullOrEmpty())
                return new SendTraConfirmationEmailOutputModel {IsSent = false,};

            var personalization = new Dictionary<string, object>
            {
                {EmailKeys.EmailAddress, tra.Email},
                {EmailKeys.Subject, $"{tra.Name} meeting notes confirmation" },
                {EmailKeys.TraName, tra.Name },
                {EmailKeys.MeetingUrl, $"{_config?.Value.FrontEndAppUrl}meeting/?existingMeeting=true#traToken={token}"},
                {EmailKeys.OfficerName, $"{inputModel.OfficerName}"},
                {EmailKeys.OfficerAddress, $"{inputModel.OfficerAddress}"}
            };

            await _client.SendEmailAsync(tra.Email, _config?.Value.TemplateId, personalization)
                .ConfigureAwait(false);

            return new SendTraConfirmationEmailOutputModel
            {
                IsSent = true,
                MeetingUrl = personalization[EmailKeys.MeetingUrl].ToString()
            };
        }
    }
}
