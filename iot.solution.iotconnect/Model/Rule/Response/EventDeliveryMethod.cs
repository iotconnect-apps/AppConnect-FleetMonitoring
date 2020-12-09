using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// All EventDeliveryMethod(s).
    /// </summary>
    public class EventDeliveryMethod
    {
        /// <summary>
        /// Returns Guid
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Returns Name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Returns BitValue
        /// </summary>
        public int bitValue { get; set; }
        /// <summary>
        /// Returns IsPrivate
        /// </summary>
        public bool isPrivate { get; set; }
    }
}
