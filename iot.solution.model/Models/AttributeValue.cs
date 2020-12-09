using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class AttributeValue
    {
        public Guid Guid { get; set; }
        public Guid? CompanyGuid { get; set; }
        public string LocalName { get; set; }
        public string UniqueId { get; set; }
        public string Tag { get; set; }
        public string AttributeValue1 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? SdkUpdatedDate { get; set; }
        public DateTime? GatewayUpdatedDate { get; set; }
        public DateTime? DeviceUpdatedDate { get; set; }
        public int AggregateType { get; set; }
    }
}
