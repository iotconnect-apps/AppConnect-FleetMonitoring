using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class RoleUser
    {
        public Guid Guid { get; set; }
        public Guid RoleGuid { get; set; }
        public Guid UserGuid { get; set; }
        public Guid? CompanyGuid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
