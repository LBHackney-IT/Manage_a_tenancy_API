using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Controllers.v2;
using ManageATenancyAPI.Gateways.GetAllTRAs;
using ManageATenancyAPI.Services.JWT;
using ManageATenancyAPI.UseCases.TRA.GetAllTRAs;
using Xunit;

namespace ManageATenancyAPI.Tests.v2.AcceptanceTests.GetAllTRAs
{
    public class GetAllTRAsAcceptanceTests
    {
        private TRAController _classUnderTest;
        private IGetAllTRAsUseCase _useCase;
        private IGetAllTRAsGateway _gateway;
        private IJWTService _jwtService;
        
        public GetAllTRAsAcceptanceTests()
        {
            _gateway = new GetAllTRAsGateway();
            _useCase = new GetAllTRAsUseCase(_gateway);
            _jwtService = new JWTService();
            _classUnderTest = new TRAController(_jwtService, _useCase, null);
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
