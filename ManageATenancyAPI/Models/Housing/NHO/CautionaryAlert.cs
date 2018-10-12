using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class CautionaryAlert
    {
        public List<string> cautionaryAlertIds { get; set; }
        //List in case more than one alert created at a time
        public List<string> cautionaryAlertType { get; set; }
        public string contactId { get; set; }
        public string uprn { get; set; }
        //determines if the update is to remove or add cautionary alert flag
        public bool cautionaryAlertIsToBeRemoved { get; set; }
    }
}
