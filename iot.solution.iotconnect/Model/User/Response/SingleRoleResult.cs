using System.Collections.Generic;

namespace IoTConnect.Model
{
    /// <summary>
    /// Role Result.
    /// </summary>
    public class SingleRoleResult
    {
        /// <summary>
        /// Guid.
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// Role Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Role Description.
        /// </summary>
        public object Description { get; set; }

        internal List<Solution> Solutions { get; set; }
    }
    /// <summary>
    /// Returns Solution GUIDs to subscribe for a singe role. Not needed in RDK.
    /// </summary>
    public class Solution
    {
        public string Guid { get; set; }
    }

}
