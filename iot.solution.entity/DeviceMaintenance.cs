using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace iot.solution.entity
{
    public class DeviceMaintenance
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid? ParentEntityGuid { get; set; }
        //This is zone Guid
        public Guid? EntityGuid { get; set; }
        public Guid DeviceGuid { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string TimeZone { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
    public class CompleteMaintenanceRequest
    {
        [Required]
        public string guid { get; set; }
        [Required]
        public DateTime? currentDate { get; set; }
        [Required]
        public string timeZone { get; set; }      
    }
    public class DeviceMaintenanceListRequest : ListRequest
    {
        public string deviceId { get; set; } = "";
        public DateTime? currentDate { get; set; } = null;
        public string timeZone { get; set; } = "";
    }
    public class DeviceMaintenanceDetail : DeviceMaintenance
    {
        public string Miles { get; set; }
        public string DeviceName { get; set; }
        public string EntityName { get; set; }
        public string SubEntityName { get; set; }
    }

    public class DeviceMaintenanceResponse: DeviceMaintenance
    {
        public string DeviceName { get; set; }
        public string EntityName { get; set; }
        public string SubEntityName { get; set; }
    }

    public class DeviceMaintenanceRequest
    {
   
        public Guid? EntityGuid { get; set; }
        public Guid? DeviceGuid { get; set; }
        public DateTime? currentDate { get; set; }
        public string timeZone { get; set; }
    }

    public class DeviceSceduledMaintenanceResponse
    {
        public string UniqueId { get; set; }
        public string Day { get; set; }
        public string Hour { get; set; }
        public string Minute { get; set; }
        public DateTime startDateTime { get; set; }
       
    }
}
