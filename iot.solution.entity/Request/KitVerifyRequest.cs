using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class HardwareKitDTO
    {
        public Guid Guid { get; set; }
        public Guid? KitTypeGuid { get; set; }
        public string KitCode { get; set; }
        public Guid? CompanyGuid { get; set; }
        public Guid? KitGuid { get; set; }
        public string ParentUniqueId { get; set; }
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public Guid? Tag { get; set; }
        public bool? IsProvisioned { get; set; }
    }


    public class KitVerifyRequest 
    {     
        public string CompanyGuid { get; set; }
        public string KitTypeGuid { get; set; }
        public List<HardwareKitRequest> HardwareKits { get; set; }
    }
}
