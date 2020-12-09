using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class CompanyConfig
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid ConfigurationGuid { get; set; }
        public string Value { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
