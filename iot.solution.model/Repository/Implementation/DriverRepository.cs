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

namespace iot.solution.model.Repository.Implementation
{
    public class DriverRepository : GenericRepository<Model.Driver>, IDriverRepository
    {
        private readonly LogHandler.Logger logger;
        public DriverRepository(IUnitOfWork unitOfWork, LogHandler.Logger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }

        public Entity.SearchResult<List<Entity.DriverDetail>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.DriverDetail>> result = new Entity.SearchResult<List<Entity.DriverDetail>>();
            List<Entity.DriverDetail> lst = new List<DriverDetail>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DriverRepository.List");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Driver_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    lst = DataUtils.DataReaderToList<Entity.DriverDetail>(dbDataReader, null);
                    result.Items = lst;
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.InfoLog(Constants.ACTION_EXIT, "EntityRepository.List");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public ActionStatus Manage(Model.Driver request)
        {
            ActionStatus result = new ActionStatus(true);
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "EntityRepository.Manage");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", request.Guid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", request.CompanyGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("fleetGuid", request.FleetGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("firstName", request.FirstName, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("lastName", request.LastName, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("email", request.Email, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("contactNo", request.ContactNo, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("address ", request.Address, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("city", request.City, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("stateGuid", request.StateGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("countryGuid", request.CountryGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("zipCode", request.Zipcode, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("image", request.Image, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("licenceNo", request.LicenceNo, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("licenceImage", request.LicenceImage, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("driverId", request.DriverId, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("newid", request.Guid, DbType.Guid, ParameterDirection.Output));
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[Driver_AddUpdate]", CommandType.StoredProcedure, null), parameters.ToArray());

                    int outPut = int.Parse(parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault().Value.ToString());
                    if (outPut > 0)
                    {
                        string guidResult = parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString();
                        if (!string.IsNullOrEmpty(guidResult))
                        {
                            result.Data = _uow.DbContext.Driver.Where(u => u.Guid.Equals(Guid.Parse(guidResult))).FirstOrDefault();
                        }
                    }
                    else
                    {
                        result.Success = false;
                        string msg = parameters.Where(p => p.ParameterName.Equals("fieldname")).FirstOrDefault().Value.ToString();
                        if (msg == "EmailIdAlreadyExists")
                        {
                            result.Message = "EmailId Already Exists";
                        }
                        else if (msg == "LicenceNoAlreadyExists")
                        {
                            result.Message = "Licence No Already Exists";
                        }
                        else if (msg == "DriverIdAlreadyExists")
                        {
                            result.Message = "Driver Id Already Exists";
                        }
                        else
                        {
                            result.Message = "Failed To Save Driver";
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

        public Entity.BaseResponse<List<Entity.DriverDashboardOverviewResponse>> GetStatistics(Guid driverGuid, DateTime currentDate, string timeZone)
        {
            Entity.BaseResponse<List<Entity.DriverDashboardOverviewResponse>> result = new Entity.BaseResponse<List<Entity.DriverDashboardOverviewResponse>>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "DriverRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    DateTime dateValue;
                    if (DateTime.TryParse(currentDate.ToString(), out dateValue))
                    {
                        dateValue = dateValue.AddMinutes(-double.Parse(timeZone));
                    }
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(SolutionConfiguration.CurrentUserId, SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", driverGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[DriverStatistics_Get]", CommandType.StoredProcedure, null), parameters.ToArray());

                    result.Data = DataUtils.DataReaderToList<Entity.DriverDashboardOverviewResponse>(dbDataReader, null);
                    if (parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault() != null)
                    {
                        result.LastSyncDate = Convert.ToString(parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault().Value);
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "DriverRepository.Get");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}
