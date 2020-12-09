using iot.solution.entity.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace iot.solution.entity
{
    //Facility
    public class Fleet
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
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int? Radius { get; set; }
        public int? TotalMiles { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }

       
        public List<EntityWiseDeviceResponse> Devices { get; set; }
      
    }
    public class FleetDetail : Fleet
    {
       
        public List<DeviceMediaFiles> FleetPermissionFiles { get; set; }
        public int TotalDevices { get; set; }
        public int TotalAlerts { get; set; }
      
    }
    public class FleetUpdateModel {
        public bool status { get; set; }
        public Guid guid { get; set; }
        public System.DateTime? currentDate { get; set; }
        public string timeZone { get; set; }
    }
    public class FleetDeleteModel 
    { 
    public Guid guid { get; set; }
        public System.DateTime? currentDate { get; set; }
        public string timeZone { get; set; }
    }
        public class FleetModel : Fleet
    {
        [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
        public List<deviceInfo> devices { get; set; }
        public IFormFile ImageFile { get; set; }
        public List<IFormFile> PermissionFiles { get; set; }
        
    }
    public class FleetListRequest : ListRequest
    {
        public System.DateTime? currentDate { get; set; }
        public string timeZone { get; set; }
    }
    public class FleetListItem
    {
        public Guid Guid { get; set; }
        public string FleetId { get; set; }
        public string RegistrationNo { get; set; }
        public string LoadingCapacity { get; set; }
        public string FleetTypeName { get; set; }
        public string MaterialTypeName { get; set; }
        public string TemplateGuid { get; set; }
        public string TemplateName { get; set; }
        public string Status { get; set; }
        public string Image { get; set; }
        public string Latitude { get; set; }
        public string longitude { get; set; }
        public int Radius { get; set; }
        public int TotalMiles { get; set; }
        public bool? IsActive { get; set; }
        public bool IsStarted { get; set; }
        public int TotalDevices { get; set; }
      
    }
    public class FleetMapListItem
    {
        public Guid Guid { get; set; }
        public string UniqueId { get; set; }
        public string FleetId { get; set; }
        public Guid? TripGuid { get; set; }
        public string TripId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string SourceLatitude { get; set; }
        public string SourceLongitude { get; set; }
        public string DestinationLatitude { get; set; }
        public string DestinationLongitude { get; set; }
        public int Radius { get; set; }
        public int TotalMiles { get; set; }
        public string FleetTypeName { get; set; }
        public string MaterialTypeName { get; set; }
        public Guid? DriverGuid { get; set; }
        public string DriverId { get; set; }
        public bool IsStarted { get; set; }
        public string Status { get; set; }
    }

}
