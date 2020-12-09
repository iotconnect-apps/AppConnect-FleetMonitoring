using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class Company
    {
        public Guid Guid { get; set; }
        public Guid EntityGuid { get; set; }
        public Guid? ParentGuid { get; set; }
        public Guid? AdminUserGuid { get; set; }
        public string Name { get; set; }
        public string CpId { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public Guid CountryGuid { get; set; }
        public Guid TimezoneGuid { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid? StateGuid { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
}
