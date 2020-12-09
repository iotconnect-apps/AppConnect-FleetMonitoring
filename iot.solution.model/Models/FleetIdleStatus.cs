using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class FleetIdleStatus
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid FleetGuid { get; set; }
        public DateTime? IdleStartDateTime { get; set; }
        public DateTime? IdleEndDateTime { get; set; }
        public DateTime LastUpdatedIdleDateTime { get; set; }
        public Guid? DriverGuid { get; set; }
        public Guid TripGuid { get; set; }
    }
}
