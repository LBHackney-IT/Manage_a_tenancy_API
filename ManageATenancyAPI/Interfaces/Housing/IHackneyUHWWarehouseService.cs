using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Interfaces.Housing
{
    public interface IHackneyUHWWarehouseService
    {
        Task<object> GetTagReferencenumber(string hackneyhomesId);
    }
}
