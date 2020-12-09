using EF = iot.solution.model.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Entity = iot.solution.entity;

namespace iot.solution.model
{
    public interface IUnitOfWork : IDisposable
    {
        EF.devfleetmonitoringContext DbContext { get; }
        bool InTransaction { get; }
        void BeginTransaction();
        Entity.ActionStatus SaveAndContinue();
        Entity.ActionStatus EndTransaction();
        void RollBack();
    }
}