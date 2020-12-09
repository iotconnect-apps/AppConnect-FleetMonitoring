using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class DashboardRoute
    {
        public struct Name
        {
            public const string GetEntity = "dashboard.getcompanylocation";
            public const string GetOverview = "dashboard.getoverview";
            public const string GetOverviewByEntity = "dashboard.getoverviewbyentity";
            public const string GetFleetDetail = "dashboard.getfleetdetail";
            public const string GetTripDetail = "dashboard.gettripdetail";
            public const string GetDriverDetail = "dashboard.getdriverdetail";
            public const string GetDeviceDetail = "dashboard.getdevicedetail";
            public const string GetEntityZone = "dashboard.getentityzone";
            public const string GetEntityDevices = "dashboard.getentitydevices";
            public const string GetEntityChildDevices = "dashboard.getentitychilddevices";

            public const string GetMasterWidget = "configuration.getmasterwidget";
            public const string GetMasterWidgetById = "configuration.getmasterwidgetbyid";
            public const string Manage = "configuration.managemasterwidget";
            public const string DeleteMasterWidget = "configuration.deletemasterwidget";
            public const string GetUserWidget = "configuration.getuserwidget";
            public const string GetUserWidgetById = "configuration.getuserwidgetbyid";
            public const string ManageUserWidget = "configuration.manageuserwidget";
            public const string DeleteUserWidget = "configuration.deleteuserwidget";
        }
        public struct Route
        {
            public const string Global = "api/dashboard";
            public const string GetEntity = "getcompanyentity/{companyId}";
            public const string GetOverview = "overview";
            public const string GetOverviewByEntity = "overviewbyentity";
            public const string GetFleetDetail = "getfleetdetail";
            public const string GetTripDetail = "gettripdetail";
            public const string GetDriverDetail = "getdriverdetail";
            public const string GetDeviceDetail = "getdevicedetail/{deviceId}";
            public const string GetEntityZone = "getentityzone/{facilityId}";
            public const string GetEntityDevices = "getentitydevices/{facilityId}";
            public const string GetEntityChildDevices = "getentitychilddevices/{deviceId}";

            public const string GetMasterWidget = "getmasterwidget";
            public const string GetMasterWidgetById = "getmasterwidgetbyid/{widgetId}";
            public const string Manage = "managemasterwidget";
            public const string DeleteMasterWidget = "deletemasterwidget/{id}";

            public const string GetUserWidget = "getuserwidget";
            public const string GetUserWidgetById = "getuserwidgetbyid/{widgetId}";
            public const string ManageUserWidget = "manageuserwidget";
            public const string DeleteUserWidget = "deleteuserwidget/{id}";
        }
    }
}
