using component.helper;
using component.logger;
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
using System.Xml.Serialization;
using System.IO;

namespace iot.solution.model.Repository.Implementation
{
    public class FleetRepository : GenericRepository<Model.Fleet>, IFleetRepository
    {
        private readonly LogHandler.Logger logger;
        public FleetRepository(IUnitOfWork unitOfWork, LogHandler.Logger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }
        public List<Entity.LookupItem> GetLookup(Guid companyId)
        {
            var result = new List<Entity.LookupItem>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "FleetRepository.GetLookup");
                result = _uow.DbContext.Fleet.Where(u => u.CompanyGuid.Equals(companyId)  && u.IsActive == true && !u.IsDeleted).Select(g => new Entity.LookupItem() { Text = g.FleetId, Value = g.Guid.ToString() }).ToList();
                logger.InfoLog(Constants.ACTION_EXIT, "FleetRepository.GetLookup");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public List<Entity.LookupItem> GetTypeLookup()
        {
            var result = new List<Entity.LookupItem>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "FleetRepository.GetTypeLookup");
                result = _uow.DbContext.FleetType.Where(u => u.IsActive == true && !u.IsDeleted).Select(g => new Entity.LookupItem() { Text = g.Name, Value = g.Guid.ToString() }).ToList();
                logger.InfoLog(Constants.ACTION_EXIT, "FleetRepository.GetTypeLookup");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public List<Entity.LookupItem> GetMaterialTypeLookup()
        {
            var result = new List<Entity.LookupItem>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "FleetRepository.GetMaterialTypeLookup");
                result = _uow.DbContext.FleetMaterialType.Where(u => u.IsActive == true && !u.IsDeleted).Select(g => new Entity.LookupItem() { Text = g.Name, Value = g.Guid.ToString() }).ToList();
                logger.InfoLog(Constants.ACTION_EXIT, "FleetRepository.GetMaterialTypeLookup");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.SearchResult<List<Entity.FleetListItem>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.FleetListItem>> result = new Entity.SearchResult<List<Entity.FleetListItem>>();
            List<Entity.FleetListItem> lst = new List<FleetListItem>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "FleetRepository.List");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    DateTime dateValue;
                    if (DateTime.TryParse(request.CurrentDate.ToString(), out dateValue))
                    {
                        dateValue = dateValue.AddMinutes(-double.Parse(request.TimeZone));
                    }
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                   
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));

                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Fleet_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    lst = DataUtils.DataReaderToList<Entity.FleetListItem>(dbDataReader, null);
                    
                    result.Items = lst;
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.InfoLog(Constants.ACTION_EXIT, "FleetRepository.List");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.SearchResult<List<Entity.FleetMapListItem>> MapList(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.FleetMapListItem>> result = new Entity.SearchResult<List<Entity.FleetMapListItem>>();
            List<Entity.FleetMapListItem> lst = new List<FleetMapListItem>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "FleetRepository.List");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    DateTime dateValue;
                    if (DateTime.TryParse(request.CurrentDate.ToString(), out dateValue))
                    {
                        dateValue = dateValue.AddMinutes(-double.Parse(request.TimeZone));
                    }
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));

                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));

                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Fleet_MapList]", CommandType.StoredProcedure, null), parameters.ToArray());
                    lst = DataUtils.DataReaderToList<Entity.FleetMapListItem>(dbDataReader, null);

                    result.Items = lst;
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.InfoLog(Constants.ACTION_EXIT, "FleetRepository.MapList");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public ActionStatus Manage(Model.FleetModel request)
        {
            ActionStatus result = new ActionStatus(true);
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "FleetRepository.Manage");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", request.CompanyGuid, DbType.Guid, ParameterDirection.Input));
                    if(request.Guid!=null && request.Guid !=Guid.Empty)
                        parameters.Add(sqlDataAccess.CreateParameter("guid", request.Guid, DbType.Guid, ParameterDirection.Input));
                  
                    parameters.Add(sqlDataAccess.CreateParameter("fleetId", request.FleetId, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("registrationNo", request.RegistrationNo, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("loadingCapacity", request.LoadingCapacity, DbType.String, ParameterDirection.Input));
                   
                    parameters.Add(sqlDataAccess.CreateParameter("typeGuid", request.TypeGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("materialTypeGuid", request.MaterialTypeGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("speedLimit", request.SpeedLimit, DbType.String, ParameterDirection.Input));

                    parameters.Add(sqlDataAccess.CreateParameter("latitude", request.Latitude, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("longitude", request.Longitude, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("radius", request.Radius, DbType.Int64, ParameterDirection.Input));

                    parameters.Add(sqlDataAccess.CreateParameter("totalMiles", request.TotalMiles, DbType.Int64, ParameterDirection.Input));

                    parameters.Add(sqlDataAccess.CreateParameter("image", request.Image, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("deviceData", request.deviceData, DbType.Xml, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("newid", request.Guid,  DbType.Guid, ParameterDirection.Output));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[Fleet_AddUpdate]", CommandType.StoredProcedure, null), parameters.ToArray());                    
                    
                    int outPut = int.Parse(parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault().Value.ToString());
                    if (outPut > 0)
                    {
                        string guidResult = parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString();
                        if (!string.IsNullOrEmpty(guidResult))
                        {
                            result.Data = _uow.DbContext.Fleet.Where(u => u.Guid.Equals(Guid.Parse(guidResult))).FirstOrDefault();
                        }
                    }
                    else
                    {
                        
                        result.Success = false;
                        string msg = parameters.Where(p => p.ParameterName.Equals("fieldname")).FirstOrDefault().Value.ToString();
                        if (msg == "RegistrationNoAlreadyExists")
                        {
                            result.Message = "Registration No Sould Be Unique";
                        }
                        else if (msg == "FleetIdAlreadyExists")
                        {
                            result.Message = "FleetId Sould Be Unique";
                        }
                        else if (msg == "OnGoingTripExists")
                        {
                            result.Message = "Fleet is allocated with on going trip so it cannot be updated.";
                        }
                        else if (msg == "OnGoingMaintenanceExists")
                        {
                            result.Message = "Fleet maintenance is going on so it cannot be updated.";
                        }
                        else
                        {
                            result.Message = "Failed To Save Device";
                        }
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "FleetRepository.Manage");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public ActionStatus ValidateDevice(Entity.FleetDeleteModel request)
        {
            ActionStatus result = new ActionStatus(true);
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "FleetRepository.ValidateDevice");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);

                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    if (request.guid != null && request.guid != Guid.Empty)
                        parameters.Add(sqlDataAccess.CreateParameter("guid", request.guid, DbType.Guid, ParameterDirection.Input));
                    DateTime dateValue;
                    if (DateTime.TryParse(request.currentDate.ToString(), out dateValue))
                    {
                        dateValue = dateValue.AddMinutes(-double.Parse(request.timeZone));
                    }
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));

                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[Device_ValidateDelete]", CommandType.StoredProcedure, null), parameters.ToArray());

                    int outPut = int.Parse(parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault().Value.ToString());
                    if (outPut > 0)
                    {
                        result.Message = "Device can be deleted.";
                    }
                    else
                    {
                        result.Success = false;
                        string msg = parameters.Where(p => p.ParameterName.Equals("fieldname")).FirstOrDefault().Value.ToString();
                        result.Message = msg;
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "FleetRepository.ValidateDevice");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public ActionStatus Delete(Entity.FleetDeleteModel request)
        {
            ActionStatus result = new ActionStatus(true);
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "FleetRepository.Delete");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);

                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    if (request.guid != null && request.guid != Guid.Empty)
                        parameters.Add(sqlDataAccess.CreateParameter("guid", request.guid, DbType.Guid, ParameterDirection.Input));
                    DateTime dateValue;
                    if (DateTime.TryParse(request.currentDate.ToString(), out dateValue))
                    {
                        dateValue = dateValue.AddMinutes(-double.Parse(request.timeZone));
                    }
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));

                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[Fleet_Delete]", CommandType.StoredProcedure, null), parameters.ToArray());

                    int outPut = int.Parse(parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault().Value.ToString());
                    if (outPut > 0)
                    {
                        result.Message = "Fleet deleted successfully.";
                    }
                    else
                    {

                        result.Success = false;
                        string msg = parameters.Where(p => p.ParameterName.Equals("fieldname")).FirstOrDefault().Value.ToString();
                        if (msg == "OnGoingTripExists")
                        {
                            result.Message = "Fleet is allocated with on going trip so it cannot be deleted.";
                        }
                        else if (msg == "OnGoingMaintenanceExists")
                        {
                            result.Message = "Fleet maintenance is going on so it cannot be deleted.";
                        }
                        else if (msg == "DriverExists")
                        {
                            result.Message = "Driver is allocated to fleet so it cannot be deleted.";
                        }
                        else
                        {
                            result.Message = "Failed To Delete Fleet";
                        }
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "FleetRepository.Delete");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.BaseResponse<List<Entity.FleetDashboardOverviewResponse>> GetStatistics(Guid fleetGuid, DateTime currentDate, string timeZone)
        {
            Entity.BaseResponse<List<Entity.FleetDashboardOverviewResponse>> result = new Entity.BaseResponse<List<Entity.FleetDashboardOverviewResponse>>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "EntityRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    DateTime dateValue;
                    if (DateTime.TryParse(currentDate.ToString(), out dateValue))
                    {
                        dateValue = dateValue.AddMinutes(-double.Parse(timeZone));
                    }
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(SolutionConfiguration.CurrentUserId, SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", fleetGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[FleetStatistics_Get]", CommandType.StoredProcedure, null), parameters.ToArray());

                    result.Data = DataUtils.DataReaderToList<Entity.FleetDashboardOverviewResponse>(dbDataReader, null);
                    if (parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault() != null)
                    {
                        result.LastSyncDate = Convert.ToString(parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault().Value);
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "EntityRepository.Get");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}
