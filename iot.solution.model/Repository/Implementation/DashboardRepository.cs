using iot.solution.model.Models;
using component.logger;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using System.Reflection;
using iot.solution.data;
using System.Data.Common;
using System.Data;
using component.helper;
using System.Linq;
using LogHandler = component.services.loghandler;
namespace iot.solution.model.Repository.Implementation
{
    public class DashboardRepository:GenericRepository<Device>,IDashboardRepository
    {
        private readonly LogHandler.Logger _logger;
        public DashboardRepository(IUnitOfWork unitOfWork, LogHandler.Logger logManager) : base(unitOfWork, logManager)
        {
            _logger = logManager;
            _uow = unitOfWork;
        }
        public Entity.BaseResponse<List<Entity.DashboardOverviewResponse>> GetStatistics(DateTime currentDate, string timeZone)
        {
            Entity.BaseResponse<List<Entity.DashboardOverviewResponse>> result = new Entity.BaseResponse<List<Entity.DashboardOverviewResponse>>();
            try
            {
                _logger.InfoLog(Constants.ACTION_ENTRY, "GeneratorRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    DateTime dateValue;
                    if (DateTime.TryParse(currentDate.ToString(), out dateValue))
                    {
                        dateValue = dateValue.AddMinutes(-double.Parse(timeZone));
                    }
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(SolutionConfiguration.CurrentUserId, SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[CompanyStatistics_Get]", CommandType.StoredProcedure, null), parameters.ToArray());
                   
                    result.Data = DataUtils.DataReaderToList<Entity.DashboardOverviewResponse>(dbDataReader, null);
                    if (parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault() != null)
                    {
                        result.LastSyncDate = Convert.ToString(parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault().Value);
                    }
                }
                _logger.InfoLog(Constants.ACTION_EXIT, "GeneratorRepository.Get");
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.BaseResponse<List<Entity.DashboardOverviewByEntityResponse>> GetStatisticsByEntity(DateTime currentDate, string timeZone)
        {
            Entity.BaseResponse<List<Entity.DashboardOverviewByEntityResponse>> result = new Entity.BaseResponse<List<Entity.DashboardOverviewByEntityResponse>>();
            try
            {
                _logger.InfoLog(Constants.ACTION_ENTRY, "GetStatisticsByEntity.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    DateTime dateValue;
                    if (DateTime.TryParse(currentDate.ToString(), out dateValue))
                    {
                        dateValue = dateValue.AddMinutes(-double.Parse(timeZone));
                    }
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(SolutionConfiguration.CurrentUserId, SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("currentDate", dateValue, DbType.DateTime, ParameterDirection.Input));

                    parameters.Add(sqlDataAccess.CreateParameter("search", "", DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", -1, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", -1, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", "", DbType.String, ParameterDirection.Input));

                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Entity_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    
                    
                    result.Data = DataUtils.DataReaderToList<Entity.DashboardOverviewByEntityResponse>(dbDataReader, null);
                    //if (parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault() != null)
                    //{
                    //    result.LastSyncDate = Convert.ToString(parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault().Value);
                    //}
                }
                _logger.InfoLog(Constants.ACTION_EXIT, "GetStatisticsByEntity.Get");
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}
