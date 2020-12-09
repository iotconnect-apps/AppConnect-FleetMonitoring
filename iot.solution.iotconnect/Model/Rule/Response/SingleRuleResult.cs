using System.Collections.Generic;

namespace IoTConnect.Model
{
    /// <summary>
    /// Single Rule.
    /// </summary>
    public class SingleRuleResult
    {
        /// <summary>
        /// Returns guid
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Returns companyGuid
        /// </summary>
        public string companyGuid { get; set; }
        /// <summary>
        /// Returns templateGuid
        /// </summary>
        public string templateGuid { get; set; }
        /// <summary>
        /// Returns attributeGuid
        /// </summary>
        public List<string> attributeGuid { get; set; }
        /// <summary>
        /// Returns ruleType
        /// </summary>
        public int ruleType { get; set; }
        /// <summary>
        /// Returns parentAttributeGuid
        /// </summary>
        public string parentAttributeGuid { get; set; }
        /// <summary>
        /// Returns name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Returns conditionText
        /// </summary>
        public string conditionText { get; set; }
        /// <summary>
        /// Returns ignorePreference
        /// </summary>
        public bool ignorePreference { get; set; }
        /// <summary>
        /// Returns entityGuid
        /// </summary>
        public string entityGuid { get; set; }
        /// <summary>
        /// Returns applyTo
        /// </summary>
        public int applyTo { get; set; }
        /// <summary>
        /// Returns isActive
        /// </summary>
        public bool isActive { get; set; }
        /// <summary>
        /// Returns eventSubscriptionGuid
        /// </summary>
        public string eventSubscriptionGuid { get; set; }
        /// <summary>
        /// Returns eventSubscription
        /// </summary>
        public EventSubscription eventSubscription { get; set; }
        /// <summary>
        /// Returns tag
        /// </summary>
        public string tag { get; set; }
        /// <summary>
        /// Returns roles
        /// </summary>
        public List<string> roles { get; set; }
        /// <summary>
        /// Returns users
        /// </summary>
        public List<string> users { get; set; }
        /// <summary>
        /// Returns devices
        /// </summary>
        public List<string> devices { get; set; }
    }
    /// <summary>
    /// EventSubscription.
    /// </summary>
    public class EventSubscription
    {
        /// <summary>
        /// EventSubscription Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Event Guid.
        /// </summary>
        public string eventGuid { get; set; }
        /// <summary>
        /// Delivery Method
        /// </summary>
        public int deliveryMethod { get; set; }
        /// <summary>
        /// List Of Delivery Method Data
        /// </summary>
        public List<DeliveryMethodData> deliveryMethodData { get; set; }
        /// <summary>
        /// Company Guid
        /// </summary>
        public string companyGuid { get; set; }
        /// <summary>
        /// Solution Guid
        /// </summary>
        public string solutionGuid { get; set; }
        /// <summary>
        /// Ref Guid
        /// </summary>
        public string refGuid { get; set; }
        /// <summary>
        /// Data Xml
        /// </summary>
        public DataXml dataXml { get; set; }
        /// <summary>
        /// Severity Level Guid
        /// </summary>
        public string severityLevelGuid { get; set; }
    }

    public class DeliveryMethodData
    {
        public string guid { get; set; }
        public string name { get; set; }
        public int bitValue { get; set; }
    }

    public class Roleguids
    {
        public List<string> roleguid { get; set; }
    }

    public class Userguids
    {
        public List<string> userguid { get; set; }
    }

    public class EventCommand
    {
        public string guid { get; set; }
        public object text { get; set; }
    }

    public class DataXml
    {
        public Roleguids roleguids { get; set; }
        public Userguids userguids { get; set; }
        public EventCommand command { get; set; }
        public object webhook { get; set; }
    }
}
