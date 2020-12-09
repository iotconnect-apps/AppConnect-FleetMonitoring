using host.iot.solution.Filter;

using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;

namespace host.iot.solution.Controllers
{
    [Route(TripRoute.Route.Global)]
    [ApiController]
    public class TripController : BaseController
    {
        private readonly ITripService _service;
        public TripController(ITripService tripService)
        {
            _service = tripService;
        }


        [HttpGet]
        [Route(TripRoute.Route.GetById, Name = TripRoute.Name.GetById)]
        [EnsureGuidParameter("id", "Trip")]
        public Entity.BaseResponse<Entity.TripDetail> Get(string id)
        {
            if (id == null || id == string.Empty)
            {
                return new Entity.BaseResponse<Entity.TripDetail>(false, "Trip id required!");
            }

            Entity.BaseResponse<Entity.TripDetail> response = new Entity.BaseResponse<Entity.TripDetail>(true);
            try
            {
                response.Data = _service.Get(Guid.Parse(id));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.TripDetail>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(TripRoute.Route.Manage, Name = TripRoute.Name.Add)]
        public Entity.BaseResponse<Entity.Trip> Manage([FromForm] Entity.TripModel request)
        {
            Entity.BaseResponse<Entity.Trip> response = new Entity.BaseResponse<Entity.Trip>(false);
            try
            {
                var status = _service.Manage(request);
                if (status.Success)
                {
                    response.IsSuccess = status.Success;
                    response.Message = status.Message;
                    response.Data = status.Data;
                }
                else
                {
                    response.IsSuccess = status.Success;
                    response.Message = status.Message;
                }
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.Trip>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(TripRoute.Route.Delete, Name = TripRoute.Name.Delete)]
        [EnsureGuidParameter("id", "Trip")]
        public Entity.BaseResponse<bool> Delete(string id)
        {
            if (id == null || id == string.Empty)
            {
                return new Entity.BaseResponse<bool>(false, "Fleet id required!");
            }

            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.Delete(Guid.Parse(id));
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Success;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }


        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [HttpPut]
        [Route(TripRoute.Route.ShipmentFile, Name = TripRoute.Name.DeletePermissionFile)]
        [EnsureGuidParameterAttribute("tripId", "Trip")]
        public Entity.BaseResponse<bool> DeleteShipmentFile(string tripId, Guid? fileId)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.DeleteShipmentFile(Guid.Parse(tripId), fileId);
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Success;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route(TripRoute.Route.BySearch, Name = TripRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.TripListItem>>> GetBySearch([FromBody]Entity.TripListRequest request)
        {
            //string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "")
            Entity.BaseResponse<Entity.SearchResult<List<Entity.TripListItem>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.TripListItem>>>(true);
            try
            {
                if (!request.currentDate.HasValue)
                {
                    return new Entity.BaseResponse<Entity.SearchResult<List<Entity.TripListItem>>>(false, "Current Date is required");
                }
                else if (string.IsNullOrEmpty(request.timeZone))
                {
                    return new Entity.BaseResponse<Entity.SearchResult<List<Entity.TripListItem>>>(false, "Time Zone is required");
                }
                else
                {
                    response.Data = _service.List(new Entity.SearchRequest()
                    {
                        DriverGuid = request.driverGuid,
                        FleetGuid = request.fleetGuid,
                        SearchText = request.searchText,
                        PageNumber = request.pageNo.Value,
                        PageSize = request.pageSize.Value,
                        OrderBy = request.orderBy,
                        Status = request.status,
                        CurrentDate = request.currentDate,
                        TimeZone = request.timeZone,
                        StartDate=request.startDate,
                        EndDate=request.endDate
                    });
                }
                
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.TripListItem>>>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route(TripRoute.Route.UpdateStatus, Name = TripRoute.Name.UpdateStatus)]
        [EnsureGuidParameter("id", "Trip")]
        public Entity.BaseResponse<bool> UpdateStatus(string id, bool status)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                Entity.ActionStatus result = _service.UpdateStatus(Guid.Parse(id), status);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Success;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(TripRoute.Route.UpdateTripStatus, Name = TripRoute.Name.UpdateTripStatus)]
        [EnsureGuidParameter("TripGuid", "Trip")]
        public Entity.BaseResponse<bool> UpdateTripStatusCompleted(Entity.TripUpdateStatus request)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                if (request.tripGuid == null || request.tripGuid == string.Empty)
                {
                    return new Entity.BaseResponse<bool>(false, "Trip Id is required");
                }
                else if (!request.currentDate.HasValue)
                {
                    return new Entity.BaseResponse<bool>(false, "Current Date is required");
                }
                else if (string.IsNullOrEmpty(request.timeZone))
                {
                    return new Entity.BaseResponse<bool>(false, "Time Zone is required");
                }
                else
                {
                    Entity.ActionStatus result = _service.UpdateTripStatus(request);
                    response.IsSuccess = result.Success;
                    response.Message = result.Message;
                    response.Data = result.Success;
                }
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route(TripRoute.Route.StartTrip, Name = TripRoute.Name.StartTrip)]
        [EnsureGuidParameter("TripGuid", "Trip")]
        public Entity.BaseResponse<bool> StartTrip(Entity.StartTripModal request)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                if (request.tripGuid == null || request.tripGuid == string.Empty)
                {
                    return new Entity.BaseResponse<bool>(false, "Trip Id is required");
                }
                else if (!request.currentDate.HasValue)
                {
                    return new Entity.BaseResponse<bool>(false, "Current Date is required");
                }
                else if (!request.etaEndDateTime.HasValue)
                {
                    return new Entity.BaseResponse<bool>(false, "Estimated end date is required");
                }
                else if (string.IsNullOrEmpty(request.timeZone))
                {
                    return new Entity.BaseResponse<bool>(false, "Time Zone is required");
                }
                
                else
                {
                    Entity.ActionStatus result = _service.StartTrip(request);
                    response.IsSuccess = result.Success;
                    response.Message = result.Message;
                    response.Data = result.Success;
                }
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }
    }
}