using System;
using ManageATenancyAPI.Database.DTO;
using ManageATenancyAPI.Database.Models;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Repository.Interfaces;
using ManageATenancyAPI.Services;
using ManageATenancyAPI.Services.Interfaces;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.Unit.Services
{
    public class NewTenancyServiceTests
    {
        private Mock<ITenancyRepository> _tenancyRepository;
        private Mock<IClock> _clock;
        private INewTenancyService _service;
        
        public NewTenancyServiceTests()
        {
            _tenancyRepository = new Mock<ITenancyRepository>();
            _clock = new Mock<IClock>();
            _clock.Setup(m => m.UtcNow).Returns(DateTime.Now);
            _service = new NewTenancyService(_tenancyRepository.Object, _clock.Object);
        }

        [Fact]
        public void GetLastRetrieved_CalledGetLastRun_FromRepository()
        {
            _tenancyRepository.Setup(m => m.GetLastRun()).Verifiable();
            
            _service.GetLastRetrieved();
            
            _tenancyRepository.Verify(m => m.GetLastRun(), Times.Once);
        }
        
        [Fact]
        public void GetLastRetrieved_ReturnsDate_FromRepository()
        {
            var expected = new DateTime(2019, 06, 19);
            _tenancyRepository.Setup(m => m.GetLastRun()).Returns(new NewTenancyLastRunDto(expected));
            
            var result = _service.GetLastRetrieved();
            
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetLastRetrieved_NoValueAvailable_ReturnsPreviousDay()
        {
            var today = new DateTime(2015, 10, 5, 12, 30, 25);
            var yesterday = new DateTime(2015, 10, 4, 12, 30, 25);

            _clock.Setup(m => m.UtcNow).Returns(today);
            
            var result = _service.GetLastRetrieved();
            
            Assert.Equal(yesterday, result);
        }

        [Fact]
        public void UpdateLastRetrieved_CallsRepository()
        {
            _tenancyRepository.Setup(m => m.UpdateLastRun(It.IsAny<DateTime>())).Verifiable();
            
            _service.UpdateLastRetrieved(new DateTime());
            
            _tenancyRepository.Verify(m => m.UpdateLastRun(It.IsAny<DateTime>()), Times.Once);
        }
        
        [Fact]
        public void UpdateLastRetrieved_UpdatesRepository_WithCorrectValue()
        {
            var expected = new DateTime(2019, 06, 10);
            _tenancyRepository.Setup(m => m.UpdateLastRun(It.IsAny<DateTime>())).Verifiable();
            
            _service.UpdateLastRetrieved(expected);
            
            _tenancyRepository.Verify(m => m.UpdateLastRun(expected), Times.Once);
        }
    }
}