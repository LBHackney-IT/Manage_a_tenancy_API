using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Controllers.v2;
using ManageATenancyAPI.Database.Models;
using ManageATenancyAPI.UseCases.TRA.GetAllTRAs;
using Xunit;
using Moq;

namespace ManageATenancyAPI.Tests.v2.Controllers
{
    public class TRAControllerTests
    {
        private readonly TRAController _classUnderTest;
        private readonly Mock<IGetAllTRAsUseCase> _mockUseCase;

        public TRAControllerTests()
        {
            _mockUseCase = new Mock<IGetAllTRAsUseCase>();
            _classUnderTest = new TRAController(_mockUseCase.Object, null);
        }

        [Fact]
        public async Task calls_use_case()
        {
            //arrange
            _mockUseCase.Setup(s => s.ExecuteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetAllTRAsOutputModel());
            //act
            var outputModel = await _classUnderTest.Get();

            //assert
            _mockUseCase.Verify(s=> s.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task can_get_all_tras()
        {
            //arrange
            _mockUseCase.Setup(s => s.ExecuteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetAllTRAsOutputModel
                {
                    TRAs = new List<TRA>()
                    {
                        new TRA()
                    }
                });
            //act
            var outputModel = await _classUnderTest.Get();

            //assert
            outputModel.Should().NotBeNull();
            outputModel.TRAs.Should().NotBeNullOrEmpty();
        }
    }
}
