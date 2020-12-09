using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Response
{
    public class FleetDelayData
    {
        public string FleetName { get; set; }
        public DateTime StartDateTime { get; set; }
        public string SourceLocation { get; set; }
        public string destinationLocation { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerName { get; set; }
        public string DriverEmail { get; set; }
        public string DriverName { get; set; }
        public int DelayInMin { get; set; }
    }
}
