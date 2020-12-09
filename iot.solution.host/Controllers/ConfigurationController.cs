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

namespace host.iot.solution.Controllers
{
    [Route(ConfigurationRoute.Route.Global)]
    [ApiController]
    public class ConfigurationController : BaseController
    {
        private readonly IConfigurationService _configurationService;
        public ConfigurationController(IChartService chartService, IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        /// <summary>
        /// Get Confgurations
        /// </summary>
        /// <remarks>
        /// Pass 'UIAlert' for UI-Alert telemetry confguration
        /// Pass 'LiveData' for Live data telemetry confguration
        /// </remarks>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ConfigurationRoute.Route.Get, Name = ConfigurationRoute.Name.Get)]
        public Entity.BaseResponse<Response.ConfgurationResponse> Get(string key)
        {
            Entity.BaseResponse<Response.ConfgurationResponse> response = new Entity.BaseResponse<Response.ConfgurationResponse>(true);
            try
            {
                response.Data = _configurationService.GetConfguration(key);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Response.ConfgurationResponse>(false, ex.Message);
            }
            return response;
        }
    }
}