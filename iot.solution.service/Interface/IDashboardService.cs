using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;

namespace iot.solution.service.Interface
{
    public interface IDashboardService
    {
        List<Entity.LookupItem> GetEntityLookup(Guid companyId);
        Entity.BaseResponse<Entity.DashboardOverviewResponse> GetOverview(DateTime currentDate, string timeZone);
        Entity.BaseResponse<List<Entity.DashboardOverviewByEntityResponse>> GetOverviewByEntity(DateTime currentDate, string timeZone);
        #region Dynamic Dashboard
        Entity.ActionStatus ManageMasterWidget(Entity.MasterWidget request);
        List<Entity.MasterWidget> GetMasterWidget();
        Entity.MasterWidget GetMasterWidgetById(Guid Id);
        Entity.ActionStatus DeleteMasterWidget(Guid id);

        Entity.ActionStatus ManageUserWidget(Entity.UserDasboardWidget request);
        List<Entity.UserDasboardWidget> GetUserWidget();
        Entity.UserDasboardWidget GetUserWidgetById(Guid Id);
        Entity.ActionStatus DeleteUserWidget(Guid id);
        #endregion
    }
}
