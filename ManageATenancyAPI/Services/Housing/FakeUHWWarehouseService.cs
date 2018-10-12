using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using Newtonsoft.Json;

namespace ManageATenancyAPI.Services.Housing
{
    public class FakeUHWWarehouseService : IHackneyUHWWarehouseService
    {
        public Task<object> GetTagReferencenumber(string hackneyhomesId)
        {
            object tagReference = "";
            if (hackneyhomesId== "316669")
            {
                tagReference = "12345/01";
                return Task.Run(() => tagReference);
            }
           
            tagReference = HttpStatusCode.InternalServerError;
            return Task.Run(() => tagReference);
                    
            

        }
        
    }
}
