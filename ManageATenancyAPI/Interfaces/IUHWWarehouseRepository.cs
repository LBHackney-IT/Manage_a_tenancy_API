using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Interfaces
{
    public interface IUHWWarehouseRepository
    {
        Task<object> GetTagReferencenumber(string hackneyhomesId);
    }
}
