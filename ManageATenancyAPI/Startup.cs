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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Hackney.InterfaceStubs;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Database;
using ManageATenancyAPI.DbContext;
using ManageATenancyAPI.Extension;
using ManageATenancyAPI.Filters;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting;
using ManageATenancyAPI.Tests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyPropertyAccountAPI.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Swashbuckle.AspNetCore.SwaggerGen;


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

        private static List<ApiVersionDescription> _apiVersions { get; set; }
        //TODO update the below to the name of your API
        private const string ApiName = "Manage A Tenancy";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.

            services.AddSingleton<IApiVersionDescriptionProvider, DefaultApiVersionDescriptionProvider>();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Token",
                    new ApiKeyScheme
                    {
                        In = "header",
                        Description = "Your Hackney API Key",
                        Name = "x-api-key",
                        Type = "apiKey"
                    });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Token", Enumerable.Empty<string>()}
                });

                c.SwaggerDoc("v1", new Info
                {
                    Title = $"{ApiName}-api 1.0",
                    Version = "1.0",
                    Description = $"{ApiName} version 1.0. Please check older versions for depreceted endpoints."
                });

                c.CustomSchemaIds(x => x.FullName);
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });


            //   var connString = Configuration.GetSection("ConnectionStrings");
            var uhCon = Configuration.GetSection("ConnectionStrings").GetValue<string>("UHWReportingWarehouse");
            services.AddDbContext<UHWWarehouseDbContext>(options =>
                options.UseSqlServer(uhCon));
            
            var tenancyConnection = Configuration.GetSection("ConnectionStrings").GetValue<string>("TenancyDBConnection");
            services.AddDbContext<TenancyContext>(options => 
                options.UseSqlServer(tenancyConnection));
            
            services.Configure<URLConfiguration>(Configuration.GetSection("URLs"));
            services.Configure<ConnStringConfiguration>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<AppConfiguration>(Configuration.GetSection("appConfigurations"));
            services.Configure<EmailConfiguration>(Configuration.GetSection("emailConfiguration"));
            //S3 related
            services.Configure<JpegPersistenceServiceConfiguration>(options =>
            {
                options.Extension = "jpg";
                options.FileType = FileType.Jpeg;
                options.ProjectName = "etra";
            });

            services.Configure<S3Configuration>(Configuration.GetSection("S3Configuration"));

            services.AddMvc();
            services.AddCors(option =>
            {
                option.AddPolicy("AllowAny", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.AddCustomServices();
            services.AddMvc(options => options.Filters.Add(typeof(JsonExceptionFilter)));


            services.AddScoped<ICryptoMethods, Hackney.Plugin.Crypto.CryptoMethods>();
            services.AddScoped<AdminEnabledFilter>();

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    var secret = Configuration["HmacSecret"];
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = key,
                        RequireExpirationTime = true,
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateAudience = false
                    };
            });

            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //add db connection string to logger
            NLog.GlobalDiagnosticsContext.Set("ManageATenancyDatabase", Configuration.GetConnectionString("ManageATenancyDatabase"));

            loggerFactory.AddNLog();
            app.AddNLogWeb();
            env.ConfigureNLog("NLog.config");
            app.UseCors("AllowAny");


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //GetAsync All ApiVersions,
            var api = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            //GetAsync All ApiVersions,
            _apiVersions = api.ApiVersionDescriptions.Select(s => s).ToList();
            //Swagger ui to view the swagger.json file
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"v1/swagger.json", $"{ApiName}-api v1");
            });

            app.UseSwagger();


            app.UseAuthentication();
            app.UseMvc();

        }
    }
}
