using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class RoleModulePermission
    {
        public Guid Guid { get; set; }
        public Guid RoleGuid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid ModuleGuid { get; set; }
        public int? Permission { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
