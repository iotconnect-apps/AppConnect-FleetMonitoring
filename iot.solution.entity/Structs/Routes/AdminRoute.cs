using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class AdminRoute
    {
        public struct Name
        {
            public const string GetById = "adminuser.getbyid";
            public const string GetList = "adminuser.getlist";
            public const string Manage = "manage";
            //public const string BySearch = "adminuser.search";
            public const string Delete = "adminuser.deleteuser";
            public const string UpdateStatus = "adminuser.updatestatus";
            //public const string Status = "adminuser.updatestatus";
            //public const string Validate = "adminuser.validateuser";
            public const string ChangePassword = "adminuser.changepassword";
            //public const string RefreshToken = "adminuser.refreshtoken";
            //public const string UpdateStatus = "adminuser.updatestatus";
            //public const string AdminLogin = "adminuser.adminlogin";
        }

        public struct Route
        {
            public const string Global = "api/adminuser";
            public const string GetList = "list";
            public const string Manage = "manage";
            //public const string BySearch = "search";
            public const string GetById = "{id}";
            public const string Delete = "delete";
            public const string UpdateStatus = "updatestatus";
            //public const string UpdateStatus = "updatestatus";
            //public const string Validate = "login";
            //public const string AdminLogin = "adminlogin";
            //public const string RefreshToken = "refreshtoken";
            public const string ChangePassword = "changepassword";
        }
    }
}
