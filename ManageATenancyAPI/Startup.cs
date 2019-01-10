using System.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using Hackney.InterfaceStubs;
using Hackney.ServiceLocator;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.DbContext;
using ManageATenancyAPI.Extension;
using ManageATenancyAPI.Filters;
using ManageATenancyAPI.Tests;
using Microsoft.IdentityModel.Protocols;
using MyPropertyAccountAPI.Configuration;


namespace ManageATenancyAPI
{

    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            TestStatus.IsRunningInTests = false;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.

         
         //   var connString = Configuration.GetSection("ConnectionStrings");
            var uhCon = Configuration.GetSection("ConnectionStrings").GetValue<string>("UHWReportingWarehouse");
            services.AddDbContext<UHWWarehouseDbContext>(options =>
                options.UseSqlServer(uhCon));
            services.Configure<URLConfiguration>(Configuration.GetSection("URLs"));
            services.Configure<ConnStringConfiguration>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<AppConfiguration>(Configuration.GetSection("appConfigurations"));
            
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Version = "v1", Title = "ManageATenancyAPI" });
                // Set the comments path for the Swagger JSON and UI.
                var basePath = AppContext.BaseDirectory;
                string xmlPath = Path.Combine(basePath, "ManageATenancyAPI.xml");
                c.IncludeXmlComments(xmlPath);
            });
            services.AddCors(option =>
            {
                option.AddPolicy("AllowAny", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.AddCustomServices();
            services.AddMvc(options => options.Filters.Add(typeof(JsonExceptionFilter)));


            services.AddScoped<ICryptoMethods, Hackney.Plugin.Crypto.CryptoMethods>();


        }
        private void LoadPlugins(IServiceCollection services)
        {
            ICryptoMethods cryptoService = ServiceRegister<ICryptoMethods>.InstantiateService(
                "Hackney.Plugin.Crypto.CryptoMethods",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Plugins"),
                InstancingType.Singleton);
            cryptoService.EncryptionKey = Configuration.GetSection("appConfigurations").GetValue<string>("EncryptionKey");

            services.AddSingleton(typeof(ICryptoMethods), cryptoService);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            loggerFactory.AddNLog();
            app.AddNLogWeb();
            env.ConfigureNLog("NLog.config");
            app.UseCors("AllowAny");
            app.UseMvc();
            app.UseDeveloperExceptionPage();
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    string basePath = "/";
                    c.SwaggerEndpoint($"{basePath}swagger/v1/swagger.json", $"ManageATenancyAPI - {"Development"}");
                });
            }
            else
            {
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test")
                {
                    app.UseSwagger(
                        c => c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                            swaggerDoc.Host = "sandboxapi.hackney.gov.uk/manageatenancy")
                    );
                }
                else
                {
                    app.UseSwagger(
                        c => c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                            swaggerDoc.Host = "api.hackney.gov.uk/manageatenancy")
                    );
                }

                app.UseSwaggerUI(c =>
                {
                    string basePath = "/manageatenancy/";
                    c.SwaggerEndpoint($"{basePath}swagger/v1/swagger.json", $"ManageATenancyAPI - {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

                });

            }
        }
    }
}
