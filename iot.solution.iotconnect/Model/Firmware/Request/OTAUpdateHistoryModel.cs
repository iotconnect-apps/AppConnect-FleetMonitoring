
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// OTA Update history Model
    /// </summary>
    public class OTAUpdateHistoryModel
    {
        [Required(ErrorMessage = "Device Guid (s) is Required.")]
        /// <summary>
        /// List of device guid.
        /// </summary>
        public List<Device> device { get; set; }
        /// <summary>
        /// Paging  Model.
        /// </summary>
        public PagingModel pagingModel { get; set; }
    }

    /// <summary>
    /// Device Class.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Device Guid. Call Device.All() method to get list of deviceGuid.
        /// </summary>
        public string deviceguid { get; set; }
    }
}
