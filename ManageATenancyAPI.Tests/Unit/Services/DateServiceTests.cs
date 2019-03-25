using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Services;
using Microsoft.Extensions.Options;
using Moq;
using MyPropertyAccountAPI.Configuration;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ManageATenancyAPI.Tests.Unit.Services
{
    public class DateServiceTests
    {
        private readonly Mock<IOptions<URLConfiguration>> _mockConfig;
        private readonly Mock<IHackneyHousingAPICall> _mockApiCall;
        private readonly DateService _service;

        public DateServiceTests()
        {
            _mockConfig = new Mock<IOptions<URLConfiguration>>();
            _mockConfig.SetupGet(x => x.Value).Returns(new URLConfiguration { BankHolidaysUrl = "http://localhost" });
            _mockApiCall = new Mock<IHackneyHousingAPICall>();
            _service = new DateService(_mockConfig.Object, _mockApiCall.Object);
        }

        [Fact]
        public async Task GetEnglishBankHolidays_NoFromOrToDate_ReturnsAllBankHolidays()
        {
            var json = GetBankHolidayResponse();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
            _mockApiCall.Setup(x => x.getHousingAPIResponse(It.IsAny<HttpClient>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(responseMessage);

            var result = await _service.GetEnglishBankHolidays();

            Assert.True(result.Count() == 8);
        }

        private static string GetBankHolidayResponse()
        {
            return @"{
                'england-and-wales': {
                    'division': 'england-and-wales',
                    'events': [
                        {
                            title: 'New Year’s Day',
                            date: '2020-01-01',
                            notes: '',
                            bunting: true
                        },
                        {
                            title: 'Good Friday',
                            date: '2020-04-10',
                            notes: '',
                            bunting: false
                        },
                        {
                            title: 'Easter Monday',
                            date: '2020-04-13',
                            notes: '',
                            bunting: true
                        },
                        {
                            title: 'Early May bank holiday',
                            date: '2020-05-04',
                            notes: '',
                            bunting: true
                        },
                        {
                            title: 'Spring bank holiday',
                            date: '2020-05-25',
                            notes: '',
                            bunting: true
                        },
                        {
                            title: 'Summer bank holiday',
                            date: '2020-08-31',
                            notes: '',
                            bunting: true
                        },
                        {
                            title: 'Christmas Day',
                            date: '2020-12-25',
                            notes: '',
                            bunting: true
                        },
                        {
                            title: 'Boxing Day',
                            date: '2020-12-28',
                            notes: 'Substitute day',
                            bunting: true
                        }
                    ]
                }
            }";
        }
    }
}