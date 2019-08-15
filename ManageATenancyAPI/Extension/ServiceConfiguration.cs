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
using ManageATenancyAPI.Database;
using ManageATenancyAPI.Gateways.EscalateIssue;
using ManageATenancyAPI.Gateways.GetTraIssuesThatNeedEscalating;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeeting;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingAttendance;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingIssue;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting;
using ManageATenancyAPI.Gateways.SendEscalationEmailGateway;
using ManageATenancyAPI.Helpers;
using ManageATenancyAPI.Repository.Interfaces;
using ManageATenancyAPI.Services.Email;
using ManageATenancyAPI.Services.Interfaces;
using ManageATenancyAPI.Services.JWT;
using ManageATenancyAPI.Tests;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using ManageATenancyAPI.UseCases.Meeting.SignOffMeeting;

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

            services.AddTransient<IHackneyGetCRM365Token, HackneyGetCRM365Token>();

            services.AddSingleton<IClock, Clock>();
                                                
            services.AddScoped(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

            services.AddTransient<IHackneyHousingAPICallBuilder, HackneyHousingAPICallBuilder>();
            

            services.AddTransient(typeof(IDBAccessRepository), typeof(DBAccessRepository));
            services.AddTransient(typeof(ICitizenIndexRepository), typeof(CitizenIndexRepository));

            services.AddScoped<IBlockRepository, BlockRepository>();
            services.AddScoped<IEstateRepository, EstateRepository>();
            services.AddScoped<ITraEstateRepository, TraEstateRepository>();
            services.AddScoped<ITraRoleAssignmentRepository, TraRoleAssignmentRepository>();
            services.AddScoped<ITraRoleRepository, TraRoleRepository>();
            services.AddScoped<ITenancyRepository, TenancyRepository>();
            services.AddScoped<ITenancyContext, TenancyContext>();

            services.AddScoped<IBlockAction, BlockAction>();
            services.AddScoped<IETRAMeetingsAction, ETRAMeetingsAction>();
            services.AddScoped<IEstateAction, EstateAction>();
            services.AddScoped<ITraEstateAction, TraEstateAction>();
            services.AddScoped<ITraRoleAssignmentAction, TraRoleAssignmentAction>();
            services.AddScoped<ITraRoleAction, TraRoleAction>();
            services.AddScoped<ITraAction, TraAction>();
            
            services.AddScoped<ITenancyService, TenancyService>();
            services.AddScoped<INewTenancyService, NewTenancyService>();

            //must be singleton for internal caching to work
            services.AddSingleton<IDateService, DateService>();

            services.AddTransient(typeof(ITRARepository), typeof(TRARepository));
            services.AddTransient(typeof(IUHWWarehouseRepository), typeof(UHWWarehouseRepository));

            services.AddScoped<IJWTService, JWTService>();

            services.AddScoped<ISaveEtraMeetingUseCase, SaveEtraMeetingUseCase>();
            services.AddScoped<ISaveEtraMeetingGateway, SaveEtraMeetingGateway>();
            services.AddScoped<ISaveEtraMeetingIssueGateway, SaveEtraMeetingIssueGateway>();
            services.AddScoped<ISaveEtraMeetingAttendanceGateway, SaveEtraMeetingAttendanceGateway>();
            services.AddScoped<ISaveEtraMeetingSignOffMeetingGateway, SaveEtraMeetingSignOffMeetingGateway>();

            services.AddScoped<IGetEtraMeetingUseCase, GetEtraMeetingUseCase>();

            services.AddScoped<ISignOffMeetingUseCase, SignOffMeetingUseCase>();

            services.AddScoped<IJpegPersistenceService, JpegPersistenceService>();
            services.AddScoped<IJsonPersistanceService, JsonPersistanceService>();
            services.AddScoped<ISendTraConfirmationEmailGateway, SendTraConfirmationEmailGateway>();

            services.AddScoped<INotificationClient, GovNotificationClient>();

            services.AddScoped<IEscalateIssuesUseCase, EscalateIssuesUseCase>();

            services.AddScoped<IEscalateIssueGateway, EscalateIssueGateway>();
            services.AddScoped<IGetWorkingDaysGateway, GetWorkingDaysGateway>();
            services.AddScoped<ISendEscalationEmailGateway, SendEscalationEmailGateway>();
            services.AddScoped<IGetTraIssuesThatNeedEscalatingGateway, GetTraIssuesThatNeedEscalatingGateway>();
            services.AddScoped<IGetServiceAreaInformationGateway, GetServiceAreaInformationGateway>();

            services.AddScoped<IGetAreaManagerInformationGateway, GetAreaManagerInformationGateway>();
        }
    }
}
