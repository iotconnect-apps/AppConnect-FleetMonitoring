using component.eventbus;
using component.eventbus.Common;
using component.eventbus.Model.Topic.Logger;
using component.logger.data.log;
using component.logger.data.log.Context;
using component.logger.data.log.Interface;
using component.logger.data.log.Repositories;
using component.logger.platform.common;
using component.services.logger.viewer.Application.AppSetting;
using component.services.logger.viewer.Application.Scheduler;
using component.services.logger.viewer.Application.Subscribe;
using iot.solution.common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace component.services.logger.viewer
{
    public class Startup
    {
        public Startup(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();

            if (env.IsDevelopment())
            {
                builder
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            }
            else
            {
                builder
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            Common.AppConfig.Configuration = builder.Build();
            AppJsonConfig.Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<LogDataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SolutionLoggerDataConnection")));

            services
            .AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddSingleton<IConfiguration>(AppJsonConfig.Configuration);
            services.AddApplicationInsightsTelemetry(AppJsonConfig.Configuration);

            services.AddDbContext<LogDataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SolutionLoggerDataConnection")), ServiceLifetime.Transient);

            services.AddTransient<IAppSettingRepository, AppSettingRepository>();
            services.AddTransient(typeof(ServiceAppSetting));

            ServiceProvider provider = services.BuildServiceProvider();
            provider.GetService<ServiceAppSetting>().GetDefaultServiceAppSettings();

            #region Event bus config start
            ConcurrentDictionary<string, string> eventBusConfigurationList = new ConcurrentDictionary<string, string>();
            eventBusConfigurationList.TryAdd("BrokerConnection", Configuration.GetValue<string>("Logger:BrokerConnection"));
            services.AddSingleton<EventBusConfiguration>(s => new EventBusConfiguration(eventBusConfigurationList));

            services.AddSingleton<IEventBus, AzureServiceBusManager>();
            services.Configure<DomainManager>(s =>
            {
                s.ServiceType = Configuration.GetValue<string>("Logger:SolutionName");
                s.DomainConfiguration = new List<Type> { typeof(ErrorLoggerModel), typeof(FatalLoggerModel), typeof(DebugLoggerModel), typeof(WarningLoggerModel), typeof(InfoLoggerModel) };
            });
            #endregion Event bus config End

            #region Setup NLog Configuration
            string EnableInternalLogForNLog = Configuration.GetSection("AppConfiguration").GetSection("EnableInternalLogForNLog").Value ?? General.RaiseConfigurationMissingException("EnableInternalLogForNLog");

            NLogConfig.SetLogTarget(Configuration.GetConnectionString("SolutionLoggerDataConnection"), string.IsNullOrWhiteSpace(EnableInternalLogForNLog) ? false : Convert.ToBoolean(EnableInternalLogForNLog));

            #endregion Setup NLog Configuration

            services.AddSingleton<IHostedService, CleanApplicationLogScheduler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole().AddDebug();
            });

            app.UseCors("CorsPolicy");

            #region  Event bus subscribe register start
            if (new DomainManager().Logging == true)
            {
                IEventBus provider = app.ApplicationServices.GetService<IEventBus>();
                provider.Subscribe(new ErrorLoggerModel(), LoggerEventBus.SubscribeErrorLog);
                provider.Subscribe(new FatalLoggerModel(), LoggerEventBus.SubscribeFatalLog);
                provider.Subscribe(new DebugLoggerModel(), LoggerEventBus.SubscribeDebugLog);
                provider.Subscribe(new WarningLoggerModel(), LoggerEventBus.SubscribeWarningLog);
                provider.Subscribe(new InfoLoggerModel(), LoggerEventBus.SubscribeInfoLog);
                provider.Subscribe(new LostMessageNotification(), LoggerEventBus.LostMessageNotification);
            }
            #endregion  Event bus subscribe register start
        }
    }
}
