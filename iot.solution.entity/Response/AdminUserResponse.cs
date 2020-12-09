using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class AdminUserResponse
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }

        public string Password { get; set; }
    }
}
