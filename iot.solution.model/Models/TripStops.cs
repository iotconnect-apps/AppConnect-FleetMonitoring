using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class TripStops
    {
        public Guid Guid { get; set; }
        public Guid TripGuid { get; set; }
        public string StopName { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
