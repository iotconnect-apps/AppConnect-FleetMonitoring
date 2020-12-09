using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class HardwareKit
    {
        public Guid Guid { get; set; }
        public Guid KitTypeGuid { get; set; }
        public string KitCode { get; set; }
        public Guid? CompanyGuid { get; set; }
        public Guid? EntityGuid { get; set; }
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public Guid? TagGuid { get; set; }
        public bool? IsProvisioned { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
