using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Response
{
    public class RoleResponse
    {
        public Guid Id { get; set; }
        public Guid? CompanyId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string Description { get; set; }
        public bool IsAdminRole { get; set; }
      
    }
}
