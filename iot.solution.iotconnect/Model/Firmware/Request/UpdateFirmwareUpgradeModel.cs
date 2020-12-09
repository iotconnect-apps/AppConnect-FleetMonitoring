
using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    public class UpdateFirmwareUpgradeModel
    {
        [Required(ErrorMessage = "Firmware Upgrade Guid is Required.")]
        /// <summary>
        /// Firmware Upgrade Guid. Call Firmware.AllOTAUpgrde() method to get list of FirmwareUpgrade Guid.
        /// </summary>
        public string firmwareUpgradeGuid { get; set; }

        [Required(ErrorMessage = "Firmware Guid is Required.")]
        /// <summary>
        /// Firmware Guid. Call Firmware.All() method to get list of firmware.
        /// </summary>
        public string FirmwareGuid { get; set; }
        /// <summary>
        /// (Optional) Description.
        /// </summary>
        public string Description{ get; set; }

        [Required(ErrorMessage = "Software is Required.")]
        /// <summary>
        /// Software.
        /// </summary>
        public string Software{ get; set; }
        /// <summary>
        /// Firmware File. Pass byte[] of .jpg or txt file.
        /// </summary>
        public byte[] FirmwareFile { get; set; }
    }
}
