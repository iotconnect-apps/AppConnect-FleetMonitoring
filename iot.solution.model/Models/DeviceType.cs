using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class DeviceType
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public Guid EntityGuid { get; set; }
        public Guid TemplateGuid { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
