using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class Fleet
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public string FleetId { get; set; }
        public string RegistrationNo { get; set; }
        public string LoadingCapacity { get; set; }
        public Guid? TypeGuid { get; set; }
        public Guid? MaterialTypeGuid { get; set; }
        public string Image { get; set; }
        public string SpeedLimit { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int? Radius { get; set; }
        public int? TotalMiles { get; set; }
    }
}
