using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using Microsoft.Extensions.Options;
using MyPropertyAccountAPI.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Services
{
    public class DateService : IDateService
    {
        private readonly IHackneyHousingAPICall _apiCall;
        private readonly HttpClient _client;
        private KeyValuePair<DateTime, IEnumerable<BankHoliday>> _bankHolidayCache;

        public DateService(IOptions<URLConfiguration> config, IHackneyHousingAPICall apiCall)
        {
            _apiCall = apiCall;
            var configVal = config.Value;

            _client = new HttpClient
            {
                BaseAddress = new Uri(configVal.BankHolidaysUrl)
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _bankHolidayCache = new KeyValuePair<DateTime, IEnumerable<BankHoliday>>();
        }

        public async Task<IEnumerable<BankHoliday>> GetEnglishBankHolidays(DateTime? from = null, DateTime? to = null)
        {
            IEnumerable<BankHoliday> bankHolidays;
            
            //if the cached value is less than a day old use it
            if (_bankHolidayCache.Key > DateTime.Now.AddDays(-1))
                bankHolidays = _bankHolidayCache.Value;
            else
            {
                //otherwise call the bank holiday api and cache the result
                var response = await _apiCall.getHousingAPIResponse(_client, "", "");
                var responseObject = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
                bankHolidays = responseObject["england-and-wales"]["events"].ToObject<IEnumerable<BankHoliday>>();
                _bankHolidayCache = new KeyValuePair<DateTime, IEnumerable<BankHoliday>>(DateTime.Now, bankHolidays);
            }

            if (from.HasValue)
                bankHolidays = bankHolidays.Where(x => x.Date >= from.Value);
            if (to.HasValue)
                bankHolidays = bankHolidays.Where(x => x.Date <= to.Value);

            return bankHolidays;
        }

        public async Task<DateTime> GetIssueResponseDueDate(DateTime issueCreatedDate, int weeksToAdd)
        {
            var bankHolidays = await GetEnglishBankHolidays(issueCreatedDate);
            var filteredBankHolidays = bankHolidays.Select(x => x.Date).ToList();
            var dueDate = issueCreatedDate;
            var daysToAdd = weeksToAdd * 5;

            while (daysToAdd > 0)
            {
                dueDate = dueDate.AddDays(1);

                if (dueDate.DayOfWeek != DayOfWeek.Saturday &&
                    dueDate.DayOfWeek != DayOfWeek.Sunday &&
                    !filteredBankHolidays.Contains(dueDate))
                    daysToAdd--;
            }
            return dueDate.Date;
        }
    }
}