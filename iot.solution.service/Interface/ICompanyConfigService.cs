using Entity = iot.solution.entity;
using System;
using System.Collections.Generic;
using System.Text;


namespace iot.solution.service.Interface
{
    public interface ICompanyConfigService
    {
        List<Entity.CompanyConfig> Get();
        Entity.CompanyConfig Get(Guid id);
        Entity.ActionStatus Manage(Entity.CompanyConfig companyConfig);
        Entity.ActionStatus List(string searchContext);
        Entity.ActionStatus Delete(Guid id);
    }
}
