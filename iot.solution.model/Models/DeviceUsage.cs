using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class DeviceUsage
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid DeviceGuid { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
