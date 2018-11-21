using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public class TRAActions
    {
        private HttpClient _client;
        private ILoggerAdapter<TRAActions> _logger;
        private ITRARepository _traRepository;

        public TRAActions(ILoggerAdapter<TRAActions> logger, ITRARepository traRepository)
        {
            _logger = logger;
            _traRepository = traRepository;
        }

        public async Task<object> GetTRAForPatch(string patchId)
        {
           try
            {
                _logger.LogInformation($"Get TRAs for a patch requested Started");

                List<TRA> TRAResults = _traRepository.FindTRAsForPatch(patchId);

                if (TRAResults != null)
                {
                    return new 
                    {
                        results = TRAResults
                    };
                }
                else
                {
                    _logger.LogError($"TRAs not found for the given patch");
                    throw new MissingTRAsForPatchException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetTRAForPatch has encountered an error: " + ex.InnerException);
                throw ex;
            }
        }
    }

    public class MissingTRAsForPatchException : Exception
    {
    }
}
