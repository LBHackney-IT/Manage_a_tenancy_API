using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Formatters
{
    public class PostcodeFormatter
    {
        public string FormatPostcode(string postcode)
        {
            postcode = postcode.ToUpper().Replace(" ", "").Trim();
            var firstpart = postcode.Substring(0, postcode.Length - 3);
            var secondpart = postcode.Substring(Math.Max(0, postcode.Length - 3));
            var formattedPostcode = $"{firstpart} {secondpart}";
            return formattedPostcode;
        }
    }
}
