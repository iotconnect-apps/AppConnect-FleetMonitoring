using System.Collections.Generic;

namespace iot.solution.entity
{
    public class SubsciberCompanyDetails
    {
        public string companyName { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public int noOfMessages { get; set; }
        public int noOfUsers { get; set; }
        public int noOfDevices { get; set; }
        public int asa { get; set; }
        public int mqtt { get; set; }
        public int timeseries { get; set; }
        public int ruleEngine { get; set; }
        public int solutionSync { get; set; }
        public int connectionStatus { get; set; }
        public int diagnostic { get; set; }
        public int command { get; set; }
        public int twin { get; set; }
        public int d2c { get; set; }
        public int notification { get; set; }
        public object renewalDate { get; set; }
        public List<SolutionDetail> solutionDetails { get; set; }
        public string planName { get; set; }
        public string solutionId { get; set; }
        public int totalMessage { get; set; }
        public double price { get; set; }
        public object subscriptionDate { get; set; }
        public int planNoOfMessages { get; set; }
        public double planPerDayMessages { get; set; }
        public object subFeaturesConsumptionDatas { get; set; }
    }

    public class CompanyUserDetails
    {
        public string companyName { get; set; }
        public string role { get; set; }
        public bool isExist { get; set; }
        public string errorMessage { get; set; }
    }
    public class ValidateUserRequest
    {
        public string solutionId { get; set; }
        public string email { get; set; }
        public string companyName { get; set; }
    }

}
