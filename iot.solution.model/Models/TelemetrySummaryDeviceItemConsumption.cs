using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class TelemetrySummaryDeviceItemConsumption
    {
        public Guid Guid { get; set; }
        public Guid DeviceGuid { get; set; }
        public DateTime Date { get; set; }
        public string Attribute { get; set; }
        public long? Qty { get; set; }
    }
}
