using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public struct DriverRoute
    {
        public struct Name
        {
            public const string Manage = "driver.manage";
            public const string GetList = "driver.list";
            public const string GetById = "driver.getdevicebyid";
            public const string Delete = "driver.deletedevice";
            public const string DeleteLicenceImage = "driver.deletelicenceimage";
            public const string DeleteImage = "driver.deleteimage";
            public const string BySearch = "driver.search";
            public const string UpdateStatus = "driver.updatestatus";
        }

        public struct Route
        {
            public const string Global = "api/driver";
            public const string Manage = "manage";
            public const string GetList = "";
            public const string GetById = "{id}";
            public const string Delete = "delete";
            public const string DeleteLicenceImage = "deletelicenceimage";
            public const string DeleteImage = "deleteimage";
            public const string BySearch = "search";
            public const string UpdateStatus = "updatestatus";
        }
    }
}
