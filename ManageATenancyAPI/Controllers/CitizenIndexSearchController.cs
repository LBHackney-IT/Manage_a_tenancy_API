using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Controllers.Housing.NHO;
using ManageATenancyAPI.Factories;
using ManageATenancyAPI.Factories.Housing;
using ManageATenancyAPI.Helpers;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Services;
using ManageATenancyAPI.Services.Housing;
using ManageATenancyAPI.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyPropertyAccountAPI.Configuration;

namespace ManageATenancyAPI.Controllers
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class CitizenIndexSearchController : Controller
    {
        
        
        private IHackneyHousingAPICall _hackneyLeaseAccountApi;
        private readonly IHackneyGetCRM365Token _accessToken;
        private readonly IHackneyHousingAPICallBuilder _leaseAPICallBuilder;
        private readonly ILoggerAdapter<HackneyHousingAPICallBuilder> _apiBuilderLoggerAdapter;        
        private readonly ILoggerAdapter<CitizenIndexSearchController> _logger;       
        private readonly ILoggerAdapter<CitizenIndexAction> _loggerActionAdapter;
        private ICitizenIndexRepository _dBAccessRepository;


        public CitizenIndexSearchController(ILoggerAdapter<CitizenIndexSearchController> loggerAdapter, ILoggerAdapter<CitizenIndexAction> loggerActionAdapter, ICitizenIndexRepository dBAccessRepository, IOptions<URLConfiguration> config)
        {
          
            var serviceFactory = new HackneyAccountsServiceFactory();
            _hackneyLeaseAccountApi = serviceFactory.build();            
           _accessToken = new HackneyGetCRM365Token(config);
            _leaseAPICallBuilder = new HackneyHousingAPICallBuilder(config, _apiBuilderLoggerAdapter);
            _logger = loggerAdapter;
            _loggerActionAdapter = loggerActionAdapter;
            _dBAccessRepository = dBAccessRepository;

        }
        [HttpGet]
        public async Task<JsonResult> Get(string firstname = "", string surname = "", string addressline12 = "", string postcode = "", bool IsAdvanceSearch=false)
        {
            try
            {
                var validator = new HousingQueryParameterValidator();

                var validateResult = validator.ValidateSearchRequest(firstname, surname, addressline12, postcode);
                if (validateResult.Valid)
                {
                    _logger.LogInformation($" Search Request Started.");

                   var citizenIndexSearchActions = new CitizenIndexAction(_loggerActionAdapter,_leaseAPICallBuilder, _hackneyLeaseAccountApi, _dBAccessRepository,_accessToken);

                    var  results = await citizenIndexSearchActions.CitizenIndexSearch(firstname, surname, addressline12, postcode, IsAdvanceSearch);

                    var json = Json(results);
                    json.StatusCode = Json(results).StatusCode;
                    json.ContentType = "application/json";
                    return json;

                }
                else
                {
                    var errors = validateResult.ErrorMessages.Select(error => new ApiErrorMessage
                    {
                        developerMessage = error,
                        userMessage = error
                    }).ToList();
                    var jsonResponse = Json(errors);
                    jsonResponse.StatusCode = 400;
                    jsonResponse.ContentType = "application/json";
                    return jsonResponse;

                }
            }
            catch (Exception ex)
            {
                var errors = new List<ApiErrorMessage>
                {
                    new ApiErrorMessage
                    {
                        developerMessage = ex.Message,
                        userMessage = "We had some problems processing your request"
                    }
                };
                var json = Json(errors);
                json.StatusCode = 500;
                return json;
            }

        }
    }
}