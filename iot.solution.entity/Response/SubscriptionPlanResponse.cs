using System;
using System.Collections.Generic;

namespace iot.solution.entity.Response
{
    public class PlanFeature
    {
        public Guid solutionFeaturesId { get; set; }
        public string featureName { get; set; }
        public string featureValue { get; set; }
        public bool isCustomeFeature { get; set; }
        public bool isInternal { get; set; }
        public string displayName { get; set; }
        public List<SubFeature> subFeatures { get; set; }
    }

    public class SubFeature
    {
        public string solutionFeaturesId { get; set; }
        public string solutionSubFeaturesId { get; set; }
        public string value { get; set; }
        public bool isSelected { get; set; }
        public string subFeatureName { get; set; }
    }
    public class PlanData
    {
        public Guid planId { get; set; }
        public string planName { get; set; }
        public string instance { get; set; }
        public int subscriber { get; set; }
        public double consumerPlanPrice { get; set; }
        public bool status { get; set; }
        public bool isPlanSelected { get; set; }
        public bool isExpired { get; set; }
        public DateTime expiryDate { get; set; }
        public bool isAutoRenew { get; set; }
        public string planCode { get; set; }
        public string solutionMasterId { get; set; }
        public int stopSubscirptionAfterMonth { get; set; }
        public bool isPlanInactiveAndNotExpired { get; set; }
        public bool isSolutionActive { get; set; }
        public string instanceName { get; set; }
        public string solutionName { get; set; }
        public string envName { get; set; }
        public string solutionCode { get; set; }
        public string instanceType { get; set; }
        public List<PlanFeature> features { get; set; }
        public string instanceId { get; set; }
        public long totalAvailableMessages { get; set; }
    }

    public class PlanParameter
    {
        public int count { get; set; }
    }

    public class SubscriptionPlanResponse
    {
        public List<PlanData> data { get; set; }
        public PlanParameter @params { get; set; }
    }
}
