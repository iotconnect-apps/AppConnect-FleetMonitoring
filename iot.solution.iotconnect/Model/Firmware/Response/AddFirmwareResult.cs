
namespace IoTConnect.Model
{
    /// <summary>
    /// Returns Firmware
    /// </summary>
    public class AddFirmwareResult
    {
        /// <summary>
        /// New Guid.
        /// </summary>
        public string newId { get; set; }
        /// <summary>
        /// Firmware Upgrade Guid.
        /// </summary>
        public string firmwareUpgradeGuid { get; set; }
    }
}
