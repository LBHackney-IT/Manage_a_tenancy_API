using System;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyPropertyAccountAPI.Configuration;

namespace ManageATenancyAPI.Controllers
{
    [ServiceFilter(typeof(AdminEnabledFilter))]
    [Route("api/config")]
    public class ConfigController : Controller
    {
        private AppConfiguration _appConfiguration;
        private ConnStringConfiguration _connStringConfiguration;
        private URLConfiguration _urlConfiguration;
        public ConfigController(IOptions<AppConfiguration> appConfiguration,
            IOptions<ConnStringConfiguration> connStringConfiguration,
            IOptions<URLConfiguration> urlConfiguration)
        {
            _appConfiguration = appConfiguration.Value;
            _connStringConfiguration = connStringConfiguration.Value;
            _urlConfiguration = urlConfiguration.Value;
        }

        [Route("environment-variables")]
        [HttpGet]
        public dynamic GetEnv()
        {
            return Environment.GetEnvironmentVariables();
        }

        [Route("api-configuration")]
        [HttpGet]
        public dynamic GetApiConfig()
        {
            var usingConfig = new
            {
                AppConfiguration = _appConfiguration,
                ConnStringConfiguration = _connStringConfiguration,
                UrlConfiguration = _urlConfiguration
            };
            return usingConfig;
        }
    }
}
