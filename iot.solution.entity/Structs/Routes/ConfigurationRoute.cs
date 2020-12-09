using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class ConfigurationRoute
    {
        public struct Name
        {
            public const string Get = "configuration.get";
        }

        public struct Route
        {
            public const string Global = "api/configuration";
            public const string Get = "";
        }
    }
}
