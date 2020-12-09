namespace IoTConnect.Common.Constant
{
    internal class UserApi
    {
        internal const string CompanyUser = "api/v{0}/User";
        internal const string Add = "api/v{0}/User";
        internal const string AddRole = "api/v{0}/Role";
        internal const string Update = "api/v{0}/User/{1}";
        internal const string Delete = "api/v{0}/User/{1}";
        internal const string Get = "api/v{0}/User/{1}";
        internal const string Search = "api/v{0}/User";
        internal const string UserEntity = "api/v{0}//entity/{1}/user";
        internal const string UserRole = "api/v{0}//role/{1}/user";
        internal const string RoleDetail = "api/v{0}/Role/{1}";
        internal const string RoleLookup = "api/v{0}/Role/{1}";
        internal const string Count = "api/v{0}/User/count";
        internal const string ForgotPassword = "api/v{0}/User/forgot-password";
        internal const string ChangePassword = "api/v{0}/User/change-password";
        internal const string ResetPassword = "api/v{0}/User/reset-password";
        internal const string UpdateStatus = "api/v{0}/User/{1}/status";
        internal const string UpdateRoleStatus = "api/v{0}/Role/{1}/status";
        internal const string Roles = "api/v{0}/Role/role-user";
        internal const string TimeZones = "api/v{0}/Master/timezone";
        internal const string UpdateRole = "api/v{0}/Role/{1}";
        internal const string DeleteRole = "api/v{0}/Role/{1}";
        internal const string QuotaExhaustedNotification = "api/v{0}/User/quotaExhaustedNotification";
    }
}
