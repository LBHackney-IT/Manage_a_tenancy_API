﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Formatters;
using ManageATenancyAPI.Formatters;
using Xunit;

namespace ManageATenancyAPI.Tests.Formatters
{
    public class DateTimeFormatterTests
    {
        [Fact]
        public void returns_a_formatted_postcode()
        {
            var formattedDateTime = DateTimeFormatter.FormatDateTimeToUtc(new DateTime(2017,10,18,12,00,00));
            Assert.Equal(formattedDateTime, "2017-10-18T12:00:00Z");
        }
    }
}
