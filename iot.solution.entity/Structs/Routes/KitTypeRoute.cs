using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public struct KitTypeRoute
    {
        public struct Name
        {
            public const string Get = "kitype.get";
            public const string GetTypeDetail = "kitype.getdetail";
            public const string GetAttributes = "kitype.attributes";
            public const string GetCommands = "kitype.commands";
        }

        public struct Route
        {
            public const string Global = "api/kitype";

            public const string Get = "";
            public const string GetTypeDetail = "get/{templateId}";
            public const string GetAttributes = "attributes/{templateId}";
            public const string GetCommands = "commands/{templateId}";
        }
    }
}
