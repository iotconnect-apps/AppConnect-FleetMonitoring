using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Entity = iot.solution.entity;

namespace iot.solution.model.Repository.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        T GetByUniqueId(Expression<Func<T, bool>> predicate);
        Entity.ActionStatus Insert(T entity);
        Entity.ActionStatus Update(T entity);
        Entity.ActionStatus Delete(T entity);
        IQueryable<T> GetAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        Entity.ActionStatus RemoveRange(Expression<Func<T, bool>> predicate);
        void SetModified<K>(K entity) where K : class;
        Entity.ActionStatus InsertRange(List<T> entity);
        List<T> ExecuteStoredProcedure<T>(string spName, Dictionary<string, string> parameters) where T : new();
        int ExecuteStoredProcedureNonQuery(string spName, Dictionary<string, string> parameters);
    }
}