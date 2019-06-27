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
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Hackney.InterfaceStubs;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Database;
using ManageATenancyAPI.DbContext;
using ManageATenancyAPI.Extension;
using ManageATenancyAPI.Filters;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting;
using ManageATenancyAPI.Tests;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyPropertyAccountAPI.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;


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
            
            var tenancyConnection = Configuration.GetSection("ConnectionStrings").GetValue<string>("TenancyDBConnection");
            services.AddDbContext<TenancyContext>(options => 
                options.UseSqlServer(tenancyConnection));
            
            services.Configure<URLConfiguration>(Configuration.GetSection("URLs"));
            services.Configure<ConnStringConfiguration>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<AppConfiguration>(Configuration.GetSection("appConfigurations"));
            //S3 related
            services.Configure<JpegPersistenceServiceConfiguration>(m => new JpegPersistenceServiceConfiguration
            {
                Extension = "jpg",
                FileType = FileType.Jpeg,
                ProjectName = "etra"
            });

            services.Configure<S3Configuration>(Configuration.GetSection("S3Configuration"));

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

            app.UseAuthentication();

            app.UseMvc();
            app.UseDeveloperExceptionPage();

                //Legacy support for old servers
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
