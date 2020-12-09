using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class DeviceFiles
    {
        public Guid Guid { get; set; }
        public Guid DeviceGuid { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public string Type { get; set; }
    }
}
