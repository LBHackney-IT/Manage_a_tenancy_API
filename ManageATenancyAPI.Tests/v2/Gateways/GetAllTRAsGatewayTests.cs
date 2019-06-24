using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManageATenancyAPI.Database;
using ManageATenancyAPI.Gateways.GetAllTRAs;
using Xunit;

namespace ManageATenancyAPI.Tests.v2.Gateways
{
    public class GetAllTRAsGatewayTests
    {
        private IGetAllTRAsGateway _classUnderTest;
        private TenancyContext _tenancyContext;

        public GetAllTRAsGatewayTests()
        {
            
            
            _classUnderTest = new GetAllTRAsGateway();
        }

        [Fact]
        public async Task can_get_all_tras()
        {

        }
    }
}
