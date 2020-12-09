using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// All Recent Activity Result.
    /// </summary>
  public  class AllRecentActivityResult
    {
        /// <summary>
        /// Device Unique Id.
        /// </summary>
        public string uniqueId { get; set; }
        /// <summary>
        /// Hardware.
        /// </summary>
        public string hardware { get; set; }
        /// <summary>
        /// Status.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Updated Date.
        /// </summary>
        public DateTime updatedDate { get; set; }
    }
}
