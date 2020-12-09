using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class DeviceMaintenanceRoute
    {
        public struct Name
        {
            public const string Add = "Devicemaintenance.add";
            public const string GetList = "Devicemaintenance.list";
            public const string GetStatusList = "Devicemaintenance.statuslist";
            public const string GetById = "Devicemaintenance.getbyid";
            public const string Delete = "Devicemaintenance.delete";
            public const string BySearch = "Devicemaintenance.search";
            public const string UpdateStatus = "Devicemaintenance.updatestatus";
            public const string UpComingList = "Devicemaintenance.upcoming";
            public const string Complete = "Devicemaintenance.complete";
            public const string GetScheduledMaintenenceDate = "Devicemaintenance.getscheduledMaintenancedate";
        }

        public struct Route
        {
            public const string Global = "api/devicemaintenance";
            public const string Manage = "manage";
            public const string GetList = "";
            public const string UpComingList = "upcoming";
            public const string Complete = "complete";
            public const string GetStatusList = "status";
            public const string GetById = "{id}";
            public const string Delete = "delete";
            public const string UpdateStatus = "updatestatus";
            public const string BySearch = "search";
            public const string GetScheduledMaintenenceDate = "getscheduledMaintenancedate";
        }
    }
}
