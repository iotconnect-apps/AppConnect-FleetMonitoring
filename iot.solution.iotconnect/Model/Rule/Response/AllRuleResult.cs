using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// All Rule(s).
    /// </summary>
    public class AllRuleResult
    {
        /// <summary>
        /// Returns rule guid
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Returns name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Returns template
        /// </summary>
        public string template { get; set; }
        /// <summary>
        /// Returns entity
        /// </summary>
        public string entity { get; set; }
        /// <summary>
        /// Returns ruleType
        /// </summary>
        public string ruleType { get; set; }
        /// <summary>
        /// Returns conditionText
        /// </summary>
        public string conditionText { get; set; }
        /// <summary>
        /// Returns userCount
        /// </summary>
        public int userCount { get; set; }
        /// <summary>
        /// Returns roleCount
        /// </summary>
        public int roleCount { get; set; }
        /// <summary>
        /// Returns createdby
        /// </summary>
        public string createdby { get; set; }
        /// <summary>
        /// Returns updatedby
        /// </summary>
        public string updatedby { get; set; }
        /// <summary>
        /// Returns createddate
        /// </summary>
        public DateTime createddate { get; set; }
        /// <summary>
        /// Returns updateddate
        /// </summary>
        public DateTime updateddate { get; set; }
        /// <summary>
        /// Returns isActive
        /// </summary>
        public bool isActive { get; set; }
    }
}
