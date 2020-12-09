using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace iot.solution.entity
{
    public class Driver
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid FleetGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public string LicenceNo { get; set; }
        public string LicenceImage { get; set; }
        public string DriverId { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public Guid? StateGuid { get; set; }
        public Guid? CountryGuid { get; set; }
        public string Image { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public int AggressiveAcceleration { get; set; }
        public int OverSpeed { get; set; }
        public int HarshBraking { get; set; }
        public int IdleTime { get; set; }
    }
    public class DriverDetail:Driver
    {
        public string FleetName { get; set; }
        public bool IsEditDelete { get; set; }
    }

    public class DriverModel : Driver
    {
        public IFormFile ImageFile { get; set; }
        public IFormFile LicenceFile { get; set; }

    }
    public class DriverListRequest : ListRequest
    {
        public System.DateTime? currentDate { get; set; }
        public string timeZone { get; set; }
    }
}
