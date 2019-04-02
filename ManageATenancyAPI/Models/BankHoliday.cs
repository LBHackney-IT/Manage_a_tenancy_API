using System;

namespace ManageATenancyAPI.Models
{
    public class BankHoliday
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public bool Bunting { get; set; }
    }
}