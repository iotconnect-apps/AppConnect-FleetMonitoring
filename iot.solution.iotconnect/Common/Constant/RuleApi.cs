using System;
using System.Collections.Generic;
using System.Text;

namespace IoTConnect.Common.Constant
{
    internal class RuleApi
    {
        internal const string AllRules = "api/v{0}/Rule";
        internal const string RuleDetail = "api/v{0}/Rule/{1}";
        internal const string RuleCount = "api/v{0}/Rule/counters";
        internal const string RuleDelete = "api/v{0}/Rule/{1}";
        internal const string RuleVerify = "api/v{0}/Rule/verify";
        internal const string RuleAdd = "api/v{0}/Rule";
        internal const string RuleUpdate = "api/v{0}/Rule/{1}";
        internal const string RuleStatusUpdate = "api/v{0}/Rule/{1}/status";
        internal const string EventLookUp = "api/v{0}/Event";
        internal const string EventDeliveryMethod = "api/v{0}/Event/{1}/delivery-method";
        internal const string SeverityLevelLookup = "api/v{0}/severity-level/lookup";

    }
}
