using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class User
    {
        public Guid Guid { get; set; }
        public string Email { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid EntityGuid { get; set; }
        public Guid? RoleGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid? TimeZoneGuid { get; set; }
        public string ImageName { get; set; }
        public string ContactNo { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
    }
}
