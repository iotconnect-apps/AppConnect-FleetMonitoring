using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class RuleRoute
    {
        public struct Name
        {
            public const string Manage = "rule.manage";
            public const string Verify = "rule.verify";
            public const string GetById = "rule.getrulebyid";
            public const string Delete = "rule.deleterule";
            public const string List = "rule.list";
            public const string UpdateStatus = "rule.updatestatus";
        }

        public struct Route
        {
            public const string Global = "api/rule";
            public const string Manage = "manage";
            public const string Verify = "verify";
            public const string GetById = "{id}";
            public const string Delete = "delete";
            public const string List = "";
            public const string UpdateStatus = "updatestatus";
        }
    }
}
