using component.eventbus;
using component.eventbus.Common;
using component.eventbus.Common.Enum;
using component.eventbus.Model.Topic.Logger;
using component.helper;
using component.helper.Interface;
using component.logger;
using Hangfire;
using host.iot.solution.Filter;
using host.iot.solution.Middleware;
using host.iot.solution.RecurringJobs;
using IdentityServer4.AccessTokenValidation;
using iot.solution.host;
using iot.solution.service.IocConfig;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Mapper = iot.solution.service.Mapper;

namespace host.iot.solution
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            HostEnvironment = environment;
        }
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostEnvironment { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                component.helper.SolutionConfiguration.Init(HostEnvironment.WebRootPath);
                services.AddCorsMiddleware(Configuration);
                services.AddMvcCore().AddNewtonsoftJson();
                services.AddMvc(config => { config.Filters.Add(new ActionFilterAttribute()); });
                services.Configure<ApiBehaviorOptions>(options => {
                    options.SuppressModelStateInvalidFilter = true;
                });
                services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme).AddIdentityServerAuthentication("CompanyUser", o =>
                {
                    o.Authority = component.helper.SolutionConfiguration.Configuration.Token.Authority;
                    o.ApiName = component.helper.SolutionConfiguration.Configuration.Token.ApiName;
                    o.ApiSecret = component.helper.SolutionConfiguration.Configuration.Token.ApiSecret;
                    o.EnableCaching = component.helper.SolutionConfiguration.Configuration.Token.EnableCaching;
                    o.CacheDuration = TimeSpan.FromMinutes(component.helper.SolutionConfiguration.Configuration.Token.CacheDurationMinutes);
                    o.RequireHttpsMetadata = component.helper.SolutionConfiguration.Configuration.Token.RequireHttpsMetadata;
                });
               
                services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme).AddJwtBearer("Admin", options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = component.helper.SolutionConfiguration.Configuration.Token.Issuer.ToLower(),
                        ValidAudience = component.helper.SolutionConfiguration.Configuration.Token.Audience.ToLower(),
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(component.helper.SolutionConfiguration.Configuration.Token.SecurityKey))
                    };
                });

                Mapper.Configuration.Initialize();
                SwaggerExtension.ConfigureService(services, Configuration);
                ConfigureServicesCollection(services);
                ConfigureMessaging(services);
                services.AddControllers();
                ConfigureHangfireSettings(services);
                component.helper.DependencyResolver.Init(services);

                services.AddScoped<SmtpClient>((serviceProvider) =>
                {
                    var config = serviceProvider.GetRequiredService<IConfiguration>();
                    return new SmtpClient()
                    {
                        Host = component.helper.SolutionConfiguration.Configuration.SmtpSetting.Host,
                        Port = component.helper.SolutionConfiguration.Configuration.SmtpSetting.Port,
                        Credentials = new NetworkCredential(
                                component.helper.SolutionConfiguration.Configuration.SmtpSetting.UserName,
                                component.helper.SolutionConfiguration.Configuration.SmtpSetting.Password
                            )
                    };
                });
            }
            catch (Exception ex)
            {
                LogStartupError(services, ex);
            }
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            try
            {
                GlobalConfiguration.Configuration
            .UseActivator(new HangfireActivator(serviceProvider));

                app.UseHangfireServer(new BackgroundJobServerOptions { WorkerCount = Environment.ProcessorCount });
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                app.UseCorsMiddleware();
                SwaggerExtension.Configure(app);
                app.UseStaticFiles();
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseHeaderkeyAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
                if (Convert.ToBoolean(component.helper.SolutionConfiguration.Configuration.HangFire.Enabled.ToString()))
                {
                    StartHangFireBackgroundJobs();
                    if (Convert.ToBoolean(component.helper.SolutionConfiguration.Configuration.HangFire.IsSendTripMailEnabled.ToString()))
                    {
                        StartHangFireEmailBackgroundJobs();
                    }
                }
            }
            catch (Exception ex)
            {
                var log = app.ApplicationServices.GetService<component.services.loghandler.Logger>();
                if (log != null)
                {
                    log.FatalLog(ex.ToString(), ex);
                }
            }
        }
        private void ConfigureHangfireSettings(IServiceCollection services)
        {
            services.AddHangfire(x => x.UseSqlServerStorage(component.helper.SolutionConfiguration.Configuration.ConnectionString));
        }
     
        private static void StartHangFireBackgroundJobs()
        {
            // Note: if you ever remove one of these, either delete it via HangFire UI
            // or from the database manually. Removing the code entry does NOT stop the
            // existing job running.
            RecurringJob.AddOrUpdate<ITelemetryDataJob>(
                job => job.DailyProcess(),
                Cron.Daily);
            RecurringJob.AddOrUpdate<ITelemetryDataJob>(
                job => job.SubscriptionMailProcess(),
                Cron.Daily);
            //RecurringJob.AddOrUpdate<ITelemetryDataJob>(
            //   job => job.HourlyProcess(),  string.Format("0 */{0} * * *", component.helper.SolutionConfiguration.Configuration.HangFire.TelemetryHours));
            RecurringJob.AddOrUpdate<ITelemetryDataJob>(
             job => job.HourlyProcess(), string.Format("*/{0} * * * *", component.helper.SolutionConfiguration.Configuration.HangFire.TelemetryHours));
        }

        private static void StartHangFireEmailBackgroundJobs()
        {
            // Note: if you ever remove one of these, either delete it via HangFire UI
            // or from the database manually. Removing the code entry does NOT stop the
            // existing job running.
            RecurringJob.AddOrUpdate<ITelemetryDataJob>(
             job => job.HourlyEmailProcess(), string.Format("*/{0} * * * *", component.helper.SolutionConfiguration.Configuration.HangFire.DelayedMinutes));
        }
        private void ConfigureServicesCollection(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            #region Solution Logger
            ConcurrentDictionary<string, string> eventBusConfigurationList = new ConcurrentDictionary<string, string>();
            eventBusConfigurationList.TryAdd("BrokerConnection", component.helper.SolutionConfiguration.Configuration.Logger.BrokerConnection);
            services.AddSingleton(s => new EventBusConfiguration(eventBusConfigurationList));

            services.Configure<DomainManager>(s =>
            {
                //TODO: change serviceType accrodingly
                s.ApplicationType = component.helper.SolutionConfiguration.Configuration.Logger.SolutionName;
                s.ServiceType = component.helper.SolutionConfiguration.Configuration.Logger.SolutionName + "_LoggerService";

                s.DomainConfiguration = new List<Type>
                    {
                        typeof(DebugLoggerModel), typeof(ErrorLoggerModel), typeof(WarningLoggerModel), typeof(InfoLoggerModel), typeof(FatalLoggerModel)
                    };
            });
            services.AddTransient<ILogger, Logger>();
            services.AddTransient<IEventBus, AzureServiceBusManager>();
            services.AddSingleton(typeof(component.services.loghandler.Logger));
            #endregion

            IocConfigurations.Initialize(services);
            services.AddScoped<ITelemetryDataJob, TelemetryDataJob>();
            services.AddScoped<IEmailHelper, EmailHelper>();
        }
        private void ConfigureMessaging(IServiceCollection services)
        {
            if (component.helper.SolutionConfiguration.Configuration.Messaging != null
            && !string.IsNullOrWhiteSpace(component.helper.SolutionConfiguration.Configuration.Messaging.ServicebusEndPoint))
            {
                component.messaging.CustomStartup.AddIOTConnectSyncManager(services, component.helper.SolutionConfiguration.Configuration.ConnectionString,
                component.helper.SolutionConfiguration.Configuration.Messaging.ServicebusEndPoint,
                component.helper.SolutionConfiguration.Configuration.Messaging.TopicName,
                component.helper.SolutionConfiguration.Configuration.Messaging.SubscriptionName);
            }
        }
        public void LogStartupError(IServiceCollection services, Exception ex)
        {
            services.AddScoped(typeof(component.services.loghandler.Logger));

            ConcurrentDictionary<string, string> eventBusConfigurationList = new ConcurrentDictionary<string, string>();
            eventBusConfigurationList.TryAdd("BrokerConnection", component.helper.SolutionConfiguration.Configuration.Logger.BrokerConnection);
            services.AddSingleton(s => new EventBusConfiguration(eventBusConfigurationList));

            services.Configure<DomainManager>(s =>
            {
                s.ApplicationType = component.helper.SolutionConfiguration.Configuration.Logger.SolutionName;
                s.ServiceType = component.helper.SolutionConfiguration.Configuration.Logger.SolutionName + "_LoggerService";

                s.DomainConfiguration = new List<Type>
                    {
                        typeof(DebugLoggerModel), typeof(ErrorLoggerModel), typeof(WarningLoggerModel), typeof(InfoLoggerModel), typeof(FatalLoggerModel),
                    };
            });
            var buildService = services.BuildServiceProvider();
            var logger = buildService.GetService<component.services.loghandler.Logger>().FatalLog(ex.ToString(), ex);
        }
    }
}