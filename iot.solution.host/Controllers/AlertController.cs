using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;
using host.iot.solution.Controllers;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Text;
using component.helper;

namespace iot.solution.host.Controllers
{
    [Route(AlertRoute.Route.Global)]
    [ApiController]
    public class AlertController : BaseController
    {
        private readonly IRuleService _ruleService;
        private readonly IFleetService _fleetService;
        private readonly IChartService _chartService;
        public AlertController(IFleetService fleetService,IRuleService ruleService, IChartService chartService)
        {
            _ruleService = ruleService;
            _fleetService = fleetService;
            _chartService = chartService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route(AlertRoute.Route.Manage, Name = AlertRoute.Name.Manage)]
        public Entity.BaseResponse<bool> Manage()
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    string strRequest = reader.ReadToEndAsync().Result;
                    Entity.IOTAlertMessage objRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<Entity.IOTAlertMessage>(strRequest);
                    if (!string.IsNullOrWhiteSpace(strRequest))
                    {

                        if (objRequest.ruleName == "Radius")
                        {
                            Entity.BaseResponse<Entity.FleetDetail> fleetResponse = new Entity.BaseResponse<Entity.FleetDetail>(true);

                            fleetResponse.Data = _fleetService.GetByDevice(Guid.Parse(objRequest.deviceGuid));
                            //{ "message":"Radius matched for the JDevice002","companyGuid":"ffa821af-fdd6-477e-a823-53f0cfd606a5","condition":"gps_lat !=0 AND gps_lng !=0","conditionValue":{ "gps_lat":23.026965938024329,"gps_lng":72.574639537978854},"deviceGuid":"5b70d597-6b5a-40c2-b0d9-c04a9e7a0bd0","entityGuid":"b3e7b7b0-df51-4ab0-96d1-c0eb4546d162","eventDate":"2020-10-12T10:22:28.000311Z","uniqueId":"JDevice002","audience":"[{\"emailId\":\"SFM1stOct@mailinator.com\",\"roleGuid\":\"9B0355BB-309A-45AE-902E-63D3C43D9A69\",\"userGuid\":\"F7581CC9-9D83-4534-B399-A449C2B5D575\"}]","eventId":601,"refGuid":"c488c8cc-74ca-44e0-ba03-021cf7c807eb","severity":"Information","ruleName":"Radius","data":"[{\"id\":\"JDevice002\",\"d\":[{\"gps_lat\":\"23.02696593802433\",\"gps_lng\":\"72.57463953797885\",\"gps_altitude\":\"25\",\"gps_num_sats\":\"28\",\"vehicle_ign_sense\":\"29\",\"gateway_uptime\":\"27\",\"can_vehicle_speed\":\"50.00\",\"can_engine_rpm\":\"7500.00\",\"can_fuel_level\":\"7\",\"can_odometer\":\"27\",\"can_hours_operation\":\"27\",\"can_engine_rpm_total\":\"27\",\"can_distance_to_service\":\"26\",\"can_diagnostic_error_mesg\":\"25\",\"can_tyrepressure\":\"30\",\"can_enginetemp\":\"26\",\"can_currentin\":\"26\"}],\"dt\":\"2020-10-12T15:52:23.0560000+05:30\",\"tg\":\"\"}]"}
                            if (fleetResponse != null && fleetResponse.Data != null)
                            {

                                Entity.IOTAlertRadiusMessage objLatLongRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<Entity.IOTAlertRadiusMessage>(objRequest.conditionValue.ToString());
                                if (fleetResponse.Data.Radius.HasValue && objLatLongRequest != null && !string.IsNullOrEmpty(objLatLongRequest.gps_lat) && !string.IsNullOrEmpty(objLatLongRequest.gps_lng))
                                {
                                    double radiusInKM = GetDistance(Convert.ToDouble(fleetResponse.Data.Latitude), Convert.ToDouble(fleetResponse.Data.Longitude), Convert.ToDouble(objLatLongRequest.gps_lat), Convert.ToDouble(objLatLongRequest.gps_lng));
                                    double fleetRadiusInKM = Convert.ToDouble(fleetResponse.Data.Radius.Value)/1000;
                                   // radiusInKM % fleetRadiusInKM
                                    if (radiusInKM > fleetRadiusInKM)
                                    {
                                        //set next  notification only after 30minutes of last notification
                                        var lastAlert = _ruleService.AlertList(new Entity.AlertRequest()
                                        {
                                            DeviceGuid = objRequest.deviceGuid,
                                            OrderBy = "eventDate desc",
                                            PageNumber = -1,
                                            PageSize = -1                                           
                                        });
                                        double duration = 0;
                                        double limit = 0;
                                        if (lastAlert != null && lastAlert.Count > 0)
                                        {
                                            var alertResponse = lastAlert.Items.Where(t => t.RuleName == objRequest.ruleName).OrderByDescending(t => t.EventDate).FirstOrDefault();
                                            if (alertResponse != null)
                                            {
                                                duration = (objRequest.eventDate - alertResponse.EventDate).TotalMinutes;// TimeSpan.FromMilliseconds((objRequest.eventDate - alertResponse.EventDate).TotalMilliseconds).Minutes;
                                                limit = TimeSpan.FromMinutes(double.Parse(SolutionConfiguration.Configuration.EmailTemplateSettings.FleetRadiusDurationMinutes)).Minutes;
                                                if (duration >= 0 && duration <= limit)
                                                {
                                                    response.IsSuccess = true;
                                                    return response;
                                                }

                                            }
                                        }
                                        objRequest.message = "`" + fleetResponse.Data.FleetId + "` is out of its geofence radius of " + fleetRadiusInKM + " KM. " + duration + "###" + limit;
                                        objRequest.conditionValue = objRequest.conditionValue.ToString();
                                        XmlSerializer xsSubmit = new XmlSerializer(typeof(Entity.IOTAlertMessage));
                                        string xml = "";
                                        using (var sww = new StringWriter())
                                        {
                                            using (XmlWriter writer = XmlWriter.Create(sww))
                                            {
                                                xsSubmit.Serialize(writer, objRequest);
                                                xml = sww.ToString();
                                            }
                                        }
                                        _ruleService.ManageWebHook(xml);
                                        if (Convert.ToBoolean(component.helper.SolutionConfiguration.Configuration.HangFire.IsSendRadiusMailEnabled.ToString()))
                                        {
                                            _chartService.SendEmailNotification_Radius(fleetResponse.Data, radiusInKM);
                                        }

                                        //end



                                    }
                                }
                            }
                        }
                        else
                        {
                            XmlSerializer xsSubmit = new XmlSerializer(typeof(Entity.IOTAlertMessage));
                            string xml = "";
                            objRequest.conditionValue = objRequest.conditionValue.ToString();
                            using (var sww = new StringWriter())
                            {
                                using (XmlWriter writer = XmlWriter.Create(sww))
                                {
                                    xsSubmit.Serialize(writer, objRequest);
                                    xml = sww.ToString();
                                }
                            }

                            _ruleService.ManageWebHook(xml);
                        }
                        response.IsSuccess = true;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }
        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radius of the earth in km
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distance in km
            return d;
        }

        private double ToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }
        [HttpPost]
        [Route(AlertRoute.Route.List, Name = AlertRoute.Name.List)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.AlertResponse>>> GetBySearch([FromBody] Entity.AlertListRequest request)
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.AlertResponse>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.AlertResponse>>>(true);
            try
            {
                response.Data = _ruleService.AlertList(new Entity.AlertRequest()
                {
                    //DeviceGuid = deviceGuid,                    
                    EntityGuid =request.fleetGuid,
                    OrderBy = request.orderBy,
                    PageNumber = request.pageNo,
                    PageSize = request.pageSize,
                    CurrentDate = request.currentDate,
                    TimeZone=request.timeZone
                });

            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.AlertResponse>>>(false, ex.Message);
            }
            return response;
        }
    }
}