using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Gateways.EscalateIssue;
using ManageATenancyAPI.Gateways.GetTraIssuesThatNeedEscalating;
using ManageATenancyAPI.Gateways.SendEscalationEmailGateway;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.v2.UseCases.EscalateIssues
{
    public class EscalateIssuesUseCaseTests
    {
        private IEscalateIssuesUseCase _classUnderTest;
        private Mock<IGetTraIssuesThatNeedEscalatingGateway> _mockGetTraIssuesThatNeedEscalatingGateway;
        private Mock<IEscalateIssueGateway> _mockEscalateIssueGateway;
        private Mock<ISendEscalationEmailGateway> _mockEmailService;
        private TRAIssue _traIssue;

        public EscalateIssuesUseCaseTests()
        {
            _mockGetTraIssuesThatNeedEscalatingGateway = new Mock<IGetTraIssuesThatNeedEscalatingGateway>();
            _mockEmailService = new Mock<ISendEscalationEmailGateway>();
            _mockEscalateIssueGateway = new Mock<IEscalateIssueGateway>();

            _classUnderTest = new EscalateIssuesUseCase(_mockGetTraIssuesThatNeedEscalatingGateway.Object, _mockEscalateIssueGateway.Object,  _mockEmailService.Object);
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

        [Theory]
        [InlineData(1, "test meeting")]
        [InlineData(2, "test meeting 2")]
        public async Task calls_escalate_issue_gateway(int traId, string meetingName)
        {
            //arrange
            _traIssue = new TRAIssue
            {
                Id = Guid.NewGuid()
            };
            _mockGetTraIssuesThatNeedEscalatingGateway
                .Setup(s => s.GetTraIssuesThatNeedEscalating(It.IsAny<CancellationToken>())).ReturnsAsync(
                    new List<TRAIssue>
                    {
                        _traIssue
                    });
            //act
            await _classUnderTest.ExecuteAsync(CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockEscalateIssueGateway.Verify(s => s.EscalateIssueAsync(It.Is<EscalateIssueInputModel>(m=> m.Issue.Id == _traIssue.Id),It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("chair", "Jeff JohnJeff")]
        [InlineData("secretary", "Jeff NotJohnJeff")]
        public async Task calls_email_service(string role, string name)
        {
            //arrange
            //act
            await _classUnderTest.ExecuteAsync(CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockEmailService.Verify(s => s.SendEscalationEmailAsync(It.IsAny<SendEscalationEmailInputModel>(), It.IsAny<CancellationToken>()));
        }
    }
}