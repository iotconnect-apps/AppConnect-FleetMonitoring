using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public struct RoleRoute
    {
        public struct Name
        {
            public const string Add = "role.add";
            public const string GetList = "role.list";
            public const string GetById = "role.getbyid";
            public const string Delete = "role.delete";
            public const string BySearch = "role.search";
            public const string UpdateStatus = "role.updatestatus";
            public const string GetByCompany = "role.getbycompany";
        }

        public struct Route
        {
            public const string Global = "api/role";
            public const string Manage = "manage";
            public const string GetList = "";
            public const string GetById = "{id}";
            public const string Delete = "delete";
            public const string UpdateStatus = "updatestatus";
            public const string BySearch = "search";
            public const string GetByCompany = "company";
        }
    }
}
