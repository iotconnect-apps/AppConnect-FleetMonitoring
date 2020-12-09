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
using Model = iot.solution.model.Models;
using LogHandler = component.services.loghandler;
namespace iot.solution.model.Repository.Implementation
{
    public class RoleRepository : GenericRepository<Model.Role>, IRoleRepository
    {
        private readonly LogHandler.Logger logger;

        public RoleRepository(IUnitOfWork unitOfWork, LogHandler.Logger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }

        public Entity.SearchResult<List<Model.Role>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Model.Role>> result = new Entity.SearchResult<List<Model.Role>>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "RoleRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Role_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = DataUtils.DataReaderToList<Model.Role>(dbDataReader, null);
                    //  result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                    var count = parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault();
                    if (count != null && !string.IsNullOrWhiteSpace(count.Value.ToString()))
                    {
                        result.Count = int.Parse(count.Value.ToString());
                    }
                    var outPut = parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault();
                    if (outPut != null)
                    {
                        int outputvalue = int.Parse(outPut.Value.ToString());
                        result.Count = outputvalue < 0 ? outputvalue : result.Count;
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "RoleRepository.Get");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public ActionStatus Manage(Model.Role request)
        {
            ActionStatus result = new ActionStatus(true);
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "RoleRepository.Manage");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    if(request.Guid!=null && request.Guid!=Guid.Empty)
                        parameters.Add(sqlDataAccess.CreateParameter("guid", request.Guid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", request.CompanyGuid, DbType.Guid, ParameterDirection.Input));                   
                    parameters.Add(sqlDataAccess.CreateParameter("name", request.Name, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("description", request.Description, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("isAdminRole", request.IsAdminRole, DbType.Boolean, ParameterDirection.Input));                    
                    parameters.Add(sqlDataAccess.CreateParameter("newid",request.Guid, DbType.Guid, ParameterDirection.Output));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[Role_AddUpdate]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Data = parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString();
                }
                logger.InfoLog(Constants.ACTION_EXIT, "RoleRepository.Manage");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}