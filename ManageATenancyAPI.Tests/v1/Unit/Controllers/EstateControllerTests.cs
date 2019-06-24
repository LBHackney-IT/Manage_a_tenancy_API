using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Controllers.Housing.NHO;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Repository;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.Controllers
{
    public class EstateControllerTests
    {
        [Fact]
        public async Task GetUnassigned_PopulatedCorrectly()
        {
            var config = new OptionsWrapper<ConnStringConfiguration>(new ConnStringConfiguration() { });

            var mockEstateAction = new Mock<IEstateAction>();
            var mockBlockAction = new Mock<IBlockAction>();
            var mockTraEstateAction = new Mock<ITraEstateAction>();
            var estateController = new EstateController(mockEstateAction.Object, mockBlockAction.Object, mockTraEstateAction.Object);


            var returnedBlock = new List<Block>()
            {
                new Block()
                {
                    BlockId = "TestBlockId",
                    BlockName = "TestBlockName",
                    EstateId = "TestEstateId",
                    EstateName = "TestEstateName"
                }
            };

            mockBlockAction.Setup(x => x.GetBlocksByEstateId(It.IsAny<string>()))
                .Returns(Task.FromResult(returnedBlock.AsEnumerable()));

            mockTraEstateAction.Setup(x => x.GetAllUsedEstateRefs())
                .Returns(new List<string>() { "00001", "00002" });

            mockEstateAction.Setup(x => x.GetEstatesNotInList(new List<string>() { "00001", "00002" }))
                .Returns(Task.FromResult(new List<Estate>()
                {
                    new Estate() {EstateId = "00001", EstateName = "EstateName1"},
                    new Estate() {EstateId = "00002", EstateName = "EstateName2"}
                }));


            var result = await estateController.GetUnassigned();
            Assert.Equal(result.Result.Count, 2);
        }

        [Fact]
        public async Task GetEstatesByTra_PopulatedCorrectly()
        {
            var config = new OptionsWrapper<ConnStringConfiguration>(new ConnStringConfiguration());

            var mockEstateAction = new Mock<IEstateAction>();
            var mockBlockAction = new Mock<IBlockAction>();
            var mockTraEstatesAction = new Mock<ITraEstateAction>();
            var estateController = new EstateController(mockEstateAction.Object, mockBlockAction.Object, mockTraEstatesAction.Object);


            var returnedBlock = new List<Block>()
            {
                new Block()
                {
                    BlockId = "TestBlockId",
                    BlockName = "TestBlockName",
                    EstateId = "TestEstateId",
                    EstateName = "TestEstateName"
                }
            };

            mockBlockAction.Setup(x => x.GetBlocksByEstateId(It.IsAny<string>()))
                .Returns(Task.FromResult(returnedBlock.AsEnumerable()));

            mockTraEstatesAction.Setup(x => x.GetEstatesByTraId(5))
                .Returns(new List<TraEstate>() { new TraEstate() { EstateName = "EstateName1", EstateUHRef = "00001", TRAId = 5 }, new TraEstate() { EstateName = "EstateName2", EstateUHRef = "00002", TRAId = 5 } });

            mockEstateAction.Setup(x => x.GetEstates(new List<string>() { "00001", "00002" }))
                .Returns(Task.FromResult(new List<Estate>()
                {
                    new Estate() {EstateId = "00001", EstateName = "EstateName1"},
                    new Estate() {EstateId = "00002", EstateName = "EstateName2"}
                }));


            var result = await estateController.GetEstatesByTra(5);
            Assert.Equal(result.Result.Count, 2);
            Assert.Equal(result.Result.First().Blocks.Count(), 1);
        }

        [Fact]
        public async Task GetEstatesByTra_NoResults_ReturnsEmpty()
        {
            var config = new OptionsWrapper<ConnStringConfiguration>(new ConnStringConfiguration());

            var mockEstateAction = new Mock<IEstateAction>();
            var mockBlockAction = new Mock<IBlockAction>();
            var mockTraEstatesAction = new Mock<ITraEstateAction>();
            var estateController = new EstateController(mockEstateAction.Object, mockBlockAction.Object, mockTraEstatesAction.Object);


            var returnedBlock = new List<Block>()
            {
                new Block()
                {
                    BlockId = "TestBlockId",
                    BlockName = "TestBlockName",
                    EstateId = "TestEstateId",
                    EstateName = "TestEstateName"
                }
            };

            mockBlockAction.Setup(x => x.GetBlocksByEstateId(It.IsAny<string>()))
                .Returns(Task.FromResult(returnedBlock.AsEnumerable()));

            mockTraEstatesAction.Setup(x => x.GetEstatesByTraId(5))
                .Returns(new List<TraEstate>());

            mockEstateAction.Setup(x => x.GetEstates(new List<string>() { "00001", "00002" }))
                .Returns(Task.FromResult(new List<Estate>()));


            var result = await estateController.GetEstatesByTra(5);
            Assert.Equal(0, result.Result.Count);
        }

    }
}
