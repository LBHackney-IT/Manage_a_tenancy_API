using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Formatters
{
    public static class DateTimeFormatter
    {
        public static string FormatDateTimeToUtc(DateTime date)
        {
            return date.ToString("s")+"Z";
        }
    }
}
