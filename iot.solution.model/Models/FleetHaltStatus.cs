using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class FleetHaltStatus
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid FleetGuid { get; set; }
        public Guid DriverGuid { get; set; }
        public DateTime? HaltStartDateTime { get; set; }
        public DateTime? HaltEndDateTime { get; set; }
        public DateTime LastUpdatedHaltDateTime { get; set; }
        public Guid TripGuid { get; set; }
    }
}
