using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public struct AlertRoute
    {
        public struct Name
        {
            public const string BySearch = "alert.search";
            public const string Manage = "alert.add";
            public const string List = "alert.list";
        }

        public struct Route
        {
            public const string Global = "api/alert";
            public const string BySearch = "search";
            public const string Manage = "addiotalert";
            public const string GetById = "{id}";
            public const string List = "";
        }
    }
}
