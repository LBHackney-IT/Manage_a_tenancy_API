using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models
{

    public class HackneyResult<T>
    {
        public T Result { get; set; }

        public static HackneyResult<T> Create<T>(T result)
        {
            return new HackneyResult<T>() { Result = result };
        }
    }
}
