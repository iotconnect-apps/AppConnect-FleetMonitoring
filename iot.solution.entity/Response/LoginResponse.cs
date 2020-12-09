using System;

namespace iot.solution.entity
{
    public class UserDetail
    {
        public string Id { get; set; }
        public string CompanyId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string CPId { get; set; }
        public string EntityGuid { get; set; }
        public string SolutionGuid { get; set; }
        public string SolutionKey { get; set; }
        public bool IsAdmin { get; set; }
        public string FullName { get; set; }
    }

    public class LoginResponse
    {
        public int status { get; set; }
        public string data { get; set; }
        public string message { get; set; }
        public string token_type { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public long expires_in { get; set; }
        public string company_id { get; set; }
        public UserDetail UserDetail { get; set; }
    }
}
