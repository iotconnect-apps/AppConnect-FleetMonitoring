

using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Get Device From OTA Update.
    /// </summary>
   public class AllDeviceOtaUpdateModel
    {
        [Required(ErrorMessage = "otaUpdateGuid is Required.")]
        /// <summary>
        /// OTA Update Guid. Call Firmware.AllOTAUpdate() method to get list of OtaUpdate.
        /// </summary>
        public string otaUpdateGuid { get; set; }
        /// <summary>
        /// Status.Should Be : Pending/Sent/success/failed/skipped.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Paging Model.
        /// </summary>
        public PagingModel pagingModel{ get; set; }
    }
}
