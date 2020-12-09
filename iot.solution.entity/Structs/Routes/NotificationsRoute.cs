using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public struct NotificationsRoute
    {
        public struct Name
        {
            public const string Add = "notifications.add";            
            public const string Delete = "notifications.delete";
            public const string BySearch = "notifications.search";
            public const string UpdateStatus = "notifications.updatestatus";
            public const string GetById = "notifications.getbyid";
            public const string Verify = "notifications.verify";
        }

        public struct Route
        {
            public const string Global = "api/notifications";
            public const string Manage = "manage";
            public const string Verify = "verify";
            public const string Delete = "delete";            
            public const string BySearch = "search";
            public const string UpdateStatus = "updatestatus";
            public const string GetById = "{id}";
        }
    }
}
