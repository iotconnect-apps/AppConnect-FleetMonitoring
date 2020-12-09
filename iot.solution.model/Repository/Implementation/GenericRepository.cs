using component.logger;
using iot.solution.data;
using iot.solution.model.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;
namespace iot.solution.model.Repository.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class //, IEntityBase
    {
        public GenericRepository(IUnitOfWork unitOfWork, LogHandler.Logger logManager)
        {
            ConnectionString = component.helper.SolutionConfiguration.Configuration.ConnectionString;

            if (unitOfWork == null)
                throw new ArgumentNullException("UnitOfWork cannot be null.");
            if (logManager == null)
                throw new ArgumentNullException("LogManager cannot be null");

            _uow = unitOfWork;
            _logger = logManager;
        }
        private DbSet<T> Entities
        {
            get
            {
                if (_entities == null) _entities = _uow.DbContext.Set<T>();
                return _entities;
            }
        }

        public string ConnectionString { get; }

        public virtual void SetModified<K>(K entity) where K : class
        {
            _uow.DbContext.Entry(entity).State = EntityState.Modified;
        }
        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            _logger.InfoLog(Constants.ACTION_ENTRY, "GenericRepository.FindBy");
            var list = Entities.AsNoTracking().Where(predicate);
            _logger.InfoLog(Constants.ACTION_EXIT, "GenericRepository.FindBy");
            return list;
        }
        public IQueryable<T> GetAll()
        {
            _logger.InfoLog(Constants.ACTION_ENTRY, "GenericRepository.GetAll");
            IQueryable<T> list = Entities;
            _logger.InfoLog(Constants.ACTION_EXIT, "GenericRepository.GetAll");
            return list;
        }
        
        public T GetByUniqueId(Expression<Func<T, bool>> predicate)
        {
            _logger.InfoLog(Constants.ACTION_ENTRY, "GenericRepository.GetByUniqueId");
            var obj = Entities.AsNoTracking().FirstOrDefault(predicate);
            _logger.InfoLog(Constants.ACTION_EXIT, "GenericRepository.GetByUniqueId");
            return obj;
        }
        public Entity.ActionStatus Insert(T entity)
        {
            _logger.InfoLog(Constants.ACTION_ENTRY, "GenericRepository.Insert");
            if (entity == null) throw new ArgumentNullException("entity");
            var selfTran = false;
            if (!_uow.InTransaction)
            {
                _uow.BeginTransaction();
                selfTran = true;
            }

            var _actionStatus = new Entity.ActionStatus();
            try
            {
                // entity.RefId = entity.RefId == Guid.Empty ? Guid.NewGuid() : entity.RefId;
                Entities.Add(entity);
                _actionStatus = ApplyChanges();

                if (!_actionStatus.Success) throw new Exception(_actionStatus.Message);
                // _actionStatus.Result = entity.RecordId;
                _actionStatus.Data = entity;
                _logger.InfoLog(Constants.ACTION_EXIT, "GenericRepository.Insert");
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.InfoLog("GenericRepository.Insert", "Entity Inserted successfully,Committing Transaction");
                        var _tactionStatus = _uow.EndTransaction();
                        if (!_tactionStatus.Success) _actionStatus = _tactionStatus;
                    }
                    else
                    {
                        _logger.InfoLog("GenericRepository.Insert",
                            "Having issues while Inserting entity,Rollbaking transaction");
                        _uow.RollBack();
                    }
                }
            }

            return _actionStatus;
        }

        public Entity.ActionStatus InsertRange(List<T> entity)
        {
            _logger.InfoLog(Constants.ACTION_ENTRY, "GenericRepository.InsertRange");
            if (entity == null) throw new ArgumentNullException("entity");
            var selfTran = false;
            if (!_uow.InTransaction)
            {
                _uow.BeginTransaction();
                selfTran = true;
            }

            var _actionStatus = new Entity.ActionStatus();
            try
            {
                // entity.RefId = entity.RefId == Guid.Empty ? Guid.NewGuid() : entity.RefId;
                Entities.AddRange(entity);
                _actionStatus = ApplyChanges();

                if (!_actionStatus.Success) throw new Exception(_actionStatus.Message);
                // _actionStatus.Result = entity.RecordId;
                _actionStatus.Data = entity;
                _logger.InfoLog(Constants.ACTION_EXIT, "GenericRepository.InsertRange");
            }
            catch (Exception ex)
            {
                _logger.ErrorLog( ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.InfoLog("GenericRepository.InsertRange", "Entity Inserted successfully,Committing Transaction");
                        var _tactionStatus = _uow.EndTransaction();
                        if (!_tactionStatus.Success) _actionStatus = _tactionStatus;
                    }
                    else
                    {
                        _logger.InfoLog("GenericRepository.InsertRange",
                            "Having issues while Inserting entity,Rollbaking transaction");
                        _uow.RollBack();
                    }
                }
            }

            return _actionStatus;
        }
        public virtual Entity.ActionStatus Update(T entity)
        {
            _logger.InfoLog(Constants.ACTION_ENTRY, "GenericRepository.Update");
            if (entity == null) throw new ArgumentNullException("entity");
            var selfTran = false;
            if (!_uow.InTransaction)
            {
                _uow.BeginTransaction();
                selfTran = true;
            }

            var _actionStatus = new Entity.ActionStatus();
            try
            {
                SetModified(entity);
                _actionStatus = ApplyChanges();

                if (!_actionStatus.Success) throw new Exception(_actionStatus.Message);
                //_actionStatus.Result = entity.RecordId;
                _actionStatus.Data = entity;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.InfoLog("GenericRepository.Update", "Entity Updated successfully,Committing Transaction");
                        var _tactionStatus = _uow.EndTransaction();
                        if (!_tactionStatus.Success) _actionStatus = _tactionStatus;
                    }
                    else
                    {
                        _logger.InfoLog("GenericRepository.Update",
                            "Having issues while Updating entity,Rollbaking transaction");
                        _uow.RollBack();
                    }
                }
            }

            _logger.InfoLog(Constants.ACTION_EXIT, "GenericRepository.Update");
            return _actionStatus;
        }
        public Entity.ActionStatus Delete(T entity)
        {
            _logger.InfoLog(Constants.ACTION_ENTRY, "GenericRepository.Delete");

            if (entity == null) throw new ArgumentNullException("entity");

            var selfTran = false;
            if (!_uow.InTransaction)
            {
                _uow.BeginTransaction();
                selfTran = true;
            }

            var _actionStatus = new Entity.ActionStatus();
            try
            {
                Entities.Remove(entity);
                _actionStatus = ApplyChanges();

                if (!_actionStatus.Success) throw new Exception(_actionStatus.Message);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.InfoLog("GenericRepository.Delete",
                            "Operation executed successfully,Committing Transaction");
                        _actionStatus = _uow.EndTransaction();
                    }
                    else
                    {
                        _logger.InfoLog("GenericRepository.Delete",
                            "Having issues while deleting entity,Rollbaking transaction");
                        _uow.RollBack();
                    }
                }
            }

            _logger.InfoLog(Constants.ACTION_EXIT, "GenericRepository.Delete");
            return _actionStatus;
        }
        public Entity.ActionStatus RemoveRange(Expression<Func<T, bool>> predicate)
        {
            _logger.InfoLog(Constants.ACTION_ENTRY, "GenericRepository.RemoveRange");
            var _actionStatus = new Entity.ActionStatus(false, string.Empty, string.Empty, null);
            var entityList = Entities.Where(predicate);
            if (entityList != null && entityList.Count() > 0)
            {
                _actionStatus.Success = true;
                return _actionStatus;
            }

            var selfTran = false;
            if (!_uow.InTransaction)
            {
                _uow.BeginTransaction();
                selfTran = true;
            }

            try
            {
                _entities.RemoveRange(entityList);
                _actionStatus = ApplyChanges();

                if (!_actionStatus.Success) throw new Exception(_actionStatus.Message);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.InfoLog("GenericRepository.RemoveRange",
                            "Operation executed successfully,Committing Transaction");
                        _actionStatus = _uow.EndTransaction();
                    }
                    else
                    {
                        _logger.InfoLog("GenericRepository.RemoveRange",
                            "Having issues while deleting entity,Rollbaking transaction");
                        _uow.RollBack();
                    }
                }
            }

            return _actionStatus;
        }
        public List<T> ExecuteStoredProcedure<T>(string spName, Dictionary<string, string> parameters) where T : new()
        {
            var result = new List<T>();
            try
            {
                _logger.InfoLog(Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> dbparameters = new List<System.Data.Common.DbParameter>();
                    if (parameters != null && parameters.Any())
                    {
                        foreach (var param in parameters)
                        {
                            dbparameters.Add(sqlDataAccess.CreateParameter(param.Key, param.Value, DbType.String, ParameterDirection.Input));
                        }
                    }

                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(
                            sqlDataAccess.CreateCommand(spName, System.Data.CommandType.StoredProcedure, null), dbparameters.ToArray());
                    result = DataUtils.DataReaderToList<T>(dbDataReader, null);
                }
                _logger.InfoLog(Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public int ExecuteStoredProcedureNonQuery(string spName, Dictionary<string, string> parameters)
        {
            int result = 0;
            try
            {
                _logger.InfoLog(Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> dbparameters = new List<System.Data.Common.DbParameter>();
                    if (parameters != null && parameters.Any())
                    {
                        foreach (var param in parameters)
                        {
                            dbparameters.Add(sqlDataAccess.CreateParameter(param.Key, param.Value, DbType.String, ParameterDirection.Input));
                        }
                    }

                    result = sqlDataAccess.ExecuteStoredProcedureNonQuery(spName, dbparameters.ToArray());
                }
                _logger.InfoLog(Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

        #region Private Methods
        private Entity.ActionStatus ApplyChanges()
        {
            var result = new Entity.ActionStatus();
            try
            {
                result = _uow.SaveAndContinue();
                if (!result.Success) throw new Exception(result.Message);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.ErrorLog("GenericRepository.SaveChanges", ex);
                result.Message = ex.Message;
                _logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            catch (DbUpdateException ese)
            {
                _logger.ErrorLog("GenericRepository.SaveChanges", ese);

                result.Message = ese.Message;
                _logger.ErrorLog(ese, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }


            catch (Exception ex)
            {
                _logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
                result.Message = ex.Message;
            }

            return result;
        }

        #endregion

        #region Variable Declaration
        private readonly LogHandler.Logger _logger;
        protected IUnitOfWork _uow;
        private DbSet<T> _entities;
        #endregion


    }
}