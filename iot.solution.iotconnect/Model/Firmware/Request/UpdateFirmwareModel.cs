using System;
using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Update Firmware Model.
    /// </summary>
    public class UpdateFirmwareModel
    {
        [Required(ErrorMessage = "Firmware Guid is Required.")]
        /// <summary>
        /// Firmware Guid. Call Firmware.All() method to get list of Firmware.
        /// </summary>
        public string FirmwareGuid { get; set; }

        [Required(ErrorMessage = "Firmware Name is Required.")]
        /// <summary>
        /// Name.
        /// </summary>
        public string firmwareName { get; set; }
        /// <summary>
        /// (Optional) Description.
        /// </summary>
        public string firmwareDescription { get; set; }

        [Required(ErrorMessage = "Hardware Version is Required.")]
        /// <summary>
        /// Hardware.
        /// </summary>
        public string hardware { get; set; }

        [Required(ErrorMessage = "Software Version is Required.")]
        /// <summary>
        /// Software.
        /// </summary>
        public string software { get; set; }

        [Required(ErrorMessage = "Status is Required.")]
        /// <summary>
        /// Firmware Status. Pending/Sent/success/failed/skipped.
        /// </summary>
        public string status { get; set; }

    }
}
