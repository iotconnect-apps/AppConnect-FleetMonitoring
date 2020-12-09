namespace iot.solution.entity.Structs.Routes
{
    public struct UserRoute
    {
        public struct Name
        {
            public const string GetById = "user.getbyid";
            public const string GetList = "user.getlist";
            public const string Manage = "user.manage";
            public const string BySearch = "user.search";
            public const string Delete = "user.deleteuser";
            public const string Status = "user.updatestatus";
            public const string Validate = "user.validateuser";
            public const string Identity = "user.identityuser";
            public const string ChangePassword = "user.changepassword";
            public const string RefreshToken = "user.refreshtoken";
            public const string UpdateStatus = "user.updatestatus";
            public const string AdminLogin = "user.adminlogin";
        }

        public struct Route
        {
            public const string Global = "api/user";
            public const string GetList = "";
            public const string Manage = "manage";
            public const string BySearch = "search";
            public const string GetById = "{id}";
            public const string Delete = "delete";
            public const string UpdateStatus = "updatestatus";///{id}/{status}
            public const string Validate = "login";
            public const string Identity = "identity";
            public const string AdminLogin = "adminlogin";
            public const string RefreshToken = "refreshtoken";
            public const string ChangePassword = "changepassword";
        }
    }
}
