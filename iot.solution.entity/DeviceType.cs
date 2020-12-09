using iot.solution.entity.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace iot.solution.entity
{
    
    public class DeviceType
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        [MaxLength(200)]
        public string Make { get; set; }
        [Required]
        [MaxLength(200)]
        public string Model { get; set; }
        [Required]
        [MaxLength(200)]
        public string Manufacturer { get; set; }
        [Required]
        public Guid EntityGuid { get; set; }
        [Required]
        public Guid TemplateGuid { get; set; }
        public bool IsActive { get; set; }
       // public bool IsDeleted { get; set; }
        //public DateTime? CreatedDate { get; set; }
        //public Guid? CreatedBy { get; set; }
        //public DateTime? UpdatedDate { get; set; }
        //public Guid? UpdatedBy { get; set; }
    }
    
    public class DeviceTypeDetail : DeviceType 
    { 
    //public string ProductType { get; set; }
      
    }
}
