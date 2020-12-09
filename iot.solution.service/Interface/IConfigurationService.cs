using iot.solution.entity.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.service.Interface
{
    public interface IConfigurationService
    {
        public ConfgurationResponse GetConfguration(string key);
    }
}
