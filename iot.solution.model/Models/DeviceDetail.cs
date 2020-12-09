using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace iot.solution.model.Models
{
    public partial class DeviceDetail : Device
    {
        public string EntityName { get; set; }
        public string SubEntityName { get; set; }
    }
   
    public partial class DeviceModel : Device
    {
       
       
    }
   
}
