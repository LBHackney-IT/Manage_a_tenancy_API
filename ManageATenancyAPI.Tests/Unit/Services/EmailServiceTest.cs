using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Services.Email;
using ManageATenancyAPI.Services.JWT;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.Unit.Services
{
    public class EmailServiceTest
    {
        private ISendTraConfirmationEmailGateway _classUnderTest;
        private Mock<INotificationClient> _mockNotificationClient;
        private Mock<IJWTService> _mockJWTService;
        private Mock<ITraAction> _mockTraAction;
        private IOptions<EmailConfiguration> _emailConfiguration;

        public EmailServiceTest()
        {
            _mockNotificationClient = new Mock<INotificationClient>();
            _mockJWTService = new Mock<IJWTService>();
            _mockTraAction = new Mock<ITraAction>();
            _emailConfiguration = Options.Create(new EmailConfiguration
            {
                ApiKey = "API KEY",
                FrontEndAppUrl = "https://www.front-end-app.com/",
                TemplateId = Guid.NewGuid().ToString()
            });
            _classUnderTest = new SendTraConfirmationEmailGateway(_mockNotificationClient.Object, _mockJWTService.Object, _emailConfiguration, _mockTraAction.Object);
        }

        [Theory]
        [InlineData(123, "officer of Hackney", "123 hackney road" ,"test@t.com", "Tra Name")]
        [InlineData(456, "officer of Hackney 2", "123 hackney road 2", "test@p.com", "Tra Name 2")]
        public async Task calls_notification_client(int traId, string officerName, string officerAddress, string email, string traName)
        {
            //arrange
            var inputModel = new SendTraConfirmationEmailInputModel
            {
                TraId = traId,
                OfficerName = officerName,
                OfficerAddress = officerAddress,
                MeetingId = Guid.NewGuid()
            };

            var token = Guid.NewGuid().ToString();

            _mockJWTService.Setup(s =>
                s.CreateManageATenancySingleMeetingToken(
                    It.Is<Guid>(m => m == inputModel.MeetingId), 
                    It.Is<string>(m=> m.Equals(officerName)),
                    It.Is<int>(m=> m == traId),
                    It.IsAny<string>())).Returns(token);

            _mockTraAction.Setup(s =>
                s.GetAsync(It.Is<int>(m => m == inputModel.TraId))).ReturnsAsync(new TRA
            {
                Email = email,
                Name = traName
            });

            var personalization = new Dictionary<string, object>
            {
                {EmailKeys.EmailAddress,email},
                {EmailKeys.Subject, $"{traName} meeting notes confirmation" },
                {EmailKeys.MeetingUrl, $"{_emailConfiguration?.Value.FrontEndAppUrl}meeting/?existingMeeting=true#traToken={token}"},
                {EmailKeys.OfficerName, $"{officerName}"},
                {EmailKeys.OfficerAddress, $"{officerAddress}"}
            };

            //act
            var outputModel = await _classUnderTest.SendTraConfirmationEmailAsync(inputModel, CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockNotificationClient.Verify(s=> s.SendEmailAsync(
                It.Is<string>(m=> m.Equals(email)), 
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
                OfficerName = officerName,
                OfficerAddress = officerAddress,
                MeetingId = Guid.NewGuid()
            };
            //act
            var outputModel = await _classUnderTest.SendTraConfirmationEmailAsync(inputModel, CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockJWTService.Verify(s=> s.CreateManageATenancySingleMeetingToken(It.Is<Guid>(m=> m== inputModel.MeetingId), It.IsAny<string>(), It.IsAny<int>(),  It.IsAny<string>()));
        }
    }
}