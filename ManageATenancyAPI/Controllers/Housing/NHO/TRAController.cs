using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Factories.Housing;
using ManageATenancyAPI.Formatters;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Repository;
using ManageATenancyAPI.Services;
using ManageATenancyAPI.Services.Housing;
using ManageATenancyAPI.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyPropertyAccountAPI.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ManageATenancyAPI.Controllers.Housing.NHO
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class TRAController : Controller
    {

        private readonly ILoggerAdapter<TRAController> _logger;
        private ITRARepository _traRepository;
        private ITraAction _traAction;
        private ITraEstateAction _traEstateAction;
        private IEstateAction _estateAction;
        private ITraRoleAssignmentAction _traRoleAssignmentAction;
        private readonly ILoggerAdapter<TRAActions> _actionsLogger;
        public TRAController(ILoggerAdapter<TRAActions> actionsLogger, ILoggerAdapter<TRAController> loggerAdapter, IOptions<URLConfiguration> config, ITRARepository traRepository, ITraEstateAction traEstateAction, IEstateAction estateAction, ITraRoleAssignmentAction traRoleAssignmentAction, ITraAction traAction)
        {
            _logger = loggerAdapter;
            _actionsLogger = actionsLogger;
            _traRepository = traRepository;
            _traAction = traAction;
            _traEstateAction = traEstateAction;
            _estateAction = estateAction;
            _traRoleAssignmentAction = traRoleAssignmentAction;
        }

        [Route("GetTRAForPatch")]
        [HttpGet]
        public async Task<JsonResult> GetTRAbyPatchId(Guid patchId)
        {
            try
            {
                if (patchId != Guid.Empty)
                {
                    var actions = new TRAActions(_actionsLogger, _traRepository);
                    var results = actions.GetTRAForPatch(patchId.ToString()).Result;
                    return Json(results);
                }
                else
                {
                    var errors = new List<ApiErrorMessage>
                    {
                        new ApiErrorMessage
                        {
                            developerMessage = "Bad Request",
                            userMessage = "Please provide a valid patch ID"
                        }
                    };

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
                        userMessage = "We had some problems processing your request - " + ex.InnerException
                    }
                };
                _logger.LogError(ex.Message);
                var json = Json(errors);
                json.StatusCode = 500;
                json.ContentType = "application/json";
                return json;
            }
        }


        [HttpPatch]
        [Route("{traId}")]
        public async Task<IActionResult> UpdateTra(int traId, [FromBody] TraRequest tra)
        {

            if (await _traAction.Exists(tra.Name))
            {
                if (_traEstateAction.AreUnusedEstates(tra.EstateRefs))
                {
                    var persistedTra = await _traAction.Find(tra.Name);

                    var estates = await _estateAction.GetEstates(tra.EstateRefs);
                    foreach (var estate in estates)
                    {
                        _traEstateAction.AddEstateToTra(persistedTra.TRAId, estate.EstateId, estate.EstateName);
                    }
                }
                else
                {
                    return BadRequest("Request contains Estate Ids that are already used.");
                }
            }
            else
            {
                return BadRequest("This Tra already exists.");
            }
            return Ok();
        }
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateTra([FromBody] TraRequest tra)
        {

            if (await _traAction.Exists(tra.Name))
            {
                if (_traEstateAction.AreUnusedEstates(tra.EstateRefs))
                {
                    var persistedTra = await _traAction.Create(tra.Name, tra.Notes, tra.Email, tra.AreaId, tra.PatchId);

                    var estates = await _estateAction.GetEstates(tra.EstateRefs);
                    foreach (var estate in estates)
                    {
                        _traEstateAction.AddEstateToTra(persistedTra.TRAId, estate.EstateId, estate.EstateName);
                    }
                }
                else
                {
                    return BadRequest("Request contains Estate Ids that are already used.");
                }
            }
            else
            {
                return BadRequest("This Tra already exists.");
            }
            return Ok();
        }

        [HttpPost]
        [Route("{traId}/representative/{personName}/{role}")]
        public async Task<HackneyResult<bool>> AddRepresentative(int traId, string personName, string role)
        {
            _traRoleAssignmentAction.AddRepresentative(traId, personName, role);
                  return HackneyResult<bool>.Create(true);
        }

        [HttpDelete]
        [Route("{traId}/representative/{personName}")]
        public async Task<HackneyResult<bool>> RemoveRepresentative(int traId, string personName)
        {
            _traRoleAssignmentAction.RemoveRepresentative(traId, personName);
            return HackneyResult<bool>.Create(true);
        }

        [HttpGet]
        [Route("{traId}/representatives")]
        public async Task<HackneyResult<List<RoleAssignment>>> ListRepresentatives(int traId)
        {
            var res = await _traRoleAssignmentAction.GetRepresentatives(traId);
            return HackneyResult<List<RoleAssignment>>.Create(res);
        }

        [Route("GetTRAInformation")]
        [HttpGet]
        public async Task<JsonResult> GetTRAInformation(int TRAId)
        {
            try
            {
                if (TRAId != 0)
                {
                    var actions = new TRAActions(_actionsLogger, _traRepository);
                    var results = actions.GetTRAInformation(TRAId).Result;
                    return Json(results);
                }
                else
                {
                    var errors = new List<ApiErrorMessage>
                    {
                        new ApiErrorMessage
                        {
                            developerMessage = "Bad Request",
                            userMessage = "Please provide a valid TRA ID"
                        }
                    };

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
                        userMessage = "We had some problems processing your request - " + ex.InnerException
                    }
                };
                _logger.LogError(ex.Message);
                var json = Json(errors);
                json.StatusCode = 500;
                json.ContentType = "application/json";
                return json;
            }
        }
    }
}