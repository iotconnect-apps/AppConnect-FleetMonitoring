using iot.solution.service.Interface;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;
namespace host.iot.solution.RecurringJobs
{
    public class TelemetryDataJob : ITelemetryDataJob
    {

        private readonly IChartService _chartService;

        private readonly LogHandler.Logger _logger;

        public TelemetryDataJob(IChartService chartService, LogHandler.Logger logger)
        {
            _chartService = chartService;
            _logger = logger;
        }

        public void DailyProcess()
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            try
            {
                var status = _chartService.TelemetrySummary_DayWise();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
        }

        public void HourlyProcess()
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            try
            {
                var status = _chartService.TelemetrySummary_HourWise();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
        }

        public void HourlyEmailProcess()
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            try
            {
                var status = _chartService.SendEmailNotification_HourWise();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
        }

        public void SubscriptionMailProcess()
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            try
            {
                var status = _chartService.SendSubscriptionNotification();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
        }

    }
}