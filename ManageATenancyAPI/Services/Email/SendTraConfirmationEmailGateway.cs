using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Services.JWT;
using Microsoft.Extensions.Options;


namespace ManageATenancyAPI.Tests.Unit.Services
{
    public class SendTraConfirmationEmailGateway:ISendTraConfirmationEmailGateway
    {
        private INotificationClient _client;
        private readonly IJWTService _jwtService;
        private readonly IOptions<EmailConfiguration> _config;

        public SendTraConfirmationEmailGateway(INotificationClient client, IJWTService jwtService, IOptions<EmailConfiguration> config)
        {
            _client = client;
            _jwtService = jwtService;
            _config = config;
        }

        public async Task<SendTraConfirmationEmailOutputModel> SendTraConfirmationEmailAsync(SendTraConfirmationEmailInputModel inputModel, CancellationToken cancellationToken)
        {
            var token = _jwtService.CreateManageATenancySingleMeetingToken(inputModel.MeetingId,
                Environment.GetEnvironmentVariable("HmacSecret"));

            var personalization = new Dictionary<string, object>
            {
                {EmailKeys.EmailAddress,inputModel?.EmailAddress},
                {EmailKeys.Subject, $"{inputModel.TraName} meeting notes confirmation" },
                {EmailKeys.MeetingUrl, $"{_config?.Value.FrontEndAppUrl}?meetingtoken={token}"},
                {EmailKeys.OfficerName, $"{inputModel.OfficerName}"},
                {EmailKeys.OfficerAddress, $"{inputModel.OfficerAddress}"}
            };

            await _client.SendEmailAsync(inputModel.EmailAddress, _config?.Value.TemplateId, personalization)
                .ConfigureAwait(false);

            return new SendTraConfirmationEmailOutputModel
            {
                IsSent = true,
                MeetingUrl = personalization[EmailKeys.MeetingUrl].ToString()
            };
        }
    }
}
