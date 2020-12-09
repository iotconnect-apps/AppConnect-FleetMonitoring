

using System.Collections.Generic;

namespace IoTConnect.Model
{
   public class CommandsExecuteModel
    {
        /// <summary>
        /// Command Guid.
        /// </summary>
        public string commandGuid { get; set; }
        /// <summary>
        /// Param Value.
        /// </summary>
        public string parameterValue { get; set; }
        /// <summary>
        /// apply to.
        /// </summary>
        public int applyTo { get; set; }
        /// <summary>
        /// Device (s) guid.
        /// </summary>
        public List<string> devices { get; set; }
        /// <summary>
        /// Entity Guid.
        /// </summary>
        public string entityGuid { get; set; }
        /// <summary>
        /// Is Recursive ?
        /// </summary>
        public bool isRecursive { get; set; }
    }
}
