using ManageATenancyAPI.Factories;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Logging;
using ManageATenancyAPI.Repository;
using ManageATenancyAPI.Services;
using ManageATenancyAPI.Services.Housing;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Tests;

namespace ManageATenancyAPI.Extension
{
    public static class ServiceConfiguration
    {
        private static readonly ILoggerAdapter<HackneyHousingAPICallBuilder> _apiBuilderLoggerAdapter;
        public static void AddCustomServices(this IServiceCollection services)
        {
            if (TestStatus.IsRunningInTests == false)
            {
                services.AddTransient(typeof(IHackneyHousingAPICall), typeof(HackneyHousingAPICall));
            }
            else
            {
                services.AddTransient(typeof(IHackneyHousingAPICall), typeof(FakeHousingAPICall));
            }
                                                
            services.AddScoped(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

            services.AddTransient<IHackneyHousingAPICallBuilder, HackneyHousingAPICallBuilder>();
            services.AddTransient<IHackneyGetCRM365Token, HackneyGetCRM365Token>();

            services.AddTransient(typeof(IDBAccessRepository), typeof(DBAccessRepository));
            services.AddTransient(typeof(ICitizenIndexRepository), typeof(CitizenIndexRepository));

            services.AddScoped<IBlockRepository, BlockRepository>();
            services.AddScoped<IEstateRepository, EstateRepository>();
            services.AddScoped<ITraEstateRepository, TraEstateRepository>();
            services.AddScoped<ITraRoleAssignmentRepository, TraRoleAssignmentRepository>();
            services.AddScoped<ITraRoleRepository, TraRoleRepository>();


            services.AddScoped<IBlockAction, BlockAction>();
            services.AddScoped<IETRAMeetingsAction, ETRAMeetingsAction>();
            services.AddScoped<IEstateAction, EstateAction>();
            services.AddScoped<ITraEstateAction, TraEstateAction>();
            services.AddScoped<ITraRoleAssignmentAction, TraRoleAssignmentAction>();
            services.AddScoped<ITraRoleAction, TraRoleAction>();
            services.AddScoped<ITraAction, TraAction>();

            services.AddScoped<IOfficerService, OfficerService>();
            services.AddScoped<IDateService, DateService>();

            services.AddTransient(typeof(ITRARepository), typeof(TRARepository));
            services.AddTransient(typeof(IUHWWarehouseRepository), typeof(UHWWarehouseRepository));
        }
    }
}
