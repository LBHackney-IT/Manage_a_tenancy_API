using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Repository;
using Microsoft.Extensions.Options;
using Xunit;

namespace ManageATenancyAPI.Tests.Integration.Repositories
{
    public class EstateRepositoryTests: BaseTest
    {

        [Fact]
        public async  Task GetEstates_OneResult_Populated()
        {
            var estateId = "00078614";
            var options = new OptionsWrapper<ConnStringConfiguration>(GetConfiguration<ConnStringConfiguration>(Config, "ConnectionStrings"));
            var estateRepository = new EstateRepository(options);
            var result = await estateRepository.GetEstates(new List<string>() { estateId });
            Assert.Equal("00078614", result.First().EstateId);
            Assert.Equal("De Beauvoir Road  De Beauvoir Estate", result.First().EstateName);
         
        }

        [Fact]
        public async Task GetEstates_NoResult_EmptyList()
        {
            var estateId = "Nothing";
            var options = new OptionsWrapper<ConnStringConfiguration>(GetConfiguration<ConnStringConfiguration>(Config, "ConnectionStrings"));
            var estateRepository = new EstateRepository(options);
            var result = await estateRepository.GetEstates(new List<string>() { estateId });
            Assert.Equal(0, result.Count);
        }

        [Fact]
        public async Task GetEstatesNotInList()
        {
            var estateId = "00078614";

            var options = new OptionsWrapper<ConnStringConfiguration>(GetConfiguration<ConnStringConfiguration>(Config, "ConnectionStrings"));
            var estateRepository = new EstateRepository(options);
            var result = await estateRepository.GetEstatesNotInList(new List<string>() { estateId });
            Assert.DoesNotContain(result, x =>x.EstateId== "00078614");
        }
    }
}
