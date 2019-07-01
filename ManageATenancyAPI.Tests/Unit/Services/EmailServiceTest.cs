using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Services.JWT;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.Unit.Services
{
    public class EmailServiceTest
    {
        private IEmailService _classUnderTest;
        private Mock<INotificationClient> _mockNotificationClient;
        private Mock<IJWTService> _mockJWTService;
        private IOptions<EmailConfiguration> _emailConfiguration;

        public EmailServiceTest()
        {
            _mockNotificationClient = new Mock<INotificationClient>();
            _mockJWTService = new Mock<IJWTService>();
            _emailConfiguration = Options.Create(new EmailConfiguration
            {
                ApiKey = "API KEY",
                FrontEndAppUrl = "https://www.front-end-app.com/",
                TemplateId = Guid.NewGuid().ToString()
            });
            _classUnderTest = new EmailService(_mockNotificationClient.Object, _mockJWTService.Object, _emailConfiguration);
        }

        [Theory]
        [InlineData("123","test@t.com", "tra name", "officer of Hackney", "123 hackney road")]
        [InlineData("456","test2@t.com", "tra name 2", "officer of Hackney 2", "123 hackney road 2")]
        public async Task calls_notification_client(string templateId, string emailAdress, string traName, string officerName, string officerAddress)
        {
            //arrange
            var inputModel = new SendTraConfirmationEmailInputModel
            {
                EmailAddress = emailAdress,
                TraName = traName,
                OfficerName = officerName,
                OfficerAddress = officerAddress,
                MeetingId = Guid.NewGuid()
            };

            var token = Guid.NewGuid().ToString();

            _mockJWTService.Setup(s =>
                s.CreateManageATenancySingleMeetingToken(It.Is<Guid>(m => m == inputModel.MeetingId),
                    It.IsAny<string>())).Returns(token);

            var personalization = new Dictionary<string, object>
            {
                {EmailKeys.EmailAddress,emailAdress},
                {EmailKeys.Subject, $"{traName} meeting notes confirmation" },
                {EmailKeys.MeetingUrl, $"{_emailConfiguration?.Value.FrontEndAppUrl}?meetingtoken={token}"},
                {EmailKeys.OfficerName, $"{officerName}"},
                {EmailKeys.OfficerAddress, $"{officerAddress}"}
            };

            //act
            var outputModel = await _classUnderTest.SendTraConfirmationEmailAsync(inputModel, CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockNotificationClient.Verify(s=> s.SendEmailAsync(
                It.Is<string>(m=> m.Equals(emailAdress)), 
                It.Is<string>(m=> m.Equals(_emailConfiguration.Value.TemplateId)),
                It.Is<Dictionary<string, object>>( m=> 
                    m[EmailKeys.EmailAddress].Equals(personalization[EmailKeys.EmailAddress]) &&
                    m[EmailKeys.Subject].Equals(personalization[EmailKeys.Subject]) &&
                    m[EmailKeys.MeetingUrl].Equals(personalization[EmailKeys.MeetingUrl]) &&
                    m[EmailKeys.OfficerName].Equals(personalization[EmailKeys.OfficerName]) &&
                    m[EmailKeys.OfficerAddress].Equals(personalization[EmailKeys.OfficerAddress])
                ),
                It.IsAny<string>(),
                It.IsAny<string>()
            ), Times.Once);
        }

        [Theory]
        [InlineData("123", "test@t.com", "tra name", "officer of Hackney", "123 hackney road")]
        [InlineData("456", "test2@t.com", "tra name 2", "officer of Hackney 2", "123 hackney road 2")]
        public async Task calls_jwt_service_create_token(string templateId, string emailAdress, string traName, string officerName, string officerAddress)
        {
            //arrange
            var inputModel = new SendTraConfirmationEmailInputModel
            {
                EmailAddress = emailAdress,
                TraName = traName,
                OfficerName = officerName,
                OfficerAddress = officerAddress,
                MeetingId = Guid.NewGuid()
            };
            //act
            var outputModel = await _classUnderTest.SendTraConfirmationEmailAsync(inputModel, CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockJWTService.Verify(s=> s.CreateManageATenancySingleMeetingToken(It.Is<Guid>(m=> m== inputModel.MeetingId), It.IsAny<string>()));
        }
    }
}