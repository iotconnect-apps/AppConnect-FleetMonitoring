using System;
using System.Collections.Generic;

namespace iot.solution.entity.Request
{
    public class ChartRequest
    {
        public Guid CompanyGuid { get; set; }
        public Guid EntityGuid { get; set; }
        public Guid FleetGuid { get; set; }
        public Guid DriverGuid { get; set; }
        public Guid DeviceGuid { get; set; }
        //public Guid HardwareKitGuid { get; set; }
        //public Guid? ProductGuid { get; set; }
        public string Frequency { get; set; }
        public string Attribute { get; set; }
    }
}
