using System.Collections.Generic;

namespace IoTConnect.Model
{
    /// <summary>
    /// Verify Rule.
    /// </summary>
   public class VerifyRuleResult
    {
        /// <summary>
        /// Gets and sets isValid
        /// </summary>
        public bool isValid { get; set; }
        /// <summary>
        /// Gets and sets attributes
        /// </summary>
        public List<string> attributes { get; set; }
        /// <summary>
        /// Gets and sets tags
        /// </summary>
        public List<object> tags { get; set; }
    }
}
