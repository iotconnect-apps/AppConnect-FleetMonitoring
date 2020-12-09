using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class SingleRuleResponse
    {
        public List<string> Roles { get; set; }
        public string Tag { get; set; }
        public EventSubscription EventSubscription { get; set; }
        public string EventSubscriptionGuid { get; set; }
        public bool IsActive { get; set; }
        public int ApplyTo { get; set; }
        public string EntityGuid { get; set; }
        public bool IgnorePreference { get; set; }
        public string ConditionText { get; set; }
        public string Name { get; set; }
        public string ParentAttributeGuid { get; set; }
        public int RuleType { get; set; }
        public List<string> AttributeGuid { get; set; }
        public string TemplateGuid { get; set; }
        public string CompanyGuid { get; set; }
        public string Guid { get; set; }
        public List<string> Users { get; set; }
        public List<string> Devices { get; set; }
    }

    public class VerifyRuleResult
    {
        public bool isValid { get; set; }
        public List<string> attributes { get; set; }
        public List<object> tags { get; set; }
    }

    public class EventSubscription
    {
        public string Guid { get; set; }
        public string EventGuid { get; set; }
        public int DeliveryMethod { get; set; }
        public List<DeliveryMethodData> DeliveryMethodData { get; set; }
        public string CompanyGuid { get; set; }
        public string SolutionGuid { get; set; }
        public string RefGuid { get; set; }
        public DataXml DataXml { get; set; }
        public string SeverityLevelGuid { get; set; }
    }

    public class DeliveryMethodData
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public int BitValue { get; set; }
    }

    public class DataXml
    {
        public Roleguid Roleguids { get; set; }
        public Userguid Userguids { get; set; }
        public EventCommand Command { get; set; }
        public string Webhook { get; set; }
    }

    public class Roleguid
    {
        public List<string> Roleguids { get; set; }
    }


    public class Userguid
    {
        public List<string> Userguids { get; set; }
    }

    public class EventCommand
    {
        public string Guid { get; set; }
        public object Text { get; set; }
    }
}
