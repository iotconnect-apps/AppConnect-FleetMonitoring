using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class Module
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public int? Permission { get; set; }
        public int? ApplyTo { get; set; }
        public string CategoryName { get; set; }
        public double? ModuleSequence { get; set; }
        public bool IsAdminModule { get; set; }
    }
}
