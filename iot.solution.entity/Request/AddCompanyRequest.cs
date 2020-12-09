using System;
using System.ComponentModel.DataAnnotations;

namespace iot.solution.entity
{
    public class AddCompanyRequest : Company
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid UserID { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid RoleGuid { get; set; }
    }
    public class OverviewCompanyRequest
    {
       
        [Required]
        public DateTime? currentDate { get; set; }
        [Required]
        public string timeZone { get; set; }
    }

 }
