using component.helper;
using component.logger;
using iot.solution.common;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Implementation
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IDeviceMaintenanceRepository _deviceMaintenanceRepository;
        private readonly ILogger _logger;
        private readonly IDeviceService _deviceService;
        public TripService(ITripRepository tripRepository, IDeviceMaintenanceRepository deviceMaintenanceRepository, ILogger logger, IDeviceService deviceService)
        {
            _logger = logger;
            _tripRepository = tripRepository;
            _deviceMaintenanceRepository = deviceMaintenanceRepository;
            _deviceService = deviceService;
        }

        //public List<Entity.Driver> Get()
        //{
        //    try
        //    {
        //        return _driverRepository.GetAll().Where(e => !e.IsDeleted).Select(p => Mapper.Configuration.Mapper.Map<Entity.Driver>(p)).ToList();
        //    }
        //    catch (Exception ex)
        //    {

        //        _logger.Error(Constants.ACTION_EXCEPTION, "EntityService.GetAll " + ex);
        //        return new List<Entity.Driver>();
        //    }
        //}
        public Entity.TripDetail Get(Guid id)
        {
            try
            {
                Entity.TripDetail response = _tripRepository.FindBy(x => x.Guid.Equals(id)).Select(p => Mapper.Configuration.Mapper.Map<Entity.TripDetail>(p)).FirstOrDefault();
                response.ShipmentFiles = _tripRepository.GetShipmentFiles(id);
                response.TripStops= _tripRepository.GetTripStops(id);
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "EntityService.Get " + ex);
                return null;
            }
        }
        public Entity.ActionStatus Manage(Entity.TripModel request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                if (request.tripStop == null || request.tripStop.Count == 0)
                {
                    actionStatus.Success = false;
                    actionStatus.Message = "Please add destination trip stop with trip End date";
                    return actionStatus;
                }
                var dbEntity = Mapper.Configuration.Mapper.Map<Entity.TripModel, Model.TripModel>(request);
                if (request.Guid == null || request.Guid == Guid.Empty)
                {                  
                    dbEntity.Guid = Guid.Empty;
                    dbEntity.CompanyGuid = SolutionConfiguration.CompanyId;
                    dbEntity.CreatedDate = DateTime.Now;
                    dbEntity.CreatedBy = SolutionConfiguration.CurrentUserId;
                    var tripStop = new List<stop>();
                    var xmlData = string.Empty;
                    DateTime dateValue;
                    if (DateTime.TryParse(request.StartDateTime.ToString(), out dateValue))
                    {
                        dbEntity.StartDateTime = dateValue.AddMinutes(-double.Parse(request.TimeZone));
                    }
                    if(request.tripStop.Count>0)
                    {

                       foreach(var item in request.tripStop)
                        {
                            if (item.guid == null || item.guid == Guid.Empty)
                            {
                                item.guid = Guid.NewGuid();
                            }
                            if (DateTime.TryParse(item.endDateTime.ToString(), out dateValue))
                            {
                                    item.endDateTime = dateValue.AddMinutes(-double.Parse(request.TimeZone));
                            }
                        }
                    }
                    using (var stringwriter = new System.IO.StringWriter())
                    {
                        var serializer = new XmlSerializer(request.tripStop.GetType());
                        serializer.Serialize(stringwriter, request.tripStop);
                        xmlData = stringwriter.ToString().Replace("ArrayOfStop", "stops");
                    }
                    dbEntity.stopData = xmlData;
                    actionStatus = _tripRepository.Manage(dbEntity);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Trip, Entity.Trip>(actionStatus.Data);
                    if (!actionStatus.Success)
                    {
                        _logger.Error($"Fleet is not added in solution database, Error: {actionStatus.Message}");
                        actionStatus.Success = false;
                    }
                    else
                    {
                        //upload multiple images
                        if (request.ShipmentFiles != null && request.ShipmentFiles.Count > 0)
                        {
                            UploadFiles(request.ShipmentFiles, actionStatus.Data.Guid.ToString(), "S");
                        }
                    }
                }
                else
                {
                    var olddbEntity = _tripRepository.FindBy(x => x.Guid.Equals(request.Guid)).FirstOrDefault();
                    if (olddbEntity == null)
                    {
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Fleet");
                    }

                    dbEntity.UpdatedDate = DateTime.Now;
                    dbEntity.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    dbEntity.CompanyGuid = SolutionConfiguration.CompanyId;
                    var tripStop = new List<stop>();
                    var xmlData = string.Empty;
                    DateTime dateValue;
                    if (DateTime.TryParse(request.StartDateTime.ToString(), out dateValue))
                    {
                        dbEntity.StartDateTime = dateValue.AddMinutes(-double.Parse(request.TimeZone));
                    }
                    if (request.tripStop.Count > 0)
                    {

                        foreach (var item in request.tripStop)
                        {
                            if (item.guid == null || item.guid == Guid.Empty)
                            {
                                item.guid = Guid.NewGuid();
                            }
                            if (DateTime.TryParse(item.endDateTime.ToString(), out dateValue))
                            {
                                item.endDateTime = dateValue.AddMinutes(-double.Parse(request.TimeZone));
                            }
                        }
                    }
                    using (var stringwriter = new System.IO.StringWriter())
                    {
                        var serializer = new XmlSerializer(request.tripStop.GetType());
                        serializer.Serialize(stringwriter, request.tripStop);
                        xmlData = stringwriter.ToString().Replace("ArrayOfStop", "stops");
                    }
                    dbEntity.stopData = xmlData;
                    actionStatus = _tripRepository.Manage(dbEntity);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Trip, Entity.Trip>(dbEntity);
                    if (!actionStatus.Success)
                    {
                        _logger.Error($"Fleet is not updated in solution database, Error: {actionStatus.Message}");
                        actionStatus.Success = false;

                    }
                    else
                    {
                        //upload multiple images
                        if (request.ShipmentFiles != null && request.ShipmentFiles.Count > 0)
                        {
                            UploadFiles(request.ShipmentFiles, actionStatus.Data.Guid.ToString(), "S");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "FleetService.Manage " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }

        private String ObjectToXMLGeneric<T>(T filter)
        {

            string xml = null;
            using (StringWriter sw = new StringWriter())
            {

                XmlSerializer xs = new XmlSerializer(typeof(T));
                xs.Serialize(sw, filter);
                try
                {
                    xml = sw.ToString();

                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return xml;
        }
        public Entity.ActionStatus UploadFiles(List<IFormFile> files, string tripId, string type)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                if (files.Count > 0)
                {
                    List<file> lstFileUploaded = new List<file>();
                    System.Text.StringBuilder strFileNotUploaded = new System.Text.StringBuilder();
                    foreach (var formFile in files)
                    {
                        file obj = new file();

                        string filePath = SaveShipmentImage(Guid.NewGuid(), formFile);
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            obj.path = filePath.ToString();
                            obj.type = type;
                            obj.desc = Path.GetFileNameWithoutExtension(formFile.FileName);
                            lstFileUploaded.Add(obj);
                        }
                        else
                        {
                            strFileNotUploaded.Append(formFile.FileName + " is invalid! ");
                        }
                    }
                    if (lstFileUploaded.Count > 0)
                    {
                        var xmlfiles = ObjectToXMLGeneric<List<file>>(lstFileUploaded);
                        xmlfiles = xmlfiles.Replace("ArrayOfFile", "files");
                        actionStatus = _tripRepository.UploadFiles(xmlfiles, tripId);
                    }
                    else
                    {
                        actionStatus.Success = false;
                        actionStatus.Message = strFileNotUploaded.ToString();
                    }
                }
                else
                {
                    actionStatus.Success = false;
                    actionStatus.Message = "Something Went Wrong!";
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "GeneratorService.UploadFiles " + ex);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            return actionStatus;
        }


        public Entity.ActionStatus SaveStops(string tripStopsXml, string tripId)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                   if (!string.IsNullOrEmpty(tripStopsXml))
                   {
                        actionStatus = _tripRepository.AddTripStops(tripStopsXml, tripId);
                   }
                    else
                    {
                        actionStatus.Success = false;
                        actionStatus.Message = "Somthing Went Wrong";
                    }
               
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "GeneratorService.UploadFiles " + ex);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            return actionStatus;
        }
        // Saving Image on Server   
        private string SaveShipmentImage(Guid guid, IFormFile image)
        {
            var fileBasePath = SolutionConfiguration.UploadBasePath + SolutionConfiguration.ShipmentFilePath;
            bool exists = System.IO.Directory.Exists(fileBasePath);
            if (!exists)
                System.IO.Directory.CreateDirectory(fileBasePath);
            string extension = Path.GetExtension(image.FileName);
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string fileName = guid.ToString() + "_" + unixTimestamp;

            var filePath = Path.Combine(fileBasePath, fileName + extension);
            if (image != null && image.Length > 0)
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
                return Path.Combine(SolutionConfiguration.CompanyFilePath, fileName + extension);
            }
            return null;
        }

        public Entity.ActionStatus Delete(Guid id)
        {
            try
            {
                Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
                var dbTrip = _tripRepository.GetByUniqueId(x => x.Guid == id);
                if (dbTrip == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Trip");
                }
                return _tripRepository.Delete(id);
                //bool status = _tripRepository.GetTripStatus(id);
                //if(status)
                //{
                //    dbTrip.IsDeleted = true;
                //    dbTrip.UpdatedDate = DateTime.Now;
                //    dbTrip.UpdatedBy = SolutionConfiguration.CurrentUserId;
                //    return _tripRepository.Update(dbTrip);
                //}
                //else
                //{
                //    actionStatus.Message = "Trip is on going so it can not be deleted !!";
                //    actionStatus.Success = false;
                //    actionStatus.Data = "";
                //    return actionStatus;
                //}


            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public Entity.ActionStatus DeleteShipmentFile(Guid tripId, Guid? fileId)
        {

            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbMediaFile = _tripRepository.FindBy(x => x.Guid.Equals(tripId)).FirstOrDefault();
                if (dbMediaFile == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : ShipmentFile");
                }
                return _tripRepository.DeleteShipmentFile(tripId, fileId);
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "GeneratorService.DeleteMediaFile " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.SearchResult<List<Entity.TripListItem>> List(Entity.SearchRequest request)
        {
            try
            {
                Entity.SearchResult<List<Entity.TripListItem>> result = _tripRepository.List(request);
                return new Entity.SearchResult<List<Entity.TripListItem>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.TripListItem>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"Trip.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.TripListItem>>();
            }
        }

        public Entity.ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbEntity = _tripRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbEntity == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Entity");
                }

               
                dbEntity.IsActive = status;
                dbEntity.UpdatedDate = DateTime.Now;
                dbEntity.UpdatedBy = SolutionConfiguration.CurrentUserId;
                return _tripRepository.Update(dbEntity);
               
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "FleetService.UpdateStatus " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }

        public Entity.ActionStatus UpdateTripStatus(Entity.TripUpdateStatus request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbEntity = _tripRepository.FindBy(x => x.Guid.Equals(Guid.Parse(request.tripGuid)) && x.IsStarted && !x.IsCompleted).FirstOrDefault();
                if (dbEntity == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Entity");
                }
                DateTime dateValue;
                if (DateTime.TryParse(request.currentDate.ToString(), out dateValue))
                {
                    dateValue = dateValue.AddMinutes(-double.Parse(request.timeZone));
                }
                dbEntity.IsCompleted = true;
                dbEntity.CoveredMiles = request.coveredMiles;
                dbEntity.UpdatedDate = DateTime.Now;
                dbEntity.CompletedDate = dateValue;
                dbEntity.UpdatedBy = SolutionConfiguration.CurrentUserId;
                return _tripRepository.Update(dbEntity);
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "FleetService.UpdateStatus " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }

        public Entity.ActionStatus StartTrip(Entity.StartTripModal request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbEntity = _tripRepository.FindBy(x => x.Guid.Equals(Guid.Parse(request.tripGuid))).FirstOrDefault();
                if (dbEntity == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Trip");
                }
                DateTime dateValue;
                if (DateTime.TryParse(request.etaEndDateTime.ToString(), out dateValue))
                {
                    request.etaEndDateTime = dateValue.AddMinutes(-double.Parse(request.timeZone));
                }
                if (DateTime.TryParse(request.currentDate.ToString(), out dateValue))
                {
                    request.currentDate = dateValue.AddMinutes(-double.Parse(request.timeZone));
                }
                request.fleetGuid = dbEntity.FleetGuid;
                var result = _tripRepository.StartTrip(request);
                if(result.Success)
                {
                    actionStatus.Success = true;
                    actionStatus.Message = result.Message;
                }
                else
                {
                    actionStatus.Success = false;
                    actionStatus.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "FleetService.UpdateStatus " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.BaseResponse<Entity.TripDashboardOverviewResponse> GetTripDetail(Guid tripGuid, DateTime currentDate, string timeZone)
        {
            Entity.BaseResponse<List<Entity.TripDashboardOverviewResponse>> listResult = new Entity.BaseResponse<List<Entity.TripDashboardOverviewResponse>>();
            Entity.BaseResponse<Entity.TripDashboardOverviewResponse> result = new Entity.BaseResponse<Entity.TripDashboardOverviewResponse>(true);
            try
            {
                listResult = _tripRepository.GetStatistics(tripGuid, currentDate, timeZone);
                if(listResult.Data[0].DeviceGuid !=null)
                {
                    Guid deviceId =(Guid) listResult.Data[0].DeviceGuid;

                    var deviceData = _deviceService.GetTelemetryData(deviceId);
                    if(deviceData.Data.Count>0 )
                    {
                        if (listResult.Data[0].TripStatus != "Upcoming")
                        {
                            foreach (var data in deviceData.Data.ToList())
                            {
                                if (data.attributeName.Equals("can_fuel_level"))
                                {
                                    listResult.Data[0].FuelLevel = data.attributeValue;
                                }
                                if (data.attributeName.Equals("can_tyrepressure"))
                                {
                                    listResult.Data[0].TyrePressure = data.attributeValue;
                                }
                                //if (data.attributeName.Equals("remainingoil"))
                                //{
                                //    listResult.Data[0].Oil = data.attributeValue;
                                //}
                                if (data.attributeName.Equals("can_vehicle_speed"))
                                {
                                    listResult.Data[0].CurrentSpeed = data.attributeValue;
                                }
                                if (data.attributeName.Equals("can_enginetemp"))
                                {
                                    listResult.Data[0].EngineTemp = data.attributeValue;
                                }
                            }
                        }
                        else {
                            listResult.Data[0].FuelLevel = "0";
                            listResult.Data[0].CurrentSpeed = "0";
                            listResult.Data[0].TyrePressure = "0";
                            listResult.Data[0].EngineTemp = "0";
                        }
                    }
                    
                }
                if (listResult.Data.Count > 0)
                {
                    result.IsSuccess = true;
                    result.Data = listResult.Data[0];
                    result.LastSyncDate = listResult.LastSyncDate;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}
