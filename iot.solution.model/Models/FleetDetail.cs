using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class FleetDetail : Fleet
    {
        public string EntityName { get; set; }
        public string SubEntityName { get; set; }
    }

    public partial class FleetModel : Fleet
    {
        public string deviceData { get; set; }

    }
}
