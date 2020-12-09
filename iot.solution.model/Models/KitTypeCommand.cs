using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class KitTypeCommand
    {
        public Guid Guid { get; set; }
        public Guid TemplateGuid { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }
        public bool RequiredParam { get; set; }
        public bool RequiredAck { get; set; }
        public bool IsOtacommand { get; set; }
        public string Tag { get; set; }
    }
}
