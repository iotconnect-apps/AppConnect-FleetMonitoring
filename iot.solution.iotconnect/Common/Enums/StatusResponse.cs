using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IoTConnect.Common
{
    /// <summary>
    /// IotConnect Status Response (Success, Failed )
    /// </summary>
    internal enum StatusResponse
    {
        [Description("Success")]
        Success,
        [Description("Failed")]
        Failed
    }
}
