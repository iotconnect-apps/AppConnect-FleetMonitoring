using component.logger;
using iot.solution.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using EF = iot.solution.model.Models;
using Entity = iot.solution.entity;

namespace iot.solution.model
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(DbContext dbContext, ILogger logManager)
        {
            if (dbContext == null)
                throw new ArgumentNullException("DBContext cannot be null.");
            if (logManager == null)
                throw new ArgumentNullException("LogManager cannot be null");

            DbContext = (EF.devfleetmonitoringContext) dbContext;
            _logger = logManager;
        }

        private bool _disposed;
        private readonly ILogger _logger;
        private IDbContextTransaction _transaction { get; set; }

        public bool InTransaction { get; private set; }
        public EF.devfleetmonitoringContext DbContext { get; }
        public virtual void BeginTransaction()
        {
            _logger.Information(Constants.ACTION_ENTRY, "UnitOfWork.BeginTransaction");
            try
            {
                InTransaction = true;
                _transaction = DbContext.Database.BeginTransaction();
                _logger.Information(Constants.ACTION_EXIT, "UnitOfWork.BeginTransaction");
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION + ":UnitofWork.BeginTransaction", ex);
                throw;
            }
        }
        public virtual Entity.ActionStatus EndTransaction()
        {
            _logger.Information(Constants.ACTION_ENTRY, "UnitOfWork.EndTransaction");
            var status = new Entity.ActionStatus();
            try
            {
                if (_disposed) throw new ObjectDisposedException(GetType().FullName);
                DbContext.SaveChanges();
                _transaction.Commit();
                InTransaction = false;
                status.Success = true;
            }
            //catch (DbEntityValidationException dbEx)
            //{
            //    _logger.Error(Constants.ACTION_EXCEPTION + ":UnitofWork.EndTransaction", dbEx);
            //    status.Message = dbEx.Message;
            //    status.Success = false;
            //    _inTransaction = false;
            //}
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION + ":UnitofWork.SaveAndContinue", ex);
                status.Message = ex.Message;
                status.Success = false;
            }

            _logger.Information(Constants.ACTION_EXIT, "UnitOfWork.EndTransaction");
            return status;
        }
        public virtual void RollBack()
        {
            _logger.Information(Constants.ACTION_ENTRY, "UnitOfWork.RollBack");
            try
            {
                _transaction.Rollback();
                _transaction.Dispose();
                InTransaction = false;
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION + ":UnitofWork.BeginTransaction", ex);
                throw;
            }

            _logger.Information(Constants.ACTION_EXIT, "UnitOfWork.RollBack");
        }
        public virtual Entity.ActionStatus SaveAndContinue()
        {
            _logger.Information(Constants.ACTION_ENTRY, "UnitOfWork.SaveAndContinue");
            var status = new Entity.ActionStatus();
            try
            {
                DbContext.SaveChanges();
                status.Success = true;                
            }
            //catch (DbEntityValidationException dbEx)
            //{
            //    _logger.Error(Constants.ACTION_EXCEPTION + ":UnitofWork.SaveAndContinue", dbEx);
            //    var errorMessages = dbEx.EntityValidationErrors
            //        .SelectMany(x => x.ValidationErrors)
            //        .Select(x => x.ErrorMessage);

            //    status.Message = string.Join("; ", errorMessages);
            //    status.Success = false;
            //}
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION + ":UnitofWork.SaveAndContinue", ex);
                status.Message = ex.Message;
                status.Success = false;
            }

            _logger.Information(Constants.ACTION_EXIT, "UnitOfWork.SaveAndContinue");
            return status;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing && DbContext != null && InTransaction) _transaction.Dispose();
            if (disposing && DbContext != null) DbContext.Dispose();

            _disposed = true;
        }
        public void Dispose()
        {
            _logger.Information(Constants.ACTION_ENTRY, "UnitOfWork.Dispose");
            Dispose(true);
            GC.SuppressFinalize(this);
            _logger.Information(Constants.ACTION_EXIT, "UnitOfWork.Dispose");
        }
    }
}