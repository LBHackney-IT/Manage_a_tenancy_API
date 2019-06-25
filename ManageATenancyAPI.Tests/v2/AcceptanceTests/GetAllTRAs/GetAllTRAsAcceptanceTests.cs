using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Controllers.v2;
using ManageATenancyAPI.Gateways.GetAllTRAs;
using ManageATenancyAPI.UseCases.TRA.GetAllTRAs;
using Xunit;

namespace ManageATenancyAPI.Tests.v2.AcceptanceTests.GetAllTRAs
{
    public class GetAllTRAsAcceptanceTests
    {
        private TRAController _classUnderTest;
        private IGetAllTRAsUseCase _useCase;
        private IGetAllTRAsGateway _gateway;
        
        public GetAllTRAsAcceptanceTests()
        {
            _gateway = new GetAllTRAsGateway();
            _useCase = new GetAllTRAsUseCase(_gateway);
            _classUnderTest = new TRAController(_useCase, null);
        }

        [Fact]
        public async Task can_get_all_tras()
        {
            //arrange

            //act
            var outputModel = await _classUnderTest.Get();

            //assert
            outputModel.Should().NotBeNull();
            outputModel.TRAs.Should().NotBeNullOrEmpty();
        }
    }
}
