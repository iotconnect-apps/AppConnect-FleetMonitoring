using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class MasterWidget
    {
        public Guid Guid { get; set; }
        public string Widgets { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
