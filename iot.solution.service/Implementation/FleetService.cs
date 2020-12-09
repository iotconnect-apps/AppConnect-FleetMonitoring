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
using System.Xml.Serialization;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Implementation
{
    public class FleetService : IFleetService
    {
        private readonly IFleetRepository _fleetRepository;
        private readonly IDeviceMaintenanceRepository _maintenanceRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IotConnectClient _iotConnectClient;
        private readonly ILogger _logger;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IDeviceTypeRepository _deviceTypeRepository;
        private readonly IDeviceService _deviceService;

        public FleetService(IFleetRepository entityRepository, ITripRepository tripRepository, IDeviceMaintenanceRepository maintenanceRepository,ILogger logger, IDeviceRepository deviceRepository, IDeviceService deviceService, IDeviceTypeRepository deviceTypeRepository)
        {
            _logger = logger;
            _fleetRepository = entityRepository;
            _tripRepository = tripRepository;
            _deviceRepository = deviceRepository;
            _deviceService = deviceService;
            _deviceTypeRepository = deviceTypeRepository;
            _maintenanceRepository = maintenanceRepository;
            _iotConnectClient = new IotConnectClient(SolutionConfiguration.BearerToken, SolutionConfiguration.Configuration.EnvironmentCode, SolutionConfiguration.Configuration.SolutionKey);
        }
        
        public Entity.FleetDetail Get(Guid id)
        {
            try
            {
                Entity.FleetDetail response = _fleetRepository.FindBy(r => r.Guid == id).Select(p => Mapper.Configuration.Mapper.Map<Entity.FleetDetail>(p)).FirstOrDefault();
                if (response != null)
                {
                    response.FleetPermissionFiles = _deviceRepository.GetMediaFiles(id, "P");
                   
                    response.Devices = _deviceService.GetFleetWiseDevices(id);
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "FleetService.Get " + ex);
                return null;
            }
        }
        public Entity.FleetDetail GetByDevice(Guid deviceId)
        {
            try
            {
                Entity.Device dbDevice = _deviceRepository.FindBy(r => r.Guid == deviceId).Select(p => Mapper.Configuration.Mapper.Map<Entity.Device>(p)).FirstOrDefault();
                Entity.FleetDetail response = null;
                if (dbDevice != null)
                {
                     response = _fleetRepository.FindBy(r => r.Guid == dbDevice.FleetGuid).Select(p => Mapper.Configuration.Mapper.Map<Entity.FleetDetail>(p)).FirstOrDefault();
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "FleetService.Get " + ex);
                return null;
            }
        }
        public Entity.ActionStatus Manage(Entity.FleetModel request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbEntity = Mapper.Configuration.Mapper.Map<Entity.FleetModel, Model.FleetModel>(request);
                if (request.Guid == null || request.Guid == Guid.Empty)
                {                  
                    if (request.ImageFile != null)
                    {
                        // upload image                                     
                        dbEntity.Image = SaveFleetImage(request.Guid, request.ImageFile);
                    }
                    dbEntity.Guid = Guid.Empty;
                    dbEntity.CompanyGuid = SolutionConfiguration.CompanyId;
                    dbEntity.CreatedDate = DateTime.Now;
                    dbEntity.CreatedBy = SolutionConfiguration.CurrentUserId;
                    /*
                        * <deviceInfos><deviceInfo><templateGuid>12A5CD86-F6C6-455F-B27A-EFE587ED410D</templateGuid><deviceGuid>12A5CD86-F6C6-455F-B27A-EFE587ED410D</deviceGuid></deviceInfo>
                                       
                               </deviceInfos>
                        */

                    var devices = new List<deviceInfo>();
                    var xmlData = string.Empty;
                   
                    using (var stringwriter = new System.IO.StringWriter())
                    {
                        var serializer = new XmlSerializer(request.devices.GetType());
                        serializer.Serialize(stringwriter, request.devices);
                        xmlData = stringwriter.ToString().Replace("ArrayOfDeviceInfo", "deviceInfos");
                    }
                    dbEntity.deviceData = xmlData;
                    actionStatus = _fleetRepository.Manage(dbEntity);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Fleet, Entity.Fleet>(actionStatus.Data);
                    if (!actionStatus.Success)
                    {
                        _logger.Error($"Fleet is not added in solution database, Error: {actionStatus.Message}");

                        actionStatus.Success = false;

                    }
                    else
                    {
                        dbEntity.Guid = actionStatus.Data.Guid;
                        //upload multiple images
                        if (request.PermissionFiles != null && request.PermissionFiles.Count > 0)
                        {
                            UploadFiles(request.PermissionFiles, dbEntity.Guid.ToString(), "P");
                        }
                        
                    }
                }
                else
                {
                    var olddbEntity = _fleetRepository.FindBy(x => x.Guid.Equals(request.Guid)).FirstOrDefault();
                    if (olddbEntity == null)
                    {
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Fleet");
                    }


                    string existingImage = olddbEntity.Image;
                   // var dbEntity = Mapper.Configuration.Mapper.Map(request, olddbEntity);
                    if (request.ImageFile != null)
                    {
                        if (File.Exists(SolutionConfiguration.UploadBasePath + dbEntity.Image) && request.ImageFile.Length > 0)
                        {
                            //if already exists image then delete  old image from server
                            File.Delete(SolutionConfiguration.UploadBasePath + dbEntity.Image);
                        }
                        if (request.ImageFile.Length > 0)
                        {
                            // upload new image                                     
                            dbEntity.Image = SaveFleetImage(request.Guid, request.ImageFile);
                        }
                    }
                    else
                    {
                        dbEntity.Image = existingImage;
                    }
                    dbEntity.UpdatedDate = DateTime.Now;
                    dbEntity.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    dbEntity.CompanyGuid = SolutionConfiguration.CompanyId;
                    

                    var devices = new List<deviceInfo>();
                    var xmlData = string.Empty;

                    using (var stringwriter = new System.IO.StringWriter())
                    {
                        var serializer = new XmlSerializer(request.devices.GetType());
                        serializer.Serialize(stringwriter, request.devices);
                        xmlData = stringwriter.ToString().Replace("ArrayOfDeviceInfo", "deviceInfos");
                    }
                    dbEntity.deviceData = xmlData;
                    actionStatus = _fleetRepository.Manage(dbEntity);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Fleet, Entity.Fleet>(dbEntity);
                    if (!actionStatus.Success)
                    {
                        _logger.Error($"Fleet is not updated in solution database, Error: {actionStatus.Message}");
                        actionStatus.Success = false;

                    }
                    else
                    {
                        //upload multiple images
                        if (request.PermissionFiles != null && request.PermissionFiles.Count > 0)
                        {
                            UploadFiles(request.PermissionFiles, request.Guid.ToString(), "P");
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
        public Entity.ActionStatus UploadFiles(List<IFormFile> files, string deviceId, string type)
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

                        string filePath = SaveFleetImage(Guid.NewGuid(), formFile);
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
                        actionStatus = _deviceRepository.UploadFiles(xmlfiles, deviceId);
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

        // Saving Image on Server   
        private string SaveFleetImage(Guid guid, IFormFile image)
        {
            var fileBasePath = SolutionConfiguration.UploadBasePath + SolutionConfiguration.FleetFilePath;
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
                return Path.Combine(SolutionConfiguration.FleetFilePath, fileName + extension);
            }
            return null;
        }
        public Entity.ActionStatus Delete(DateTime? currentDate,string timeZone,Guid id)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbEntity = _fleetRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbEntity == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Fleet");
                }
                return _fleetRepository.Delete(new Entity.FleetDeleteModel() { guid = id, currentDate = currentDate, timeZone = timeZone });

            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "Fleet.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        // Delete Image on Server   
        private bool DeleteFleetImage(Guid guid, string imageName)
        {
            var fileBasePath = SolutionConfiguration.UploadBasePath + SolutionConfiguration.FleetFilePath;
            var filePath = Path.Combine(fileBasePath, imageName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return true;
        }
        public Entity.ActionStatus DeleteImage(Guid id)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(false);
            try
            {
                var dbEntity = _fleetRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbEntity == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Entity");
                }

                bool deleteStatus = DeleteFleetImage(id, dbEntity.Image);
                if (deleteStatus)
                {
                    dbEntity.Image = "";
                    dbEntity.UpdatedDate = DateTime.Now;
                    dbEntity.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    dbEntity.CompanyGuid = SolutionConfiguration.CompanyId;

                    actionStatus = _fleetRepository.Update(dbEntity);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Fleet, Entity.Fleet>(dbEntity);
                    actionStatus.Success = true;
                    actionStatus.Message = "Image deleted successfully!";
                    if (!actionStatus.Success)
                    {
                        _logger.Error($"Entity is not updated in database, Error: {actionStatus.Message}");
                        actionStatus.Success = false;
                        actionStatus.Message = actionStatus.Message;
                    }
                }
                else
                {
                    actionStatus.Success = false;
                    actionStatus.Message = "Image not deleted!";
                }
                return actionStatus;
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "FleetManager.DeleteImage " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.ActionStatus DeletePermissionFile(Guid fleetId, Guid? fileId)
        {

            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbMediaFile = _fleetRepository.FindBy(x => x.Guid.Equals(fleetId)).FirstOrDefault();
                if (dbMediaFile == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : MediaFile");
                }
                return _deviceRepository.DeleteMediaFiles(fleetId, fileId);
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "GeneratorService.DeleteMediaFile " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.SearchResult<List<Entity.FleetListItem>> List(Entity.SearchRequest request)
        {
            try
            {
                var result = _fleetRepository.List(request);
                Entity.SearchResult<List<Entity.FleetListItem>> response = new Entity.SearchResult<List<Entity.FleetListItem>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.FleetListItem>(p)).ToList(),
                    Count = result.Count
                };
                //var templates = _iotConnectClient.Template.All(new IoTConnect.Model.PagingModel() { PageNo = 1, PageSize = 1000 }).Result;
                //foreach (var entity in response.Items)
                //{                   
                //    if (templates != null && templates.data != null && templates.data.Any())
                //    {
                //        string[] templateGuids = entity.TemplateGuid.Split(',');
                //        for (int i = 0; i < templateGuids.Length; i++)
                //        {
                //            string templateGuid = templateGuids[i].ToUpper();
                //            if (templateGuid != Guid.Empty.ToString())
                //            {
                //                var template = templates.data.Where(t => t.Guid.ToUpper().Equals(templateGuid)).FirstOrDefault();
                //                if (template != null)
                //                    entity.TemplateName += " " + template.Name+",";
                //            }
                //        }
                        
                //        entity.TemplateName = !string.IsNullOrEmpty(entity.TemplateName) ? entity.TemplateName.Trim(','):"";

                //    }                                   
                //}
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"FleetService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.FleetListItem>>();
            }
        }
        public Entity.SearchResult<List<Entity.FleetMapListItem>> MapList(Entity.SearchRequest request)
        {
            try
            {
                var result = _fleetRepository.MapList(request);
                Entity.SearchResult<List<Entity.FleetMapListItem>> response = new Entity.SearchResult<List<Entity.FleetMapListItem>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.FleetMapListItem>(p)).ToList(),
                    Count = result.Count
                };
               
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"FleetService.MapList, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.FleetMapListItem>>();
            }
        }
        public Entity.ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbEntity = _fleetRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbEntity == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Entity");
                }

               // var dbChildEntity = _fleetRepository.FindBy(x => x.ParentEntityGuid.Equals(id) && !x.IsDeleted).FirstOrDefault();
               // var dbDevice = _deviceRepository.FindBy(x => x.EntityGuid.Equals(id) || (dbChildEntity != null && x.EntityGuid.Equals(dbChildEntity.Guid))).FirstOrDefault();
               //var dbDeviceType = _deviceTypeRepository.FindBy(x => x.EntityGuid.Equals(id) && !x.IsDeleted).FirstOrDefault();
               // if (dbDevice == null && dbChildEntity == null && dbDeviceType==null)
               // {
                    dbEntity.IsActive = status;
                    dbEntity.UpdatedDate = DateTime.Now;
                    dbEntity.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    return _fleetRepository.Update(dbEntity);
                //}
                //else if (dbChildEntity != null)
                //{
                //    _logger.Error($"Fleet is not updated in solution database.Zone exists, Error: {actionStatus.Message}");
                //    actionStatus.Success = false;
                //    actionStatus.Message = "Fleet is not updated in solution database.Zone exists";
                //}
                //else if (dbDeviceType != null)
                //{
                //    _logger.Error($"Fleet is not updated in solution database.Device Type exists, Error: {actionStatus.Message}");
                //    actionStatus.Success = false;
                //    actionStatus.Message = "Fleet is not updated in solution database.Device Type exists";
                //}
                //else
                //{
                //    _logger.Error($"Fleet is not updated in solution database.Asset exists, Error: {actionStatus.Message}");
                //    actionStatus.Success = false;
                //    actionStatus.Message = "Fleet status cannot be updated because asset exists";
                //}

            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "FleetService.UpdateStatus " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }

        public Entity.BaseResponse<Entity.FleetDashboardOverviewResponse> GetFleetDetail(Guid fleetGuid, DateTime currentDate, string timeZone)
        {
            Entity.BaseResponse<List<Entity.FleetDashboardOverviewResponse>> listResult = new Entity.BaseResponse<List<Entity.FleetDashboardOverviewResponse>>();
            Entity.BaseResponse<Entity.FleetDashboardOverviewResponse> result = new Entity.BaseResponse<Entity.FleetDashboardOverviewResponse>(true);
            try
            {
                listResult = _fleetRepository.GetStatistics(fleetGuid, currentDate, timeZone);
                if (listResult.Data.Count > 0)
                {
                    result.IsSuccess = true;
                    result.Data = listResult.Data[0];
                    result.LastSyncDate = listResult.LastSyncDate;
                    Guid deviceGuid = (Guid)listResult.Data[0].DeviceGuid;
                    var deviceData = _deviceService.GetTelemetryData(deviceGuid);
                    if (deviceData.Data.Count > 0)
                    {
                        string can_engine_rpm_total = string.Empty;
                        string can_hours_operation = string.Empty;
                        decimal AverageRotationPerMin = 0;
                        foreach (var data in deviceData.Data.ToList())
                        {
                            if (data.attributeName.Equals("can_engine_rpm_total"))
                            {
                                can_engine_rpm_total = data.attributeValue;
                            }
                            if (data.attributeName.Equals("can_hours_operation"))
                            {
                                can_hours_operation = data.attributeValue;
                            }
                            if (!string.IsNullOrEmpty(can_engine_rpm_total) && !string.IsNullOrEmpty(can_hours_operation))
                            {
                                AverageRotationPerMin = (Convert.ToDecimal(can_engine_rpm_total) / Convert.ToDecimal(can_hours_operation))*60;
                                listResult.Data[0].AverageRotationPerMin = AverageRotationPerMin.ToString("0.##");
                            }
                        }                      
                    }
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
