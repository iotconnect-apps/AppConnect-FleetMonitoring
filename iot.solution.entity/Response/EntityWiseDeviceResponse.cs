using System;
using System.Collections.Generic;

namespace iot.solution.entity.Response
{
    public class EntityWiseDeviceResponse
    {
        public Guid DeviceGuid { get; set; }
        public string Name { get; set; }
        public string UniqueId { get; set; }
        public Guid TemplateGuid { get; set; }
        public bool IsConnected { get; set; }
       
    }
}
