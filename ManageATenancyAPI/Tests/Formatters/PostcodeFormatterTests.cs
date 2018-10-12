using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Formatters;
using Xunit;

namespace ManageATenancyAPI.Tests.Formatters
{
    public class PostcodeFormatterTests
    {
        [Theory]
        [InlineData("N16 8RE", "N16 8RE")]
        [InlineData("n16 8re", "N16 8RE")]
        [InlineData(" n16 8re ", "N16 8RE")]
        [InlineData("n168re", "N16 8RE")]
        [InlineData("n 168re", "N16 8RE")]
        [InlineData("n168 re", "N16 8RE")]
        [InlineData("e70nn", "E7 0NN")]
        public void returns_a_formatted_postcode(string postcode, string expected)
        {
            var postcodeFormatter = new PostcodeFormatter();
            var formattedPostcode = postcodeFormatter.FormatPostcode(postcode);
            Assert.Equal(formattedPostcode, expected);
        }
    }
}
