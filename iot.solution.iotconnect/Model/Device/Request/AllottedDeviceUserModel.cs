

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Allotted Device User Model.
    /// </summary>
    public class AllottedDeviceUserModel
    {
        /// <summary>
        /// Device Guid (s).
        /// </summary>
        [Required(ErrorMessage = "Requried device guid (s)")]
        public List<string> deviceGuids { get; set; }
        /// <summary>
        /// Permissions.
        /// </summary>
        [Required(ErrorMessage = "Select at least one permission.")]
        public Permission permission { get; set; }
    }
}
