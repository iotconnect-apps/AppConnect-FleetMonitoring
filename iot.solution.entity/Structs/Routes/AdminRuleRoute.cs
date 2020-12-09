using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public struct AdminRuleRoute
    {
        public struct Name
        {
            public const string Add = "adminrule.add";
            public const string BySearch = "adminrule.search";
            public const string Delete = "adminrule.delete";
            public const string GetById = "adminrule.getbyid";
            public const string UpdateStatus = "adminrule.updatestatus";
        }

        public struct Route
        {
            public const string Global = "api/adminrule";            
            public const string BySearch = "search";
            public const string Manage = "manage";
            public const string Delete = "delete";
            public const string GetById = "{id}";
            public const string UpdateStatus = "updatestatus";
        }
    }
}
