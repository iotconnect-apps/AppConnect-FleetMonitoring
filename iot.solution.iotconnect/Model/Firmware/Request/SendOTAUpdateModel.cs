using System.ComponentModel.DataAnnotations;


namespace IoTConnect.Model
{
    /// <summary>
    /// OTA Update Send Model.
    /// </summary>
   public class SendOTAUpdateModel
    {
        [Required(ErrorMessage = "Firmware Upgrade Guid is Required.")]
        /// <summary>
        /// Firmware Upgrade Guid. Call Firmware.All() method to get list of Firmware.
        /// </summary>
        public string firmwareUpgradeGuid { get; set; }

        [Required(ErrorMessage = "Entity Guid is Required.")]
        /// <summary>
        /// Entity Guid. Call Entity.All() method to get list of entity.
        /// </summary>
        public string entityGuid { get; set; }
        /// <summary>
        /// (Optional) isRecursive ?.
        /// </summary>
        public bool isRecursive { get; set; }
        /// <summary>
        /// (Optional) is Force Update ?.
        /// </summary>
        public bool isForceUpdate { get; set; }
        /// <summary>
        /// (Optional) schedule On. Should Be YYYY-mm-dd HH:mm:ss Formate. with User TimeZone.
        /// </summary>
        public string scheduledOn { get; set; }
    }
}
