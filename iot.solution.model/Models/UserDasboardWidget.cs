using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class UserDasboardWidget
    {
        public Guid Guid { get; set; }
        public string DashboardName { get; set; }
        public string Widgets { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSystemDefault { get; set; }
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
