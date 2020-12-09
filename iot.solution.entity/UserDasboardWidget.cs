using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class UserDasboardWidget
    {
        public Guid Guid { get; set; }
        public string DashboardName { get; set; }
        public Object Widgets { get; set; }
        public List<Object> WidgetsList { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSystemDefault { get; set; }
        public Guid UserId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
