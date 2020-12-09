using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;
using Request = iot.solution.entity.Request;
using System.Linq;

namespace host.iot.solution.Controllers
{
    [Route(ChartRoute.Route.Global)]
    [ApiController]
    public class ChartController : BaseController
    {
        private readonly IChartService _chartService;
        
        public ChartController(IChartService chartService)
        {
            _chartService = chartService;
        }
       

        [HttpPost]
        [Route(ChartRoute.Route.EnergyUsageByFleet, Name = ChartRoute.Name.EnergyUsageByFleet)]
        public Entity.BaseResponse<List<Response.EnergyUsageResponse>> EnergyUsageByFleet(Request.ChartRequest request)
        {
            Entity.BaseResponse<List<Response.EnergyUsageResponse>> response = new Entity.BaseResponse<List<Response.EnergyUsageResponse>>(true);
            try
            {
                response.Data = _chartService.GetEnergyUsageByFleet(request);
                if (response.Data.Count == 0)
                {
                    response.IsSuccess = false;
                    response.Message = "No usage found";
                }
            }
            catch (Exception ex) {
                base.LogException(ex);
            }
            return response;
        }
        [HttpPost]
        [Route(ChartRoute.Route.OdometerReadingByFleet, Name = ChartRoute.Name.OdometerReadingByFleet)]
        public Entity.BaseResponse<List<Response.OdometerResponse>> OdometerReadingByFleet(Request.ChartRequest request)
        {
            Entity.BaseResponse<List<Response.OdometerResponse>> response = new Entity.BaseResponse<List<Response.OdometerResponse>>(true);
            try
            {
                response.Data = _chartService.GetOdometerByFleet(request);
                if (response.Data.Count == 0)
                {
                    response.IsSuccess = false;
                    response.Message = "No reading found";
                }
            }
            catch (Exception ex)
            {
                base.LogException(ex);
            }
            return response;
        }
        [HttpPost]
        [Route(ChartRoute.Route.EnergyUsage, Name = ChartRoute.Name.EnergyUsage)]
        public Entity.BaseResponse<List<Response.EnergyUsageResponse>> EnergyUsage(Request.ChartRequest request)
        {
            Entity.BaseResponse<List<Response.EnergyUsageResponse>> response = new Entity.BaseResponse<List<Response.EnergyUsageResponse>>(true);
            try
            {
                response.Data = _chartService.GetEnergyUsage(request);
                if (response.Data.Count == 0)
                {
                    response.IsSuccess = false;
                    response.Message = "No usage found";
                }
            }
            catch (Exception ex)
            {
                base.LogException(ex);
            }
            return response;
        }
        [HttpPost]
        [Route(ChartRoute.Route.TripsByDriver, Name = ChartRoute.Name.TripsByDriver)]
        public Entity.BaseResponse<List<Response.TripsByDriverResponse>> TripsByDriver(Request.ChartRequest request)
        {
            Entity.BaseResponse<List<Response.TripsByDriverResponse>> response = new Entity.BaseResponse<List<Response.TripsByDriverResponse>>(true);
            try
            {
                response.Data = _chartService.GetTripsByDriver(request);
                if (response.Data.Count == 0)
                {
                    response.IsSuccess = false;
                    response.Message = "No trips found";
                }
            }
            catch (Exception ex)
            {
                base.LogException(ex);
            }
            return response;
        }
        [HttpPost]
        [Route(ChartRoute.Route.FleetStatus, Name = ChartRoute.Name.FleetStatus)]
        public Entity.BaseResponse<List<Response.FleetStatusResponse>> FleetStatus(Request.ChartRequest request)
        {
            Entity.BaseResponse<List<Response.FleetStatusResponse>> response = new Entity.BaseResponse<List<Response.FleetStatusResponse>>(true);
            try
            {
                response.Data = _chartService.GetFleetStatus(request);
                if (response.Data.Count == 0)
                {
                    response.IsSuccess = false;
                    response.Message = "No status found";
                }
            }
            catch (Exception ex)
            {
                base.LogException(ex);
            }
            return response;
        }
        [HttpPost]
        [Route(ChartRoute.Route.FleetTypeUsage, Name = ChartRoute.Name.FleetTypeUsage)]
        public Entity.BaseResponse<List<Response.FleetTypeUsageResponse>> FleetTypeUsage(Request.ChartRequest request)
        {
            Entity.BaseResponse<List<Response.FleetTypeUsageResponse>> response = new Entity.BaseResponse<List<Response.FleetTypeUsageResponse>>(true);
            try
            {
                response.Data = _chartService.GetFleetTypeUsage(request);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
            }
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(ChartRoute.Route.ExecuteCron, Name = ChartRoute.Name.ExecuteCron)]
        public Entity.BaseResponse<bool> ExecuteCron()
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var res = _chartService.TelemetrySummary_HourWise();
                var dayRes = _chartService.TelemetrySummary_DayWise();
                response.IsSuccess = res.Success;             
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                response.Message = ex.Message.ToString();
                response.IsSuccess = false;
            }
            return response;
        }
        //[HttpPost]
        //[Route(ChartRoute.Route.CompanyUsage, Name = ChartRoute.Name.CompanyUsage)]
        //public Entity.BaseResponse<List<Response.CompanyUsageResponse>> CompanyUsage(Request.ChartRequest request)
        //{
        //    Entity.BaseResponse<List<Response.CompanyUsageResponse>> response = new Entity.BaseResponse<List<Response.CompanyUsageResponse>>(true);
        //    try
        //    {
        //        response.Data = _chartService.GetCompanyUsage(request);
        //        if (response.Data.Count == 0) {
        //            response.IsSuccess = false;
        //            response.Message = "No usage found";
        //        }
        //        else if (response.Data.Count >0 && int.Parse(response.Data[0].UtilizationPer)<=0)
        //        {
        //            response.IsSuccess = false;
        //            response.Message = "No usage found";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        base.LogException(ex);
        //    }
        //    return response;
        //}

    }
}