
using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Firmware Model.
    /// </summary>
   public class AddFirmwareModel
    {
        [Required(ErrorMessage ="Firmware Name is Required.")]
        /// <summary>
        /// Name.
        /// </summary>
        public string firmwareName { get; set; }
        /// <summary>
        /// Description.
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
        /// <summary>
        /// (Optional) Firmware Tag.
        /// </summary>
        public string firmwareTag { get; set; }

        [Required(ErrorMessage = "Upload File is Required.")]
        /// <summary>
        /// Firmware File. support only .txt,.jpg and zip file.
        /// </summary>
        public byte[] firmwarefile { get; set; }
    }
}
