using System;
using System.Collections.Generic;
using System.Text;

namespace IoTConnect.Model
{
    /// <summary>
    /// AcquireDeviceModel Class.
    /// </summary>
    public class AcquireDeviceModel
    {
        /// <summary>
        /// Primary Thumb Print.
        /// </summary>
        public string primaryThumbprint { get; set; }
        /// <summary>
        /// Secondary Thumb Print.
        /// </summary>
        public string secondaryThumbprint { get; set; }
    }
}
