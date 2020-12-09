using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class CompanyRoute
    {
        public struct Name
        {
            public const string Manage = "company.manage";
            public const string GetList = "company.list";
            public const string GetById = "company.getbyid";
            public const string Delete = "company.delete";
            public const string UpdateStatus = "company.updatestatus";
        }

        public struct Route
        {
            public const string Global = "api/company";
            public const string Manage = "manage";
            public const string GetList = "";
            public const string GetById = "{id}";
            public const string Delete = "delete";
            public const string UpdateStatus = "updatestatus";
        }
    }
}
