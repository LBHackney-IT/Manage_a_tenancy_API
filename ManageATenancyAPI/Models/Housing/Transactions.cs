using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models.Housing
{
    public class Transactions
    {

       
        public string tagReference { get; set; }
        public string propertyReference { get; set; }
        public string transactionSid { get; set; }
        public string houseReference { get; set; }
        public string transactionType { get; set; }
        public DateTime postDate { get; set; }
        public double realValue { get; set; }
        public string transactionID { get; set; }
        public string debDesc { get; set; }


    }

    public class TransactionsObject<T>
    {
        public List<T> value { get; set; }

    }
}
