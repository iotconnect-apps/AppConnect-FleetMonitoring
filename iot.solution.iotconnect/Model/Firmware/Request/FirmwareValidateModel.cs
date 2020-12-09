
using System.Collections.Generic;

namespace IoTConnect.Model
{
    /// <summary>
    /// Firmware Validate Model.
    /// </summary>
    public class FirmwareValidateModel
    {
        /// <summary>
        /// List of firmwate to validate.
        /// </summary>
        public List<Firmware> firmware { get; set; }
    }

    /// <summary>
    /// Firmware Class.
    /// </summary>
    public class Firmware
    {
        /// <summary>
        /// Code.
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// Hardware.
        /// </summary>
        public string hardware { get; set; }
        /// <summary>
        /// Software.
        /// </summary>
        public string software { get; set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string name { get; set; }
    }
}
