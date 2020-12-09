using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace iot.solution.entity
{
    public class ValidateCompanyRequest
    {
        public string CompanyName { get; set; }
        [Required]
        public string Email { get; set; }
    }
    public class CompanyListItem
    {
        public string email { get; set; }
        public string companyName { get; set; }
    }
    public class CompanyList
    {
        public List<CompanyListItem> data { get; set; }
    }
    public class SaveCompanyRequest
    {
        public string SubscriptionToken { get; set; }
        public string SolutionCode { get; set; }
        public string SolutionPlanCode { get; set; }        
        public bool IsAutoRenew { get; set; }
        public string StripeToken { get; set; }
        public string StripeCardId { get; set; }
        public string SubscriberId { get; set; }
        public UserData User { get; set; }
    }

    public class UserData
    {
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string PhoneCountryCode { get; set; }
        public string Address { get; set; }
        public string CountryId { get; set; }
        public string StateId { get; set; }
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public string TimezoneId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CpId { get; set; }
    }
}
