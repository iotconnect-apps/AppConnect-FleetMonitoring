using System;
using System.Collections.Generic;

namespace iot.solution.entity
{
    public class NotificationAddRequest
    {
        public Guid? Guid { get; set; }
        public List<string> Devices { get; set; }
        public string ParentAttributeGuid { get; set; }
        public string SeverityLevelGuid { get; set; }
        public string EventSubscriptionGuid { get; set; }
        public List<string> DeliveryMethod { get; set; }
        public List<object> RuleAttribute { get; set; }
        public bool IgnorePreference { get; set; }
        public bool IsIncludeChildEntity { get; set; }
        public List<string> Roles { get; set; }
        public string ConditionText { get; set; }
        public int ApplyTo { get; set; }
        public string EntityGuid { get; set; }
        public List<object> Wheaders { get; set; }
        public string Url { get; set; }
        public string ParameterValue { get; set; }
        public string CommandGuid { get; set; }
        public string TemplateGuid { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public List<string> Users { get; set; }
        public int RuleType { get; set; }
    }
}