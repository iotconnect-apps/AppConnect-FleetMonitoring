namespace IoTConnect.Common.Constant
{
    internal class AuthApi
    {
        public const string RefreshTokenUrl = "api/v2/auth/refresh-token";
        public const string BasicTokenUrl = "api/v{0}/auth/basic-token";
        public const string LoginUrl = "api/v{0}/auth/login";
        public const string IdentityUrl = "api/v{0}/User/identity";
    }
}
