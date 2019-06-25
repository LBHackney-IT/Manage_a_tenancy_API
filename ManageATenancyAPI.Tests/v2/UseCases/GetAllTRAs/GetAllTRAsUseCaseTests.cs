using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Database.Models;
using ManageATenancyAPI.Gateways.GetAllTRAs;
using ManageATenancyAPI.UseCases.TRA.GetAllTRAs;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.v2.UseCases.GetAllTRAs
{
    public class GetAllTRAsUseCaseTests
    {
        private IGetAllTRAsUseCase _classUnderTest;
        private Mock<IGetAllTRAsGateway> _mockGateway;
        public GetAllTRAsUseCaseTests()
        {
            _mockGateway = new Mock<IGetAllTRAsGateway>();
            _classUnderTest = new GetAllTRAsUseCase(_mockGateway.Object);
        }

        [Fact]
        public async Task calls_gateway()
        {
            //arrange
            _mockGateway.Setup(s => s.GetAllTRAsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<TRA>());
            //act
            var outputModel = await _classUnderTest.ExecuteAsync(CancellationToken.None).ConfigureAwait(false);
            //assert
            
            _mockGateway.Verify(s=> s.GetAllTRAsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task creates_output_model()
        {
            //arrange
            _mockGateway.Setup(s => s.GetAllTRAsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<TRA>());
            //act
            var outputModel = await _classUnderTest.ExecuteAsync(CancellationToken.None).ConfigureAwait(false);
            //assert

            outputModel.Should().NotBeNull();
        }

        [Fact]
        public async Task data_from_gateway_gets_returned()
        {
            //arrange
            _mockGateway.Setup(s => s.GetAllTRAsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<TRA>
            {
                new TRA()
            });
            //act
            var outputModel = await _classUnderTest.ExecuteAsync(CancellationToken.None).ConfigureAwait(false);
            //assert
            outputModel.TRAs.Should().NotBeNullOrEmpty();

        }
    }
}
