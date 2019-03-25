﻿using ManageATenancyAPI.Interfaces;
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

        public DateService(IOptions<URLConfiguration> config, IHackneyHousingAPICall apiCall)
        {
            var configVal = config.Value;

            _client = new HttpClient
            {
                BaseAddress = new Uri(configVal.BankHolidaysUrl)
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IEnumerable<BankHoliday>> GetEnglishBankHolidays(DateTime? from = null, DateTime? to = null)
        {
            //should cache this
            var response = await _apiCall.getHousingAPIResponse(_client, "", "");
            var responseObject = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
            var bankHolidays = responseObject["england-and-wales"]["events"].ToObject<IEnumerable<BankHoliday>>();

            if (from.HasValue)
                bankHolidays = bankHolidays.Where(x => x.Date >= from.Value);
            if (to.HasValue)
                bankHolidays = bankHolidays.Where(x => x.Date <= to.Value);

            return bankHolidays;
        }

        public async Task<DateTime> GetIssueDueDate(DateTime issueCreatedDate, int weeksToAdd)
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