using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Gateways.EscalateIssue;
using ManageATenancyAPI.Gateways.GetTraIssuesThatNeedEscalating;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting;
using ManageATenancyAPI.Gateways.SendEscalationEmailGateway;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.v2.UseCases.EscalateIssues
{
    public class EscalateIssuesUseCaseTests
    {
        private IEscalateIssuesUseCase _classUnderTest;
        private Mock<IGetTraIssuesThatNeedEscalatingGateway> _mockGetTraIssuesThatNeedEscalatingGateway;
        private Mock<IEscalateIssueGateway> _mockEscalateIssueGateway;
        private Mock<IGetWorkingDaysGateway> _mockGetWorkingDaysGateway;
        private Mock<ISendEscalationEmailGateway> _mockEmailService;
        private Mock<IJsonPersistanceService> _mockJsonPersistanceService;
        private Mock<IGetAreaManagerInformationGateway> _mockGetAreaManagerDetailsGateway;
        private TRAIssue _traIssue;

        public EscalateIssuesUseCaseTests()
        {
            _mockGetTraIssuesThatNeedEscalatingGateway = new Mock<IGetTraIssuesThatNeedEscalatingGateway>();
            _mockEscalateIssueGateway = new Mock<IEscalateIssueGateway>();
            _mockGetWorkingDaysGateway = new Mock<IGetWorkingDaysGateway>();
            _mockEmailService = new Mock<ISendEscalationEmailGateway>();
            _mockJsonPersistanceService = new Mock<IJsonPersistanceService>();


            _classUnderTest = new EscalateIssuesUseCase(
                _mockGetTraIssuesThatNeedEscalatingGateway.Object,
                _mockEscalateIssueGateway.Object,
                _mockGetWorkingDaysGateway.Object,
                _mockEmailService.Object,
                _mockJsonPersistanceService.Object
              );
        }

        [Theory]
        [InlineData(1, "test meeting")]
        [InlineData(2, "test meeting 2")]
        public async Task calls_get_issues_that_need_escalating_gateway(int traId, string meetingName)
        {
            //arrange
            //act
            await _classUnderTest.ExecuteAsync(CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockGetTraIssuesThatNeedEscalatingGateway.Verify(s => s.GetTraIssuesThatNeedEscalating(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task returns_no_issues_found_to_escalate()
        {
            //arrange
            //act
            var outputModel = await _classUnderTest.ExecuteAsync(CancellationToken.None).ConfigureAwait(false);
            //assert
            outputModel.IssuesToEscalate.Should().BeNull();
        }

        [Fact]
        public async Task calls_escalate_issue_gateway_twice()
        {
            //arrange
            _mockEscalateIssueGateway.Setup(s =>
                s.EscalateIssueAsync(It.IsAny<EscalateIssueInputModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                new EscalateIssueOutputModel
                {
                    Successful = true
                });
            var meetingIssueOutputModel = new EscalateMeetingIssueInputModel
            {
                Id = Guid.NewGuid(),
                IssueType=new IssueType
                {
                    IssueId ="12345"
                },
                AreaId="1"
               
            };

            _mockGetTraIssuesThatNeedEscalatingGateway
                .Setup(s => s.GetTraIssuesThatNeedEscalating(It.IsAny<CancellationToken>())).ReturnsAsync(
                    new List<EscalateMeetingIssueInputModel>
                    {
                        meetingIssueOutputModel,
                        meetingIssueOutputModel,
                    });

            _mockJsonPersistanceService.Setup(s => s.DeserializeStream<List<AreaManagerDetails>>(It.IsAny<string>())).ReturnsAsync(
                 new List<AreaManagerDetails>
                    {
                        new AreaManagerDetails
                        {
                            AreaId=1,
                            Email="test@ho.com",
                            Name="Test Area Officer"
                        }
                    });

         _mockJsonPersistanceService.Setup(s => s.DeserializeStream<List<TRAIssueServiceArea>>(It.IsAny<string>())).ReturnsAsync(
               new List<TRAIssueServiceArea>
                  {
                        new TRAIssueServiceArea
                        {
                            IssueId="12345",
                            IssueType="",
                            ServiceAreaManager="",
                            ServiceAreaManagerEmail="",
                            ServiceAreaOfficerEmail =""
                        }
                  });
            _mockEmailService.Setup(x => x.SendEscalationEmailAsync(It.IsAny<SendEscalationEmailInputModel>(), CancellationToken.None)).ReturnsAsync(new SendEscalationEmailOutputModel
            {
                 AreaHousingManager=true,
                  SentToServiceAreaManager=true,
                   SentToServiceAreaOfficer=true,
                    Successful=true
            });
            //act
            await _classUnderTest.ExecuteAsync(CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockEscalateIssueGateway.Verify(s => s.EscalateIssueAsync(It.Is<EscalateIssueInputModel>(m => m.Issue.Id == meetingIssueOutputModel.Id), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task calls_email_service_twice()
        {
            //arrange
            var getEtraMeetingOutputModel = new EscalateMeetingIssueInputModel
            {
                Id = Guid.NewGuid(),
                IssueType = new IssueType
                {
                    IssueId = "12345"
                },
                AreaId = "1"

            };

            _mockEscalateIssueGateway.Setup(s =>
                s.EscalateIssueAsync(It.IsAny<EscalateIssueInputModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                new EscalateIssueOutputModel
                {
                    Successful = true
                });
            _mockGetTraIssuesThatNeedEscalatingGateway
                .Setup(s => s.GetTraIssuesThatNeedEscalating(It.IsAny<CancellationToken>())).ReturnsAsync(
                    new List<EscalateMeetingIssueInputModel>
                    {
                        getEtraMeetingOutputModel,
                        getEtraMeetingOutputModel
                    });

            _mockJsonPersistanceService.Setup(s => s.DeserializeStream<List<AreaManagerDetails>>(It.IsAny<string>())).ReturnsAsync(
               new List<AreaManagerDetails>
                  {
                        new AreaManagerDetails
                        {
                            AreaId=1,
                            Email="test@ho.com",
                            Name="Test Area Officer"
                        }
                  });
            _mockJsonPersistanceService.Setup(s => s.DeserializeStream<List<TRAIssueServiceArea>>(It.IsAny<string>())).ReturnsAsync(
                  new List<TRAIssueServiceArea>
                     {
                        new TRAIssueServiceArea
                        {
                            IssueId="12345",
                            IssueType="",
                            ServiceAreaManager="",
                            ServiceAreaManagerEmail="",
                            ServiceAreaOfficerEmail =""
                        }
                     });

            _mockEmailService.Setup(x => x.SendEscalationEmailAsync(It.IsAny<SendEscalationEmailInputModel>(), CancellationToken.None)).ReturnsAsync(new SendEscalationEmailOutputModel
            {
                AreaHousingManager = true,
                SentToServiceAreaManager = true,
                SentToServiceAreaOfficer = true,
                Successful = true
            });
            //act
            await _classUnderTest.ExecuteAsync(CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockEmailService.Verify(s => s.SendEscalationEmailAsync(It.IsAny<SendEscalationEmailInputModel>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}