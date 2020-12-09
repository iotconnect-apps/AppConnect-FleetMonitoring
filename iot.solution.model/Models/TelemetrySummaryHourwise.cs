using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class TelemetrySummaryHourwise
    {
        public Guid Guid { get; set; }
        public Guid DeviceGuid { get; set; }
        public DateTime Date { get; set; }
        public string Attribute { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public decimal? Avg { get; set; }
        public decimal? Latest { get; set; }
        public decimal? Sum { get; set; }
    }
}
