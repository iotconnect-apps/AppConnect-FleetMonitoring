using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class CompanyConfigRoute
    {
        public struct Name
        {
            public const string Manage = "companyconfig.manage";
            public const string GetList = "companyconfig.list";
            public const string GetById = "companyconfig.getbyid";
            public const string Delete = "companyconfig.delete";
        }
        public struct Route
        {
            public const string Global = "api/companyconfig";
            public const string Manage = "manage";
            public const string GetList = "";
            public const string GetById = "{id}";
            public const string Delete = "delete";
        }
    }
}
