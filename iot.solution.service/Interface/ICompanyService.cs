using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;

namespace iot.solution.service.Interface
{
    public interface ICompanyService
    {
        List<Entity.Company> Get();
        Entity.Company Get(Guid id);
        Entity.ActionStatus Manage(Entity.AddCompanyRequest request);
        Entity.ActionStatus Delete(Guid id);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);

    }
}
