using iot.solution.data;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;
using Model = iot.solution.model.Models;
using LogHandler = component.services.loghandler;
using component.logger;
using component.helper;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace iot.solution.model.Repository.Implementation
{
    public class TripRepository : GenericRepository<Model.Trip>, ITripRepository
    {
        private readonly LogHandler.Logger logger;
        private readonly IWebHostEnvironment _env;
        public TripRepository(IUnitOfWork unitOfWork, IWebHostEnvironment env, LogHandler.Logger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
            _env = env;
        }

        public Entity.SearchResult<List<Entity.TripListItem>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.TripListItem>> result = new Entity.SearchResult<List<Entity.TripListItem>>();
            List<Entity.TripListItem> lst = new List<Entity.TripListItem>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "TripRepository.List");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    DateTime dateValue;
                    if (DateTime.TryParse(request.CurrentDate.ToString(), out dateValue))
                    {
                        dateValue = dateValue.AddMinutes(-double.Parse(request.TimeZone));
                        parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));
                    }
                    
                    if (request.FleetGuid.HasValue && request.FleetGuid.Value != Guid.Empty) {
                        parameters.Add(sqlDataAccess.CreateParameter("fleetGuid", request.FleetGuid.Value, DbType.Guid, ParameterDirection.Input));
                    }
                    if (request.DriverGuid.HasValue && request.DriverGuid.Value != Guid.Empty)
                    {
                        parameters.Add(sqlDataAccess.CreateParameter("driverGuid", request.DriverGuid.Value, DbType.Guid, ParameterDirection.Input));
                    }
                    if (request.StartDate.HasValue && request.EndDate.HasValue)
                    {
                        DateTime dateRangeValue;
                        if (DateTime.TryParse(request.StartDate.ToString(), out dateRangeValue))
                        {
                            request.StartDate = dateRangeValue.AddMinutes(-double.Parse(request.TimeZone));
                        }

                        if (DateTime.TryParse(request.EndDate.ToString(), out dateRangeValue))
                        {
                            request.EndDate = dateRangeValue.AddMinutes(-double.Parse(request.TimeZone));
                        }
                        parameters.Add(sqlDataAccess.CreateParameter("startDate", request.StartDate, DbType.DateTime, ParameterDirection.Input));
                        parameters.Add(sqlDataAccess.CreateParameter("endDate", request.EndDate, DbType.DateTime, ParameterDirection.Input));

                    }
                    
                    if (!string.IsNullOrEmpty(request.Status))
                    {
                        parameters.Add(sqlDataAccess.CreateParameter("status", request.Status, DbType.String, ParameterDirection.Input));
                    }
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Trip_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    lst = DataUtils.DataReaderToList<Entity.TripListItem>(dbDataReader, null);
                    result.Items = lst;
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.InfoLog(Constants.ACTION_EXIT, "TripRepository.List");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public ActionStatus Manage(Model.TripModel request)
        {
            ActionStatus result = new ActionStatus(true);
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "TripRepository.Manage");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    if (request.Guid != null && request.Guid != Guid.Empty)
                        parameters.Add(sqlDataAccess.CreateParameter("guid", request.Guid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("fleetGuid", request.FleetGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("tripId", request.TripId, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("sourceLocation", request.SourceLocation, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("destinationLocation", request.DestinationLocation, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("materialTypeGuid", request.MaterialTypeGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("weight", request.Weight, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("startDateTime ", request.StartDateTime, DbType.DateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("sourceLatitude ", request.SourceLatitude, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("sourceLongitude ", request.SourceLongitude, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("destinationLatitude ", request.DestinationLatitude, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("destinationLongitude ", request.DestinationLongitude, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("totalMiles ", request.TotalMiles, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("stopData", request.stopData, DbType.Xml, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("newid", request.Guid, DbType.Guid, ParameterDirection.Output));
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[Trip_AddUpdate]", CommandType.StoredProcedure, null), parameters.ToArray());

                    int outPut = int.Parse(parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault().Value.ToString());
                    if (outPut > 0)
                    {
                        string guidResult = parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString();
                        if (!string.IsNullOrEmpty(guidResult))
                        {
                            result.Data = _uow.DbContext.Trip.Where(u => u.Guid.Equals(Guid.Parse(guidResult))).FirstOrDefault();
                        }
                    }
                    else
                    {
                        result.Success = false;
                        string msg = parameters.Where(p => p.ParameterName.Equals("fieldname")).FirstOrDefault().Value.ToString();
                        if (msg == "FleetNotExists")
                        {
                            result.Message = "Fleet Not Exists";
                        }
                        else if(msg == "DriverNotExist")
                        {
                            result.Message = "Driver Not Assigned To Selected Fleet";
                        }
                        else if (msg == "TripAlreadyExists")
                        {
                            result.Message = "Trip Already Exists";
                        }
                        else if (msg == "TripIdAlreadyExists")
                        {
                            result.Message = "Trip Id Already Exists";
                        }
                        else if (msg == "WeightIsGreaterThenFleetLoadCapacity")
                        {
                            result.Message = "Weight Should Be Less Then or Equal To Total Fleet Load Capacity";
                        }
                        else if (msg == "DeviceMaintenenceExists")
                        {
                            result.Message = "Fleet is Under Maintenence Between Selected Date Range";
                        }
                        else if (msg == "TripAlreadyExistsBetweenDateRange")
                        {
                            result.Message = "Trip Already Exists Between Selected Date Range";
                        }
                        else if (msg == "StartDateIsGreaterThenEndDate")
                        {
                            result.Message = "Trip StartDateTime Should be Less Then EndDateTime";
                        }
                        else
                        {
                            result.Message = "Failed To Save Trip";
                        }
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DriverRepository.Manage");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public Entity.ActionStatus UploadFiles(string xmlString, string tripId)
        {
            var response = new ActionStatus();
            int outPut = 0;
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceRepository.UploadFiles");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("tripGuid", Guid.Parse(tripId)));
                    parameters.Add(sqlDataAccess.CreateParameter("files", xmlString, DbType.Xml, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[ShipmentFiles_Add]", CommandType.StoredProcedure, null), parameters.ToArray());
                    outPut = int.Parse(parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault().Value.ToString());
                }

                if (outPut == 1)
                {
                    response.Message = "Files Uploaded Successfully!!";
                    response.Data = null;
                    response.Success = true;
                }
                else
                {
                    response.Message = "Unable to Upload Files";
                    response.Data = null;
                    response.Success = false;
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceRepository.UploadFiles");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return response;
        }

        public Entity.ActionStatus AddTripStops(string xmlString, string tripId)
        {
            var response = new ActionStatus();
            int outPut = 0;
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "TripRepository.tripstops");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("tripGuid", Guid.Parse(tripId)));
                    parameters.Add(sqlDataAccess.CreateParameter("trips", xmlString, DbType.Xml, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[TripStops_Add]", CommandType.StoredProcedure, null), parameters.ToArray());
                    outPut = int.Parse(parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault().Value.ToString());
                }

                if (outPut == 1)
                {
                    response.Message = "Trips Added Successfully!!";
                    response.Data = null;
                    response.Success = true;
                }
                else
                {
                    response.Message = "Unable to Add Trips";
                    response.Data = null;
                    response.Success = false;
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceRepository.UploadFiles");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return response;
        }
        public Entity.ActionStatus DeleteShipmentFile(Guid tripId, Guid? fileId)
            {
                ActionStatus result = new ActionStatus(true);
                try
                {
                    logger.InfoLog(Constants.ACTION_ENTRY, "DeviceRepository.DeleteMediaFiles");
                    using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                    {

                        List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                        parameters.Add(sqlDataAccess.CreateParameter("guid", fileId, DbType.Guid, ParameterDirection.Input));
                        parameters.Add(sqlDataAccess.CreateParameter("tripGuid", tripId, DbType.Guid, ParameterDirection.Input));
                        parameters.Add(sqlDataAccess.CreateParameter("status", true, DbType.Boolean, ParameterDirection.Input));
                        parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                        parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                        sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[ShipmentFiles_UpdateStatus]", CommandType.StoredProcedure, null), parameters.ToArray());
                        int outPut = int.Parse(parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault().Value.ToString());
                        if (outPut > 0)
                        {
                            result.Success = true;
                        }
                        else
                        {
                            result.Success = false;
                        }
                        result.Message = parameters.Where(p => p.ParameterName.Equals("fieldname")).FirstOrDefault().Value.ToString();
                    }
                    logger.InfoLog(Constants.ACTION_EXIT, "DeviceRepository.DeleteMediaFiles");
                }
                catch (Exception ex)
                {
                    logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
                }
                return result;
            }

        public List<Entity.DeviceMediaFiles> GetShipmentFiles(Guid tripId)
        {
            var result = new List<DeviceMediaFiles>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceRepository.GetMediaFiles");

                result = _uow.DbContext.ShipmentFiles.Where(u => u.TripGuid == tripId && !u.IsDeleted).Select(g => new Entity.DeviceMediaFiles()
                {
                    Guid = g.Guid,
                    FilePath = g.FilePath,
                    Description = g.Description,
                    FileName = Path.GetFileName(g.FilePath),
                    //  FileSize = GetFileSize(g.FilePath)
                }).ToList();
                foreach (DeviceMediaFiles file in result)
                {
                    file.FileSize = GetFileSize(file.FilePath);
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceRepository.GetMediaFiles");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        private string GetFileSize(string FilePath)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            string fileSize = string.Empty;
            string contentRootPath = _env.ContentRootPath;
            string webRootPath = _env.WebRootPath;
            FileInfo fileInfo = new FileInfo(webRootPath + "/" + FilePath);
            if (fileInfo.Exists)
            {
                try
                {
                    double len = fileInfo.Length;
                    int order = 0;
                    while (len >= 1024 && order < sizes.Length - 1)
                    {
                        order++;
                        len = len / 1024;
                    }

                    // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
                    // show a single decimal place, and no space.
                    fileSize = String.Format("{0:0.##} {1}", len, sizes[order]);
                }
                catch (Exception ex)
                {

                }
            }
            return fileSize;
        }

        public List<Entity.TripStops> GetTripStops(Guid tripId)
        {
            var result = new List<Entity.TripStops>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceRepository.GetMediaFiles");

                result = _uow.DbContext.TripStops.Where(u => u.TripGuid == tripId && !u.IsDeleted).Select(g => new Entity.TripStops()
                {
                    Guid = g.Guid,
                    TripGuid = g.TripGuid,
                    StopName = g.StopName,
                    EndDateTime=g.EndDateTime,
                    Latitude=g.Latitude,
                    Longitude=g.Longitude
                   
                }).ToList();
                
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceRepository.GetMediaFiles");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public ActionStatus Delete(Guid tripId)
        {
            ActionStatus result = new ActionStatus(true);
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "Trip.Delete");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("guid", tripId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[Trip_Delete]", CommandType.StoredProcedure, null), parameters.ToArray());

                    int outPut = int.Parse(parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault().Value.ToString());
                    if (outPut > 0)
                    {
                        result.Message = "Trip deleted successfully.";
                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                        string msg = parameters.Where(p => p.ParameterName.Equals("fieldname")).FirstOrDefault().Value.ToString();
                        if (msg == "OnGoingTripExists")
                        {
                            result.Message = "Trip is on going so it can not be deleted !!";
                        }
                        else if (msg == "TripNotFound")
                        {
                            result.Message = "Trip not found.";
                        }
                        else
                        {
                            result.Message = "Failed To Delete Trip";
                        }
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "Trip.Delete");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public bool GetTripStatus(Guid tripId)
        {
            bool res = true;
            List<Entity.TripListItem> lst = new List<Entity.TripListItem>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "TripRepository.List");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("tripGuid", tripId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Get_TripStatus]", CommandType.StoredProcedure, null), parameters.ToArray());
                    lst = DataUtils.DataReaderToList<Entity.TripListItem>(dbDataReader, null);
                    if(lst.Count>0)
                    {
                        Entity.TripListItem obj = new TripListItem();
                        obj = lst[0];
                        if(obj.Status.Equals("On Going"))
                        {
                            res = false;
                        }
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "TripRepository.List");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return res;
        }

        public Entity.BaseResponse<List<Entity.TripDashboardOverviewResponse>> GetStatistics(Guid tripGuid, DateTime currentDate, string timeZone)
        {
            Entity.BaseResponse<List<Entity.TripDashboardOverviewResponse>> result = new Entity.BaseResponse<List<Entity.TripDashboardOverviewResponse>>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "TripRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    DateTime dateValue;
                    if (DateTime.TryParse(currentDate.ToString(), out dateValue))
                    {
                        dateValue = dateValue.AddMinutes(-double.Parse(timeZone));
                    }
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(SolutionConfiguration.CurrentUserId, SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", tripGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[TripStatistics_Get]", CommandType.StoredProcedure, null), parameters.ToArray());

                    result.Data = DataUtils.DataReaderToList<Entity.TripDashboardOverviewResponse>(dbDataReader, null);
                    if (parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault() != null)
                    {
                        result.LastSyncDate = Convert.ToString(parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault().Value);
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "TripRepository.Get");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public ActionStatus StartTrip(Entity.StartTripModal request)
        {
            ActionStatus result = new ActionStatus(true);
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "TripRepository.TripStart");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("tripId", Guid.Parse(request.tripGuid), DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("fleetId", request.fleetGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("actualStartDateTime", request.currentDate, DbType.DateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("etaEndDateTime", request.etaEndDateTime, DbType.DateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("odometer", request.odometer.Value, DbType.Int64, ParameterDirection.Input));
                    
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[TripStart_UpdateStatus]", CommandType.StoredProcedure, null), parameters.ToArray());
                    int outPut = int.Parse(parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault().Value.ToString());
                    if (outPut > 0)
                    {
                        result.Success = true;
                        result.Message= parameters.Where(p => p.ParameterName.Equals("fieldname")).FirstOrDefault().Value.ToString();
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = parameters.Where(p => p.ParameterName.Equals("fieldname")).FirstOrDefault().Value.ToString();
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DriverRepository.Manage");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

    }
}
