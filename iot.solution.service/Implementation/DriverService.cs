using component.helper;
using component.logger;
using iot.solution.common;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Implementation
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _driverRepository;
        private readonly ITripRepository _tripRepository;
        private readonly ILogger _logger;
        public DriverService(IDriverRepository driverRepository, ITripRepository tripRepository, ILogger logger)
        {
            _logger = logger;
            _driverRepository = driverRepository;
            _tripRepository = tripRepository;
        }

        public List<Entity.Driver> Get()
        {
            try
            {
                return _driverRepository.GetAll().Where(e => !e.IsDeleted).Select(p => Mapper.Configuration.Mapper.Map<Entity.Driver>(p)).ToList();
            }
            catch (Exception ex)
            {

                _logger.Error(Constants.ACTION_EXCEPTION, "EntityService.GetAll " + ex);
                return new List<Entity.Driver>();
            }
        }
        public Entity.Driver Get(Guid id)
        {
            try
            {
                Entity.Driver response = _driverRepository.FindBy(r => r.Guid == id).Select(p => Mapper.Configuration.Mapper.Map<Entity.Driver>(p)).FirstOrDefault();
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "EntityService.Get " + ex);
                return null;
            }
        }
        public Entity.ActionStatus Manage(Entity.DriverModel request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                if (request.Guid == null || request.Guid == Guid.Empty)
                {
                            Entity.Driver flEntity = Mapper.Configuration.Mapper.Map<Entity.DriverModel, Entity.Driver>(request);
                            request.Guid = Guid.NewGuid();
                            var dbEntity = Mapper.Configuration.Mapper.Map<Entity.Driver, Model.Driver>(flEntity);
                            if (request.ImageFile != null)
                            {
                                // upload image                                     
                                dbEntity.Image = SaveDriverImage(request.Guid, request.ImageFile,"image");
                            }
                            if (request.LicenceFile != null)
                            {
                                // upload image                                     
                                dbEntity.LicenceImage = SaveDriverImage(request.Guid, request.LicenceFile,"licence");
                            }
                            dbEntity.Guid = request.Guid;
                            dbEntity.CompanyGuid = SolutionConfiguration.CompanyId;
                            dbEntity.CreatedDate = DateTime.Now;
                            dbEntity.CreatedBy = SolutionConfiguration.CurrentUserId;
                            
                            actionStatus = _driverRepository.Manage(dbEntity);
                            actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Driver, Entity.Driver>(actionStatus.Data);
                            if (!actionStatus.Success)
                            {
                                _logger.Error($"Driver is not added in solution database, Error: {actionStatus.Message}");
                            }
                }
                else
                {
                    
                    var olddbEntity = _driverRepository.FindBy(x => x.Guid.Equals(request.Guid)).FirstOrDefault();
                    if (olddbEntity == null)
                    {
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Driver");
                    }
                    bool IsMailExist = false;
                    bool IsLicenceExist = false;
                    if (!olddbEntity.Email.Equals(request.Email))
                    {
                        var checkExistingLicence = _driverRepository.FindBy(x => x.LicenceNo.Equals(request.LicenceNo) && x.Guid !=request.Guid && x.IsActive == true && !x.IsDeleted).FirstOrDefault();
                        var checkDriverMail = _driverRepository.FindBy(x => x.LicenceNo.Equals(request.Email) && x.Guid != request.Guid && x.IsActive == true && !x.IsDeleted).FirstOrDefault();
                        if (checkDriverMail != null)
                        {
                            IsMailExist = true;
                        }
                    }

                    if(!olddbEntity.LicenceNo.Equals(request.LicenceNo))
                    {
                        var checkExistingLicence = _driverRepository.FindBy(x => x.LicenceNo.Equals(request.LicenceNo) && x.Guid != request.Guid && x.IsActive == true && !x.IsDeleted).FirstOrDefault();
                        if (checkExistingLicence != null)
                        {
                            IsLicenceExist = true;
                        }
                    }

                    if(!IsMailExist && !IsLicenceExist)
                    {
                        Entity.Driver flEntity = Mapper.Configuration.Mapper.Map<Entity.DriverModel, Entity.Driver>(request);
                        string existingImage = olddbEntity.Image;
                        var dbEntity = Mapper.Configuration.Mapper.Map(flEntity, olddbEntity);
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
                                dbEntity.Image = SaveDriverImage(request.Guid, request.ImageFile, "image");
                            }
                        }
                        else
                        {
                            dbEntity.Image = existingImage;
                        }
                        string existingLicence = olddbEntity.LicenceImage;
                        if (request.LicenceFile != null)
                        {
                            if (File.Exists(SolutionConfiguration.UploadBasePath + dbEntity.LicenceImage) && request.LicenceFile.Length > 0)
                            {
                                //if already exists image then delete  old image from server
                                File.Delete(SolutionConfiguration.UploadBasePath + dbEntity.LicenceImage);
                            }
                            if (request.LicenceFile.Length > 0)
                            {
                                // upload new image                                     
                                dbEntity.LicenceImage = SaveDriverImage(request.Guid, request.LicenceFile, "licence");
                            }
                        }
                        else
                        {
                            dbEntity.LicenceImage = existingLicence;
                        }
                        dbEntity.UpdatedDate = DateTime.Now;
                        dbEntity.UpdatedBy = SolutionConfiguration.CurrentUserId;
                        dbEntity.CompanyGuid = SolutionConfiguration.CompanyId;

                        actionStatus = _driverRepository.Manage(dbEntity);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Driver, Entity.Driver>(dbEntity);
                        if (!actionStatus.Success)
                        {
                            _logger.Error($"Location is not updated in solution database, Error: {actionStatus.Message}");
                            actionStatus.Success = false;
                            actionStatus.Message = "Something Went Wrong!";
                        }
                    }
                    else
                    {
                        if (IsLicenceExist)
                        {
                            _logger.Error($"Driver Licence Already Exist !!");
                            actionStatus.Success = false;
                            actionStatus.Message = "Driver Licence Already Exist";
                        }
                        if (IsMailExist)
                        {
                            _logger.Error($"Driver Mail Id Already Exist !!");
                            actionStatus.Success = false;
                            actionStatus.Message = "Driver Mail Id Already Exist";
                        }
                    }
                   
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "EntityService.Manage " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }

        public Entity.ActionStatus Delete(Guid id)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(false);
            try
            {
                var dbDriver = _driverRepository.GetByUniqueId(x => x.Guid == id);
                if (dbDriver == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : HardwareKit");
                }
                var dbTrip = _tripRepository.FindBy(x => x.FleetGuid.Equals(dbDriver.FleetGuid) && !x.IsCompleted && !x.IsDeleted).FirstOrDefault();
                if (dbTrip == null)
                {
                    dbDriver.IsDeleted = true;
                    dbDriver.UpdatedDate = DateTime.Now;
                    dbDriver.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    return _driverRepository.Update(dbDriver);
                }
                else
                {
                    actionStatus.Success = false;
                    actionStatus.Message = "Driver can not be deleted trip exist for driver";
                    return actionStatus;
                }
               
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
        // Saving Image on Server   
        private string SaveDriverImage(Guid guid, IFormFile image,string imgType)
        {
            string folderPath = "";
            if(imgType.ToString()== "image")
            {
                folderPath = SolutionConfiguration.DriverImageFilePath;
            }
            if (imgType.ToString() == "licence")
            {
                folderPath = SolutionConfiguration.DriverLicenceFilePath;
            }
            var fileBasePath = SolutionConfiguration.UploadBasePath + folderPath;
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
                return Path.Combine(folderPath, fileName + extension);
            }
            return null;
        }

        //Delete Driver Image
        public Entity.ActionStatus DeleteImage(Guid id)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(false);
            try
            {
                var dbEntity = _driverRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbEntity == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Driver");
                }

                bool deleteStatus = DeleteImage(dbEntity.Image);
                if (deleteStatus)
                {
                    dbEntity.Image = "";
                    dbEntity.UpdatedDate = DateTime.Now;
                    dbEntity.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    dbEntity.CompanyGuid = SolutionConfiguration.CompanyId;

                    actionStatus = _driverRepository.Update(dbEntity);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Driver, Entity.Driver>(dbEntity);
                    actionStatus.Success = true;
                    actionStatus.Message = "Image deleted successfully!";
                    if (!actionStatus.Success)
                    {
                        _logger.Error($"Driver is not updated in database, Error: {actionStatus.Message}");
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
                _logger.Error(Constants.ACTION_EXCEPTION, "DriverManager.DeleteImage " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }

        //Delete Driver Licence
        public Entity.ActionStatus DeleteLicenceImage(Guid id)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(false);
            try
            {
                var dbEntity = _driverRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbEntity == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Entity");
                }

                bool deleteStatus = DeleteImage(dbEntity.LicenceImage);
                if (deleteStatus)
                {
                    dbEntity.LicenceImage = "";
                    dbEntity.UpdatedDate = DateTime.Now;
                    dbEntity.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    dbEntity.CompanyGuid = SolutionConfiguration.CompanyId;

                    actionStatus = _driverRepository.Update(dbEntity);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Driver, Entity.Driver>(dbEntity);
                    actionStatus.Success = true;
                    actionStatus.Message = "Licence Image deleted successfully!";
                    if (!actionStatus.Success)
                    {
                        _logger.Error($"Driver is not updated in database, Error: {actionStatus.Message}");
                        actionStatus.Success = false;
                        actionStatus.Message = actionStatus.Message;
                    }
                }
                else
                {
                    actionStatus.Success = false;
                    actionStatus.Message = "Licence Image not deleted!";
                }
                return actionStatus;
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "DriverManager.DeleteImage " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        private bool DeleteImage(string imageName)
        {
            var fileBasePath = SolutionConfiguration.UploadBasePath;

            var filePath = Path.Combine(fileBasePath, imageName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return true;
        }

        public Entity.SearchResult<List<Entity.DriverDetail>> List(Entity.SearchRequest request)
        {
            try
            {
                Entity.SearchResult<List<Entity.DriverDetail>> result = _driverRepository.List(request);
                return new Entity.SearchResult<List<Entity.DriverDetail>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.DriverDetail>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"DriverService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.DriverDetail>>();
            }
        }

        public Entity.BaseResponse<Entity.DriverDashboardOverviewResponse> GetDriverDetail(Guid driverGuid, DateTime currentDate, string timeZone)
        {
            Entity.BaseResponse<List<Entity.DriverDashboardOverviewResponse>> listResult = new Entity.BaseResponse<List<Entity.DriverDashboardOverviewResponse>>();
            Entity.BaseResponse<Entity.DriverDashboardOverviewResponse> result = new Entity.BaseResponse<Entity.DriverDashboardOverviewResponse>(true);
            try
            {
                listResult = _driverRepository.GetStatistics(driverGuid, currentDate, timeZone);

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

        public Entity.ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbEntity = _driverRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbEntity == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Entity");
                }
                var dbTrip = _tripRepository.FindBy(x => x.FleetGuid.Equals(dbEntity.FleetGuid) && !x.IsCompleted && !x.IsDeleted).FirstOrDefault();
                if (dbTrip == null)
                {
                    dbEntity.IsActive = status;
                    dbEntity.UpdatedDate = DateTime.Now;
                    dbEntity.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    return _driverRepository.Update(dbEntity);
                }
                else
                {
                    actionStatus.Success = false;
                    actionStatus.Message = "Driver status can not be updated trip exist for driver";
                    return actionStatus;
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
    }
}
