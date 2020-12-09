using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IoTConnect.Model
{
    /// <summary>
    /// Update OTA Status Model
    /// </summary>
    public class UpdateOTAStatusModel
    {

        /// <summary>
        /// Device Guid. Call Device.All() method to get list of device. Pass either deviceguid or uniqueid.
        /// </summary>
        public string deviceGuid { get; set; }

        /// <summary>
        /// unique Id. Call Device.All() method to get list of device. Pass either deviceguid or uniqueid.
        /// </summary>
        public string uniqueId { get; set; }

        [Required(ErrorMessage = "Ota Update Item Guid is Required.")]
        /// <summary>
        /// OTA Update Item Guid. Call Firmware.AllOTAupgrade() method to get list of entity.
        /// </summary>
        public string otaUpdateItemGuid { get; set; }

        [Required(ErrorMessage = "Status is Required.")]
        /// <summary>
        /// Status. should be Pending/Sent/success/failed/skipped.
        /// </summary>
        public string status { get; set; }

    }
}
