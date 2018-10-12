using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Validators;
using Xunit;

namespace ManageATenancyAPI.Tests.Validations
{
    public class PostCodeValidationTests
    {
        [Theory]
        [InlineData("NR7 2RE", true)]
        [InlineData("NR", false)]
        [InlineData("NR5677", false)]
        [InlineData("8QR", false)]
        [InlineData("", false)]
        [InlineData("    ", false)]
        [InlineData(null, false)]
        [InlineData("N168QR", true)]
        [InlineData("N16 8QR", true)]
        [InlineData("n168qr", true)]
        [InlineData("n16 8qr", true)]
        [InlineData("N16   8QR", true)]
        [InlineData(" N168QR ", true)]
        [InlineData(" N16 8QR ", true)]
        public void return_a_boolean_if_postcode_is_valid(string postcode, bool expected)
        {
            var postcodeValidator = new PostcodeValidator();
            var result = postcodeValidator.Validate(postcode);
            Assert.Equal(expected, result);
        }
    }
}
