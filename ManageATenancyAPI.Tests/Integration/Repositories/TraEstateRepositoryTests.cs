using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Repository;
using Microsoft.Extensions.Options;
using Xunit;

namespace ManageATenancyAPI.Tests.Integration.Repositories
{
    public class TraEstateRepositoryTests
    {
        [Fact]
        public async Task GetEstatesByTraId_ListEstates()
        {
            var options = new OptionsWrapper<ConnStringConfiguration>(new ConnStringConfiguration() { ManageATenancyDatabase = "Server=lbhsqld01.ad.hackney.gov.uk;Database=ManageATenancy_DEV;User Id=tmprocess; Password=Hackney123;" });
            var traEstateRepository = new TraEstatesRepository(options);
            var result = traEstateRepository.GetEstatesByTraId(1);


            var estateList = result.Select(x => x.EstateUHRef).ToList();
            Assert.Contains(estateList, x => x == "00078569");
        }

        [Fact]
        public async Task GetAllUsedEstateRefsTest_ReturnsList_OnlyUsed()
        {
            var options = new OptionsWrapper<ConnStringConfiguration>(new ConnStringConfiguration() { ManageATenancyDatabase = "Server=lbhsqld01.ad.hackney.gov.uk;Database=ManageATenancy_DEV;User Id=tmprocess; Password=Hackney123;" });
            var traEstateRepository = new TraEstatesRepository(options);
            var estatesWithTra = traEstateRepository.GetAllUsedEstateRefs();
            var traEstates = traEstateRepository.GetEstatesByTraId(1);

            Assert.Contains(estatesWithTra, x => x == traEstates.First().EstateUHRef);
        }

        [Fact]
        public async Task AreUnusedEstates_EstateUsed_ReturnsFalse()
        {
            var options = new OptionsWrapper<ConnStringConfiguration>(new ConnStringConfiguration() { ManageATenancyDatabase = "Server=lbhsqld01.ad.hackney.gov.uk;Database=ManageATenancy_DEV;User Id=tmprocess; Password=Hackney123;" });
            var traEstateRepository = new TraEstatesRepository(options);
            var traEstates = traEstateRepository.GetEstatesByTraId(1);
            var estatesWithTra = traEstateRepository.AreUnusedEstates(new List<string>() { { traEstates.First().EstateName } });
            Assert.False(estatesWithTra);
        }

        [Fact]
        public async Task AddEstateToTraThenRemove_EstateAddedEstateRemoved()
        {
            var options = new OptionsWrapper<ConnStringConfiguration>(new ConnStringConfiguration() { ManageATenancyDatabase = "Server=lbhsqld01.ad.hackney.gov.uk;Database=ManageATenancy_DEV;User Id=tmprocess; Password=Hackney123;", UHWReportingWarehouse = "Server=10.80.65.49;Database=StagedDB;User Id=reports; Password=reports" });
            var traEstateRepository = new TraEstatesRepository(options);
            var estateRepository = new EstateRepository(options);

            var traEstates = traEstateRepository.GetEstatesByTraId(1);
            var unaddedEstates = await estateRepository.GetEstatesNotInList(traEstates.Select(x => x.EstateUHRef).ToList());

            traEstateRepository.AddEstateToTra(1, unaddedEstates.First().EstateId, unaddedEstates.First().EstateName);
            var traEstatesOneAdded = traEstateRepository.GetEstatesByTraId(1);
            Assert.Equal(traEstates.Count + 1, traEstatesOneAdded.Count);

            traEstateRepository.RemoveEstateFromTra(1, unaddedEstates.First().EstateId);
            var traEstatesOneRemoved = traEstateRepository.GetEstatesByTraId(1);
            Assert.Equal(traEstatesOneRemoved.Count + 1, traEstatesOneAdded.Count);

        }
    }
}
