using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public struct DeviceTypeRoute
    {
        public struct Name
        {
            public const string Add = "devicetype.add";
            public const string GetList = "devicetype.list";
            public const string GetById = "devicetype.getproductbyid";
            public const string Delete = "devicetype.deleteproduct";
            public const string DeleteImage = "devicetype.deleteproductimage";
            public const string BySearch = "devicetype.search";
            public const string Status = "devicetype.updatestatus";
        }

        public struct Route
        {
            public const string Global = "api/devicetype";
            public const string Manage = "manage";
            public const string GetList = "";
            public const string GetById = "{id}";
            public const string Delete = "delete";
            public const string DeleteImage = "deleteimage";
            public const string UpdateStatus = "updatestatus";///{id}/{status}
            public const string BySearch = "search";
           

        }
    }
}
