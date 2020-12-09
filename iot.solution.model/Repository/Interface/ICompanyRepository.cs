using iot.solution.entity;
using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Interface
{
    public interface ICompanyRepository : IGenericRepository<Model.Company>
    {
        ActionStatus Manage(Entity.AddCompanyRequest request);
        ActionStatus UpdateDetails(Model.Company request);
    }
}
