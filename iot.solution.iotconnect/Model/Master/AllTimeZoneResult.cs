namespace IoTConnect.Model
{
    /// <summary>
    /// Returns All Time Zone 
    /// </summary>
    public class AllTimeZoneResult
    {
        /// <summary>
        /// Returns UtcName.
        /// </summary>
        public string utcName { get; set; }
        /// <summary>
        /// Returns OffSet.
        /// </summary>
        public string offset { get; set; }
        /// <summary>
        /// Returns Isdst.
        /// </summary>
        public bool isdst { get; set; }
        /// <summary>
        /// Returns Is Deleted Or Not.
        /// </summary>
        public bool isDeleted { get; set; }
        /// <summary>
        /// Returns Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Returns Name.
        /// </summary>
        public string name { get; set; }
    }
}
