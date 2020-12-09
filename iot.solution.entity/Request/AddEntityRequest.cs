using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace iot.solution.entity
{
    public class AddEntityRequest
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string ParentEntityGuid { get; set; }
        [Required]
        [MaxLength(50)]
        public string City { get; set; }
        [Required]
        [MaxLength(7)]
        public string Zipcode { get; set; }
        [Required]
        public Guid? StateGuid { get; set; }
        [Required]
        public Guid? CountryGuid { get; set; }
        public string Image { get; set; }
        [Required]
        [MaxLength(20)]
        public string Latitude { get; set; }
        [Required]
        [MaxLength(20)]
        public string Longitude { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
    public class EntityModel : Entity
    {
        public IFormFile ImageFile { get; set; }

    }
    
         public class OverviewEntityRequest
    {
        [Required]
        public string fleetGuid { get; set; }
        [Required]
        public DateTime? currentDate { get; set; }
        [Required]
        public string timeZone { get; set; }
    }
    public class OverviewDriverRequest
    {
        [Required]
        public string driverGuid { get; set; }
        [Required]
        public DateTime? currentDate { get; set; }
        [Required]
        public string timeZone { get; set; }
    }
    public class OverviewTripRequest
    {
        [Required]
        public string tripGuid { get; set; }
        [Required]
        public DateTime? currentDate { get; set; }
        [Required]
        public string timeZone { get; set; }
    }
}
