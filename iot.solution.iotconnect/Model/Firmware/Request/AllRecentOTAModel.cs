using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// Recent OTA Model.
    /// </summary>
    public class AllRecentOTAModel
    {
        /// <summary>
        /// Is Recursive ?
        /// </summary>
        public bool isRecursive { get; set; }
        /// <summary>
        /// Entity Guid. Call Entity.All() method to get list of Entity.
        /// </summary>
        public string entityGuid { get; set; }
        /// <summary>
        /// Paging Model.
        /// </summary>
        public PagingModel pagingModel { get; set; }
        /// <summary>
        /// Entries.
        /// </summary>
        public string Entries { get; set; }
    }
}
