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
    [Collection("Database collection")]
    public class EstateRepositoryTests: BaseTest
    {

        [Fact]
        public async  Task GetEstates_OneResult_Populated()
        {
            var estateId = "ESTATEID0011";
            var options = new OptionsWrapper<ConnStringConfiguration>(GetConfiguration<ConnStringConfiguration>(Config, "ConnectionStrings"));
            var estateRepository = new EstateRepository(options);
            var result = await estateRepository.GetEstates(new List<string>() { estateId });
            Assert.Equal("ESTATEID0011", result.First().prop_ref);
            Assert.Equal("EstateName001", result.First().short_address.Trim());
         
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
            var estateId = "NOTINLIST";

            var options = new OptionsWrapper<ConnStringConfiguration>(GetConfiguration<ConnStringConfiguration>(Config, "ConnectionStrings"));
            var estateRepository = new EstateRepository(options);
            var result = await estateRepository.GetEstatesNotInList(new List<string>() { estateId });
            Assert.DoesNotContain(result, x =>x.major_ref== "NOTINLIST");
        }
    }
}
