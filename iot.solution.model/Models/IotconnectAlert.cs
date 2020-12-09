using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class IotconnectAlert
    {
        public Guid Guid { get; set; }
        public string Message { get; set; }
        public Guid? CompanyGuid { get; set; }
        public string Condition { get; set; }
        public Guid? DeviceGuid { get; set; }
        public Guid? EntityGuid { get; set; }
        public DateTime? EventDate { get; set; }
        public string UniqueId { get; set; }
        public string Audience { get; set; }
        public string EventId { get; set; }
        public Guid? RefGuid { get; set; }
        public string Severity { get; set; }
        public string RuleName { get; set; }
        public string Data { get; set; }
    }
}
