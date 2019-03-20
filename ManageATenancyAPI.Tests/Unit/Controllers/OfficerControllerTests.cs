using ManageATenancyAPI.Controllers.Housing.NHO;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ManageATenancyAPI.Tests.Unit.Controllers
{
    public class OfficerControllerTests
    {
        private readonly Mock<IOfficerService> _mockOfficerService;
        private readonly OfficerController _controller;

        public OfficerControllerTests()
        {
            _mockOfficerService = new Mock<IOfficerService>();
            _controller = new OfficerController(_mockOfficerService.Object);
        }

        [Fact]
        public async Task GetNewTenanciesForHousingOfficer_EmptyStringEmailAddress_ReturnsBadRequest()
        {
            var actionResult = await _controller.GetNewTenanciesForHousingOfficer(string.Empty);

            Assert.IsType<BadRequestResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetNewTenanciesForHousingOfficer_NullEmailAddress_ReturnsBadRequest()
        {
            var actionResult = await _controller.GetNewTenanciesForHousingOfficer(null);

            Assert.IsType<BadRequestResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetNewTenanciesForHousingOfficer_NonExistingEmailAddress_ReturnsNotFound()
        {
            _mockOfficerService.Setup(x => x.GetOfficerDetails(It.IsAny<string>())).ReturnsAsync((OfficerDetails)null);

            var actionResult = await _controller.GetNewTenanciesForHousingOfficer("test@hackney.gov.uk");
                       
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetNewTenanciesForHousingOfficer_ExistingEmailAddress_ReturnsResultsList()
        {
            var officer = OfficerDetails.Create(new Dictionary<string, object>
            {
                { "hackney_estateofficerid", Guid.NewGuid().ToString() },
                { "hackney_emailaddress", "testy.testerson@hackney.gov.uk" },
                { "hackney_name", "Testy Testerson" },
                { "hackney_firstname", "Testy" },
                { "hackney_lastname", "Testerson" },
                { "hackney_lastnewtenancycheckdate", DateTime.Now }
            });
            _mockOfficerService.Setup(x => x.GetOfficerDetails(It.IsAny<string>())).ReturnsAsync(officer);
            _mockOfficerService.Setup(x => x.GetNewTenanciesForHousingOfficer(It.IsAny<OfficerDetails>())).ReturnsAsync(new List<NewTenancyResponse>());

            var actionResult = await _controller.GetNewTenanciesForHousingOfficer("testy.testerson@hackney.gov.uk");
                       
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var objectResult = actionResult.Result as OkObjectResult;

            Assert.IsType<HackneyResult<IList<NewTenancyResponse>>>(objectResult.Value);
        }
    }
}