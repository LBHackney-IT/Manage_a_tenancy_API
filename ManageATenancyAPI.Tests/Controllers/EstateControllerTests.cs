using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task GetEstatesByTra_PopulatedCorrectly()
        {
            var config = new OptionsWrapper<ConnStringConfiguration>(new ConnStringConfiguration(){ManageATenancyDatabase = "",UHWReportingWarehouse = ""});

            var mockEstateRepo = new Mock<IEstateRepository>();
            var mockBlockRepo = new Mock<IBlockRepository>();
            var mockTraEstatesRepo = new Mock<ITraEstatesRepository>();
            var estateController = new EstateController(mockEstateRepo.Object, mockBlockRepo.Object, mockTraEstatesRepo.Object);


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

            mockBlockRepo.Setup(x => x.GetBlocksByEstateId(It.IsAny<string>()))
                .Returns(Task.FromResult(returnedBlock.AsEnumerable()));

            mockTraEstatesRepo.Setup(x => x.GetEstatesByTraId(5))
                .Returns(new List<TraEstate>(){new TraEstate(){EstateName = "EstateName1",EstateUHRef = "00001",Id=1,TraId = 5}, new TraEstate() { EstateName = "EstateName2", EstateUHRef = "00002", Id = 2, TraId = 5 } });

            mockEstateRepo.Setup(x => x.GetEstates(new List<string>() {"00001", "00002"}))
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
            var config = new OptionsWrapper<ConnStringConfiguration>(new ConnStringConfiguration() { ManageATenancyDatabase = "", UHWReportingWarehouse = "" });

            var mockEstateRepo = new Mock<IEstateRepository>();
            var mockBlockRepo = new Mock<IBlockRepository>();
            var mockTraEstatesRepo = new Mock<ITraEstatesRepository>();
            var estateController = new EstateController(mockEstateRepo.Object, mockBlockRepo.Object, mockTraEstatesRepo.Object);


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

            //mockBlockRepo.Setup(x => x.GetBlocksByEstateId(It.IsAny<string>()))
            //    .Returns(Task.FromResult(returnedBlock.AsEnumerable()));

            mockTraEstatesRepo.Setup(x => x.GetEstatesByTraId(5))
                .Returns(new List<TraEstate>());

            mockEstateRepo.Setup(x => x.GetEstates(new List<string>() { "00001", "00002" }))
                .Returns(Task.FromResult(new List<Estate>()));


            var result = await estateController.GetEstatesByTra(5);
            Assert.Equal(result.Result.Count, 2);
            Assert.Equal(result.Result.First().Blocks.Count(), 1);
        }

    }
}
