using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class Trip
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid FleetGuid { get; set; }
        public string SourceLocation { get; set; }
        public string DestinationLocation { get; set; }
        public Guid? MaterialTypeGuid { get; set; }
        public string Weight { get; set; }
        public DateTime? StartDateTime { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public string SourceLatitude { get; set; }
        public string SourceLongitude { get; set; }
        public string DestinationLatitude { get; set; }
        public string DestinationLongitude { get; set; }
        public int? TotalMiles { get; set; }
        public string TripId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool IsStarted { get; set; }
        public DateTime? ActualStartDateTime { get; set; }
        public DateTime? EtaEndDateTime { get; set; }
        public int AggressiveAcceleration { get; set; }
        public int HarshBraking { get; set; }
        public int OverSpeed { get; set; }
        public int IdleTime { get; set; }
        public int? CoveredMiles { get; set; }
        public long? odometer{get;set;}
    }
}
