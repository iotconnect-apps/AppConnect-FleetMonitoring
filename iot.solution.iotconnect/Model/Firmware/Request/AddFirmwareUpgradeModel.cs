

using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    public class AddFirmwareUpgradeModel
    {
        [Required(ErrorMessage = "FirmwareGuid is Required.")]
        /// <summary>
        /// Firmware Guid.
        /// </summary>
        public string firmwareGuid { get; set; }
        /// <summary>
        ///(Optional) Description.
        /// </summary>
        public string description { get; set; }

        [Required(ErrorMessage = "Software is Required.")]
        /// <summary>
        /// Software.
        /// </summary>
        public string software { get; set; }        
        /// <summary>
        /// (Optional) Firmware File. Supports .jpg,.txt and zip file.
        /// </summary>
        public byte[] firmwarefile { get; set; }
    }
}
