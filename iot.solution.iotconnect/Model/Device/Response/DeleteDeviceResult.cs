namespace IoTConnect.Model
{
    /// <summary>
    /// Delete device Result.    
    /// </summary>
   public class DeleteDeviceResult
    {
        /// <summary>
        /// Device Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Device Status.
        /// </summary>
        public bool status { get; set; }
        /// <summary>
        /// Device Entity Guid.
        /// </summary>
        public string entityGuid { get; set; }
        /// <summary>
        /// Device Uniq Identifier.
        /// </summary>
        public string uniqueId { get; set; }
        /// <summary>
        /// Parent Devie Guid.
        /// </summary>
        public object parentDeviceGuid { get; set; }
        /// <summary>
        /// Event Place Holders.
        /// </summary>
        public string eventPlaceHolders { get; set; }
    }
}
