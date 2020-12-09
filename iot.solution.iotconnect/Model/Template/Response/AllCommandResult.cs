using System;

namespace IoTConnect.Model
{
    public class AllCommandResult
    {
        /// <summary>
        /// Command Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Command Name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Command
        /// </summary>
        public string command { get; set; }
        /// <summary>
        /// Created Date.
        /// </summary>
        public DateTime createdDate { get; set; }
        /// <summary>
        /// Created By.
        /// </summary>
        public string createdby { get; set; }
        /// <summary>
        /// Updated Date.
        /// </summary>
        public DateTime updatedDate { get; set; }
        /// <summary>
        /// Required Param ?
        /// </summary>
        public bool requiredParam { get; set; }
        /// <summary>
        /// Required Ack ?
        /// </summary>
        public bool requiredAck { get; set; }
        /// <summary>
        /// Is OTA Command.
        /// </summary>
        public bool isOTACommand { get; set; }
        /// <summary>
        /// Template Tag.
        /// </summary>
        public string tag { get; set; }
    }
}
