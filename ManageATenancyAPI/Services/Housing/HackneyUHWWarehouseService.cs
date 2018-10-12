using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using Newtonsoft.Json;

namespace ManageATenancyAPI.Services.Housing
{
    public class HackneyUHWWarehouseService : IHackneyUHWWarehouseService
    {

        
        private IUHWWarehouseRepository _uhwWarehouseRepository;
        private ILoggerAdapter<AccountActions> _logger;
        public HackneyUHWWarehouseService(IUHWWarehouseRepository uhwWarehouseRepository, ILoggerAdapter<AccountActions> logger)
        {

            _uhwWarehouseRepository = uhwWarehouseRepository;
            _logger = logger;
        }

        public async Task<object> GetTagReferencenumber(string hackneyhomesId)
        {
            _logger.LogInformation($"Getting Tag Reference number for hackneyhomesId {hackneyhomesId}");
            var response = await _uhwWarehouseRepository.GetTagReferencenumber(hackneyhomesId);
            _logger.LogInformation($"HackneyUHWWarehouseService/GetTagReferencenumber(): Received response from upstream ServiceClient (hackneyhomesId: {hackneyhomesId})");

            return response;
        }
    }
}
