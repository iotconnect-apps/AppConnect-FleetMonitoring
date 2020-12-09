using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class KitTypeAttribute
    {

        public Guid Guid { get; set; }
        public Guid? ParentTemplateAttributeGuid { get; set; }
        public Guid TemplateGuid { get; set; }
        public string LocalName { get; set; }
        public string Tag { get; set; }
    }
}
