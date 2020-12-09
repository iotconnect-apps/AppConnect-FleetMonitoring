using component.logger;
using iot.solution.data;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

using LogHandler = component.services.loghandler;
namespace iot.solution.model.Repository.Implementation
{
    public class AdminRuleRepository : GenericRepository<Model.AdminRule>, IAdminRuleRepository
    {
        private readonly LogHandler.Logger _logger;
        private readonly ICompanyRepository _companyRepository;

        public AdminRuleRepository(IUnitOfWork unitOfWork, LogHandler.Logger logger, ICompanyRepository companyRepository) : base(unitOfWork, logger)
        {
            _logger = logger;
            _uow = unitOfWork;
            _companyRepository = companyRepository;
        }

        public Entity.SearchResult<List<Entity.AdminRule>> List(Entity.SearchRequest request)
        {
            var isAdmin = false;

            Entity.SearchResult<List<Entity.AdminRule>> result = new Entity.SearchResult<List<Entity.AdminRule>>();
            try
            {
                _logger.InfoLog(Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                var checkForAdmin = _companyRepository.FindBy(x => x.Guid.Equals(component.helper.SolutionConfiguration.CompanyId)).FirstOrDefault();

                if (checkForAdmin == null)
                    isAdmin = true;
                else
                    isAdmin = false;

                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    parameters.Add(sqlDataAccess.CreateParameter("isAdmin", isAdmin, DbType.Boolean, ParameterDirection.Input));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[AdminRule_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = DataUtils.DataReaderToList<Entity.AdminRule>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                _logger.InfoLog(Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

    }
}
