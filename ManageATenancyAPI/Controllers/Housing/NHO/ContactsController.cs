using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions;
using ManageATenancyAPI.Actions.Housing;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Factories;
using ManageATenancyAPI.Factories.Housing;
using ManageATenancyAPI.Models.Housing.NHO;
using Microsoft.AspNetCore.Mvc;
using ManageATenancyAPI.Helpers;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Services;
using ManageATenancyAPI.Services.Housing;
using ManageATenancyAPI.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MyPropertyAccountAPI.Configuration;

namespace ManageATenancyAPI.Controllers.Housing.NHO
{
    [Authorize]
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class ContactsController : Controller
    {
        private readonly ILoggerAdapter<ContactsController> _logger;
        private readonly IHackneyHousingAPICall _accountsApiCall;
        private readonly IHackneyGetCRM365Token _getCRM365AccessToken;
        private readonly ILoggerAdapter<HackneyHousingAPICallBuilder> _apiBuilderLoggerAdapter;
        private readonly IHackneyHousingAPICallBuilder _contactsAPICallBuilder;
        private readonly ILoggerAdapter<ContactsActions> _loggerActionAdapter;
        private readonly IOptions<ConnStringConfiguration> _connStringConfiguration;

        public ContactsController(ILoggerAdapter<ContactsController> loggerAdapter,
            ILoggerAdapter<ContactsActions> loggerActionAdapter, IOptions<URLConfiguration> config, IOptions<ConnStringConfiguration> connStringConfig)
        {
           
            var serviceFactory = new HackneyAccountsServiceFactory();
            _accountsApiCall = serviceFactory.build();
            _logger = loggerAdapter;
            _contactsAPICallBuilder =
                new HackneyHousingAPICallBuilder(config, _apiBuilderLoggerAdapter);
            _getCRM365AccessToken = new HackneyGetCRM365Token(config);
            _loggerActionAdapter = loggerActionAdapter;
            _connStringConfiguration = connStringConfig;
        }

        // POST: api/Contact
        /// <summary>
        /// Creates a new CRM contact
        /// </summary>
        /// <returns>Returns the guid of the created contact</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Contact contact)
        {
            try
            {
                var validator = new HousingQueryParameterValidator();
                var validateResult = validator.validateCreateContactRequest(contact);
                if (validateResult.Valid)
                {
                    var contactsActions = new ContactsActions(_loggerActionAdapter, _contactsAPICallBuilder, _accountsApiCall,_getCRM365AccessToken, _connStringConfiguration);

                    var createContact = await contactsActions.CreateContact(contact);

                    var json = Json(createContact);

                    json.StatusCode = 201;
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
            catch (ContactsServiceException ex)
            {

                var errors = new List<ApiErrorMessage>
                {
                    new ApiErrorMessage
                    {
                        developerMessage = ex.Message,
                        userMessage = "We had some problems processing your request"
                    }
                };
                _logger.LogError(ex.Message);
                var json = Json(errors);
                json.StatusCode = 500;
                json.ContentType = "application/json";
                return json;
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
                _logger.LogError(ex.Message);
                var json = Json(errors);
                json.StatusCode = 500;
                json.ContentType = "application/json";
                return json;
            }
        }

        [HttpPut]
        public async Task<JsonResult> Put(string contactId, [FromBody] Contact contact)
        {
            try
            {
                var validator = new HousingQueryParameterValidator();
                var validateResult = validator.validateUpdateAccountRequest(contactId, contact);
                if (validateResult.Valid)
                {
                    var contactsActions = new ContactsActions(_loggerActionAdapter, _contactsAPICallBuilder, _accountsApiCall,_getCRM365AccessToken, _connStringConfiguration);

                    var createContact = await contactsActions.UpdateContact(contactId, contact);

                    var json = Json(createContact);

                    json.StatusCode = 201;
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
            catch (ContactsServiceException ex)
            {

                var errors = new List<ApiErrorMessage>
                {
                    new ApiErrorMessage
                    {
                        developerMessage = ex.Message,
                        userMessage = "We had some problems processing your request"
                    }
                };
                _logger.LogError(ex.Message);
                var json = Json(errors);
                json.StatusCode = 500;
                json.ContentType = "application/json";
                return json;
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
                _logger.LogError(ex.Message);
                var json = Json(errors);
                json.StatusCode = 500;
                json.ContentType = "application/json";
                return json;
            }
        }

        [Route("GetCautionaryAlerts")]
        [HttpGet]

        public async Task<JsonResult> GetContactCautionaryAlerts(string urpn)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(urpn))
                {
                    var contactsActions = new ContactsActions(_loggerActionAdapter, _contactsAPICallBuilder, _accountsApiCall,_getCRM365AccessToken, _connStringConfiguration);

                    var getAlerts = await contactsActions.GetCautionaryAlert(urpn);

                    var json = Json(getAlerts);

                    json.StatusCode = 200;
                    json.ContentType = "application/json";
                    return json;
                }
                else
                {
                    var errors = new List<ApiErrorMessage>
                    {
                        new ApiErrorMessage
                        {
                            developerMessage = "Please a valid URPN",
                            userMessage = "Pleasea valid URPN"
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
                        userMessage = "We had some problems processing your request"
                    }
                };
                _logger.LogError(ex.Message);
                var json = Json(errors);
                json.StatusCode = 500;
                json.ContentType = "application/json";
                return json;
            }

        }

        [Route("RemoveCautionaryAlerts")]
        [HttpPost]
        public async Task<JsonResult> RemoveContactCautionaryAlerts([FromBody] CautionaryAlert cautionaryAlerts)
        {
            try
            {
                var validator = new HousingQueryParameterValidator();
                var validateResult = validator.ValidateRemoveCautionaryAlert(cautionaryAlerts);
                if (validateResult.Valid)
                {
                    var contactsActions = new ContactsActions(_loggerActionAdapter, _contactsAPICallBuilder, _accountsApiCall,_getCRM365AccessToken, _connStringConfiguration);

                    var getAlerts = await contactsActions.RemoveCautionaryAlerts(cautionaryAlerts);

                    var json = Json(getAlerts);

                    json.StatusCode = 200;
                    json.ContentType = "application/json";
                    return json;
                }
                else
                {
                    var errors = new List<ApiErrorMessage>
                    {
                        new ApiErrorMessage
                        {
                            developerMessage = "Please a valid 'Remove cautionary alert' request",
                            userMessage = "Pleasea 'Remove cautionary alert' request"
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
                        userMessage = "We had some problems processing your request"
                    }
                };
                _logger.LogError(ex.Message);
                var json = Json(errors);
                json.StatusCode = 500;
                json.ContentType = "application/json";
                return json;
            }

        }

        [Route("CreateCautionaryAlerts")]
        [HttpPost]
        public async Task<JsonResult> CreateContactCautionaryAlerts([FromBody] CautionaryAlert cautionaryAlerts)
        {
            try
            {
                var validator = new HousingQueryParameterValidator();
                var validateResult = validator.ValidateCreateCautionaryAlert(cautionaryAlerts);
                if (validateResult.Valid)
                {
                    var contactsActions = new ContactsActions(_loggerActionAdapter, _contactsAPICallBuilder, _accountsApiCall,_getCRM365AccessToken, _connStringConfiguration);

                    var getAlerts = await contactsActions.CreateCautionaryAlerts(cautionaryAlerts);

                    var json = Json(getAlerts);

                    json.StatusCode = 201;
                    json.ContentType = "application/json";
                    return json;
                }
                else
                {
                    var errors = new List<ApiErrorMessage>
                    {
                        new ApiErrorMessage
                        {
                            developerMessage = "Please a valid 'Create cautionary alert' request",
                            userMessage = "Pleasea 'Create cautionary alert' request"
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
                        userMessage = "We had some problems processing your request"
                    }
                };
                _logger.LogError(ex.Message);
                var json = Json(errors);
                json.StatusCode = 500;
                json.ContentType = "application/json";
                return json;
            }

        }
        [Route("GetContactDetails")]
        [HttpGet]
        public async Task<JsonResult> GetContactDetailsByContactId(string contactId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(contactId))
                {
                    var contactsActions = new ContactsActions(_loggerActionAdapter, _contactsAPICallBuilder, _accountsApiCall,_getCRM365AccessToken, _connStringConfiguration);

                    var getAlerts = await contactsActions.GetContactDetailsByContactId(contactId);

                    var json = Json(getAlerts);

                    json.StatusCode = 200;
                    json.ContentType = "application/json";
                    return json;
                }
                else
                {
                    var errors = new List<ApiErrorMessage>
                    {
                        new ApiErrorMessage
                        {
                            developerMessage = "Please a valid contact id",
                            userMessage = "Pleasea valid contact id"
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
                        userMessage = "We had some problems processing your request"
                    }
                };
                _logger.LogError(ex.Message);
                var json = Json(errors);
                json.StatusCode = 500;
                json.ContentType = "application/json";
                return json;
            }

        }

        [Route("GetContactsByUprn")]
        [HttpGet]

        public async Task<JsonResult> GetContactsByUprn(string urpn)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(urpn))
                {
                    var contactsActions = new ContactsActions(_loggerActionAdapter, _contactsAPICallBuilder, _accountsApiCall,_getCRM365AccessToken, _connStringConfiguration);

                    var getContact = await contactsActions.GetContactsByUprn(urpn);

                    var json = Json(getContact);

                    json.StatusCode = 200;
                    json.ContentType = "application/json";
                    return json;
                }
                else
                {
                    var errors = new List<ApiErrorMessage>
                    {
                        new ApiErrorMessage
                        {
                            developerMessage = "Please a valid URPN",
                            userMessage = "Pleasea valid URPN"
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
                        userMessage = "We had some problems processing your request"
                    }
                };
                _logger.LogError(ex.Message);
                var json = Json(errors);
                json.StatusCode = 500;
                json.ContentType = "application/json";
                return json;
            }

        }
        // PUT: api/Contact
        /// <summary>
        /// Updates next of kin details saved against a contact
        /// </summary>
        /// <returns>Returns the updated information</returns>
        [Route("UpdateNextOfKin")]
        [HttpPatch]
        public async Task<JsonResult> UpdateNextOfKin([FromBody] NextOfKin nextOfKinDetails)
        {
            try
            {
                if (nextOfKinDetails.contactID != Guid.Empty)
                {
                    var contactsActions = new ContactsActions(_loggerActionAdapter, _contactsAPICallBuilder, _accountsApiCall, _getCRM365AccessToken, _connStringConfiguration);

                    var getContact = await contactsActions.UpdateNextOfKin(nextOfKinDetails);

                    var json = Json(getContact);

                    json.StatusCode = 204;
                    json.ContentType = "application/json";
                    return json;
                }
                else
                {
                    var errors = new List<ApiErrorMessage>
                    {
                        new ApiErrorMessage
                        {
                            developerMessage = "Please provide a valid request",
                            userMessage = "Pleasea provide a valid request"
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
                        userMessage = "We had some problems processing your request"
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