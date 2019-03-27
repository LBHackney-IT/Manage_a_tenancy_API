using ManageATenancyAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Interfaces
{
    public interface IDateService
    {
        Task<IEnumerable<BankHoliday>> GetEnglishBankHolidays(DateTime? from = null, DateTime? to = null);
        Task<DateTime> GetIssueResponseDueDate(DateTime issueCreatedDate, int weeksToAdd);
    }
}