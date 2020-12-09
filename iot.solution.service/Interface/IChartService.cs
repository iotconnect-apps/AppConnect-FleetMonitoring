using iot.solution.entity;
using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Request = iot.solution.entity.Request;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Interface
{
    public interface IChartService
    {
        Entity.ActionStatus TelemetrySummary_DayWise();
        Entity.ActionStatus TelemetrySummary_HourWise();
        Entity.ActionStatus SendEmailNotification_HourWise();
        Entity.ActionStatus SendSubscriptionNotification();
        Entity.ActionStatus SendEmailNotification_Radius(Entity.FleetDetail data,double radiusInKM);
        List<Response.EnergyUsageResponse> GetEnergyUsage(Request.ChartRequest request);
        List<Response.TripsByDriverResponse> GetTripsByDriver(Request.ChartRequest request);
        List<Response.FleetStatusResponse> GetFleetStatus(Request.ChartRequest request);
        List<Response.EnergyUsageResponse> GetEnergyUsageByFleet(Request.ChartRequest request);
        List<Response.OdometerResponse> GetOdometerByFleet(Request.ChartRequest request);

        //  List<Response.CompanyUsageResponse> GetCompanyUsage(Request.ChartRequest request);

        // Entity.BaseResponse<List<Response.DeviceStatisticsResponse>> GetStatisticsByDevice(Request.ChartRequest request);
        List<Response.FleetTypeUsageResponse> GetFleetTypeUsage(Request.ChartRequest request);
        
    }
}
