
namespace IoTConnect.Model
{
    /// <summary>
    /// Recent Activity Model
    /// </summary>
   public class RecentActivityModel
    {
        /// <summary>
        /// Should Be : Pending/Sent/success/failed/skipped. Pass Empty If you not want to filter.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Number of OTA Update statistics to display. Pass Empty If you not want to filter.
        /// </summary>
        public int  count { get; set; }

    }
}
