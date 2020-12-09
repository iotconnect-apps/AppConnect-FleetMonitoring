using System;
using System.ComponentModel.DataAnnotations;

namespace iot.solution.entity.Request
{
    public class AddRoleRequest
    {
        public Guid? CompanyId { get; set; }
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public bool IsAdminRole { get; set; }
       
    }
}
