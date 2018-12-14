using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ManageATenancyAPI.Models
{

    public class HackneyResult<T> 
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public T Result { get; set; }

        public static HackneyResult<T> Create<T>(T result)
        {
            return new HackneyResult<T>() { Result = result };
        }
     
    }
}
