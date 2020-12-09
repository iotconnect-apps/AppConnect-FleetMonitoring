using System;

namespace iot.solution.entity
{
    public class UserResponse
    {
        public Guid Guid { get; set; }
        public string UserId { get; set; }
        public Guid CompanyGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        
      
        public string RoleName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public Guid TimeZoneGuid { get; set; }
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid EntityGuid { get; set; }
        public string EntityName { get; set; }
    }
}
