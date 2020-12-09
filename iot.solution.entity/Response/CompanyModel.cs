using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Response
{
    public class CompanyModel
    {
        public Guid Guid { get; set; }
        public Guid EntityGuid { get; set; }
        public Guid? Parentguid { get; set; }
        public Guid? AdminUserGuid { get; set; }
        public string Name { get; set; }
        public string CpId { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public Guid? CountryGuid { get; set; }
        public Guid? TimezoneGuid { get; set; }
        public string Image { get; set; }
        public bool? Isactive { get; set; }
        public bool Isdeleted { get; set; }
        public DateTime? Createddate { get; set; }
        public Guid? Createdby { get; set; }
        public DateTime? Updateddate { get; set; }
        public Guid? Updatedby { get; set; }
    }
}
