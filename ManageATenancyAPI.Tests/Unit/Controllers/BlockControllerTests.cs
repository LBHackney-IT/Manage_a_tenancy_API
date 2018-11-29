using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManageATenancyAPI.Controllers.Housing.NHO;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Repository;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.Controllers
{
    public class BlockControllerTests
    {
        [Fact]
        public async Task ReturnEmptyArrayIfNoResults()
        {
            var mockBlockRepo = new Mock<IBlockRepository>();
            var blockController = new BlockController(mockBlockRepo.Object);

            var returned = new List<Block>();
            mockBlockRepo.Setup(x => x.GetBlocksByEstateId(It.IsAny<string>()))
                .Returns(Task.FromResult(returned.AsEnumerable()));

            var result = await blockController.GetBlocksByEstate("Test123");
            Assert.Empty(result.Result);
        }

        [Fact]
        public async Task ReturnCorrectResults()
        {
            var mockBlockRepo = new Mock<IBlockRepository>();
            var blockController = new BlockController(mockBlockRepo.Object);

            var returned = new List<Block>()
            {
                new Block()
                {
                    BlockId = "TestBlockId",
                    BlockName = "TestBlockName",
                    EstateId = "TestEstateId",
                    EstateName = "TestEstateName"
                }
            };
            mockBlockRepo.Setup(x => x.GetBlocksByEstateId(It.IsAny<string>()))
                .Returns(Task.FromResult(returned.AsEnumerable()));

            var result = await blockController.GetBlocksByEstate("Test123");

            var expected = HackneyResult<IEnumerable<Block>>.Create(returned);
            Assert.Equal(expected.Result.First().EstateId, result.Result.First().EstateId);
            Assert.Equal(expected.Result.First().BlockId, result.Result.First().BlockId);
            Assert.Equal(expected.Result.First().BlockName, result.Result.First().BlockName);
            Assert.Equal(expected.Result.First().EstateName, result.Result.First().EstateName);
        }
    }
}
