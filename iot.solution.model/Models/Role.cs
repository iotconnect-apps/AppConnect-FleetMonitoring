using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class Role
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAdminRole { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
