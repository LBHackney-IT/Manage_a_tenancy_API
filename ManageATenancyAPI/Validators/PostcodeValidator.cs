using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces;

namespace ManageATenancyAPI.Validators
{
    public class PostcodeValidator : IPostcodeValidator
    {
        public bool Validate(string postcode)
        {
            if (string.IsNullOrWhiteSpace(postcode))
                return false;
            postcode = postcode.Replace(" ", "");
            var postcodePattern = "^([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9]?[A-Za-z]))))s?[0-9][A-Za-z]{2})$";
            var postcodereg = new Regex(postcodePattern, RegexOptions.IgnoreCase);
            return postcodereg.IsMatch(postcode);
        }
    }
}
