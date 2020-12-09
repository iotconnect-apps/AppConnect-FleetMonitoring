using component.logger;
using iot.solution.data;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using LogHandler = component.services.loghandler;

namespace iot.solution.model.Repository.Implementation
{
    public class DeviceMaintenanceRepository : GenericRepository<Model.DeviceMaintenance>, IDeviceMaintenanceRepository
    {
        private readonly LogHandler.Logger logger;
        public DeviceMaintenanceRepository(IUnitOfWork unitOfWork, LogHandler.Logger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }

        public List<Entity.DeviceMaintenanceResponse> GetUpComingList(Entity.DeviceMaintenanceRequest request)
        {
            List<Entity.DeviceMaintenanceResponse> result = new List<Entity.DeviceMaintenanceResponse>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceMaintenanceRepository.GetUpComingList");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, "v1");
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    if (request.EntityGuid.HasValue)
                        parameters.Add(sqlDataAccess.CreateParameter("entityGuid", request.EntityGuid.Value, DbType.Guid, ParameterDirection.Input));
                    if (request.DeviceGuid.HasValue)
                        parameters.Add(sqlDataAccess.CreateParameter("guid", request.DeviceGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", request.currentDate, DbType.DateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[DeviceMaintenance_UpComingList]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result = DataUtils.DataReaderToList<Entity.DeviceMaintenanceResponse>(dbDataReader, null);

                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceMaintenanceRepository.GetUpComingList");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public Entity.DeviceMaintenance Get(Guid id, DateTime currentDate, string timeZone)
        {
            List<Entity.DeviceMaintenance> result = new List<Entity.DeviceMaintenance>();
            Entity.DeviceMaintenance maintenanceDetail = new Entity.DeviceMaintenance();
            try
            {
                DateTime dateValue;
                if (DateTime.TryParse(currentDate.ToString(), out dateValue))
                {
                    dateValue = dateValue.AddMinutes(-double.Parse(timeZone));
                }
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceMaintenanceRepository.GetUpComingList");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, "v1");
                    parameters.Add(sqlDataAccess.CreateParameter("guid", id, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[DeviceMaintenance_Get]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result = DataUtils.DataReaderToList<Entity.DeviceMaintenance>(dbDataReader, null);
                    if (result.Count > 0)
                    {
                        maintenanceDetail = result[0];
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceMaintenanceRepository.GetUpComingList");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return maintenanceDetail;
        }

        public Entity.BaseResponse<Entity.DeviceSceduledMaintenanceResponse> GetDeviceScheduledMaintenance(Entity.DeviceMaintenanceRequest request)
        {
            Entity.BaseResponse<List<Entity.DeviceSceduledMaintenanceResponse>> result = new Entity.BaseResponse<List<Entity.DeviceSceduledMaintenanceResponse>>();
            var deviceDetail = new Entity.BaseResponse<Entity.DeviceSceduledMaintenanceResponse>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceMaintenanceRepository.GetDeviceScheduledMaintenence");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    DateTime dateValue;
                    if (DateTime.TryParse(request.currentDate.ToString(), out dateValue))
                    {
                        dateValue = dateValue.AddMinutes(-double.Parse(request.timeZone));
                    }
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, "v1");
                    if (request.DeviceGuid.HasValue && !request.DeviceGuid.Value.Equals(Guid.Empty))
                      parameters.Add(sqlDataAccess.CreateParameter("guid", request.DeviceGuid.Value, DbType.Guid, ParameterDirection.Input));
                    if (request.EntityGuid.HasValue && !request.EntityGuid.Value.Equals(Guid.Empty))
                        parameters.Add(sqlDataAccess.CreateParameter("entityGuid", request.EntityGuid.Value, DbType.Guid, ParameterDirection.Input));
                   
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[DeviceStatistics_Get]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Data = DataUtils.DataReaderToList<Entity.DeviceSceduledMaintenanceResponse>(dbDataReader, null);
                    if (parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault() != null)
                    {
                        result.LastSyncDate = Convert.ToString(parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault().Value);
                    }
                    Entity.DeviceSceduledMaintenanceResponse obj = new Entity.DeviceSceduledMaintenanceResponse();
                    obj.UniqueId = "";
                    obj.Day = "0";
                    obj.Hour = "0";
                    obj.Minute = "0";
                    if (result.Data != null && result.Data.Count > 0)
                    {
                        var data = result.Data[0];
                        if (data.startDateTime != DateTime.MinValue)
                        {
                            dateValue = data.startDateTime;//.AddMinutes(-double.Parse(request.timeZone));// TimeZoneInfo.ConvertTimeBySystemTimeZoneId(data.startDateTime, "UTC", request.timeZone);
                            TimeSpan span = (dateValue - DateTime.UtcNow);// request.currentDate.Value);
                            obj.Day = Convert.ToString(span.Days);
                            obj.Hour = Convert.ToString(span.Hours);
                            obj.Minute = Convert.ToString(span.Minutes);
                            obj.UniqueId = data.UniqueId;
                        }
                    }
                    obj.startDateTime = dateValue;
                    deviceDetail.Data = obj;
                    deviceDetail.LastSyncDate = result.LastSyncDate;
                    deviceDetail.IsSuccess = true;
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceMaintenanceRepository.GetDeviceScheduledMaintenence");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return deviceDetail;
        }

        public Entity.SearchResult<List<Entity.DeviceMaintenanceDetail>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.DeviceMaintenanceDetail>> result = new Entity.SearchResult<List<Entity.DeviceMaintenanceDetail>>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceMaintenanceRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);

                    DateTime dateValue;
                    if (DateTime.TryParse(request.CurrentDate.ToString(), out dateValue))
                    {
                        dateValue = dateValue.AddMinutes(-double.Parse(request.TimeZone));                       
                    }
                    if (!request.EntityId.Equals(Guid.Empty))
                    {
                        parameters.Add(sqlDataAccess.CreateParameter("entityGuid", request.EntityId, DbType.Guid, ParameterDirection.Input));
                    }
                    if (!string.IsNullOrEmpty(request.Guid))
                    {
                        parameters.Add(sqlDataAccess.CreateParameter("deviceguid", new Guid(request.Guid), DbType.Guid, ParameterDirection.Input));
                    }

                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[DeviceMaintenance_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = DataUtils.DataReaderToList<Entity.DeviceMaintenanceDetail>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceMaintenanceRepository.Get");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.ActionStatus Manage(Model.DeviceMaintenance request)
        {
            Entity.ActionStatus result = new Entity.ActionStatus(true);
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DeviceMaintenanceRepository.Manage");
                int outPut = 0;
                int intResult = 0;
                string guidResult = string.Empty;
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", request.CompanyGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("startDateTime", request.StartDateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("endDateTime", request.EndDateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("description", request.Description, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    if (request.Guid == null || request.Guid == Guid.Empty)
                    {
                        parameters.Add(sqlDataAccess.CreateParameter("newid", request.Guid, DbType.Guid, ParameterDirection.Output));
                        parameters.Add(sqlDataAccess.CreateParameter("entityGuid", request.EntityGuid, ParameterDirection.Input));
                        parameters.Add(sqlDataAccess.CreateParameter("DeviceGuid", request.DeviceGuid, ParameterDirection.Input));
                        intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[DeviceMaintenance_Add]", CommandType.StoredProcedure, null), parameters.ToArray());
                        guidResult = parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString();
                    }
                    else
                    {
                        parameters.Add(sqlDataAccess.CreateParameter("guid", request.Guid, DbType.Guid, ParameterDirection.Input));
                        intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[DeviceMaintenance_UpdateStatus]", CommandType.StoredProcedure, null), parameters.ToArray());
                        guidResult = request.Guid.ToString();
                    }
                    outPut = int.Parse(parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault().Value.ToString());
                    if (outPut > 0)
                    {
                        if (!string.IsNullOrEmpty(guidResult))
                        {
                            result.Data = Guid.Parse(guidResult);
                        }
                    }
                    else
                    {
                        result.Success = false;
                        string msg = parameters.Where(p => p.ParameterName.Equals("fieldname")).FirstOrDefault().Value.ToString();
                        if (msg == "DeviceNotExists")
                        {
                            result.Message = "Device Not Exists";
                        }
                        else if (msg == "DeviceMaintenanceNotFound")
                        {
                            result.Message = "Device Maintenance Not Found";
                        }
                        else
                        {
                            result.Message = msg;
                        }
                    }

                }
                logger.InfoLog(Constants.ACTION_EXIT, "DeviceMaintenanceRepository.Manage");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

    }
}
