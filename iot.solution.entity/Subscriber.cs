using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{

    public class SubscriberData
    {
        public string subscriberName { get; set; }
        public string email { get; set; }
        public string solutionName { get; set; }
        public string planName { get; set; }
        public string planCode { get; set; }
        public bool isAutoRenew { get; set; }
        public string ioTConnectCPId { get; set; }
        public bool subscriptionStatus { get; set; }
        public DateTime subscriptionStartDate { get; set; }
        public DateTime subscriptionEndDate { get; set; }
        public bool isSubscriptionCancelled { get; set; }
        public string environmentInstanceName { get; set; }
        public string productCode { get; set; }
        public string solutionId { get; set; }
        public bool isCompanyCreatedWithoutPlan { get; set; }
        public string companyName { get; set; }
        public string ioTConnectCompanyGuid { get; set; }
    }

    public class SubscriberParameter
    {
        public int count { get; set; }
    }

    public class Subscriber
    {
        public List<SubscriberData> data { get; set; }
        public SubscriberParameter @params { get; set; }
    }
}
