using iot.solution.entity.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace iot.solution.entity
{
    public class Trip
    {
        public Guid Guid { get; set; }
        public string TripId { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid FleetGuid { get; set; }
        public string SourceLocation { get; set; }
        public string DestinationLocation { get; set; }
        public Guid? MaterialTypeGuid { get; set; }
        public string Weight { get; set; }
        public DateTime? StartDateTime { get; set; }
        public int? TotalMiles { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string SourceLatitude { get; set; }
        public string SourceLongitude { get; set; }
        public string DestinationLatitude { get; set; }
        public string DestinationLongitude { get; set; }
        public bool IsStarted { get; set; }
        public DateTime? ActualStartDateTime { get; set; }
        public DateTime? EtaEndDateTime { get; set; }
        public int AggressiveAcceleration { get; set; }
        public int OverSpeed { get; set; }
        public int HarshBraking { get; set; }
        public int IdleTime { get; set; }
        public int? CoveredMiles { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class TripStops
    {
        public Guid Guid { get; set; }
        public Guid TripGuid { get; set; }
        public string StopName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTime? EndDateTime { get; set; }
    }

    public class stop
    {
        public Guid? guid { get; set; }
        public string stopName { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public DateTime? endDateTime { get; set; }
    }
    public class TripDetail :Trip
    {
        public string FleetName { get; set; }
        public string MaterialType { get; set; }
        public List<TripStops> TripStops { get; set; }
        public List<DeviceMediaFiles> ShipmentFiles { get; set; }
    }
    public class TripListRequest :ListRequest
    {
        public string status { get; set; }
        public Guid? driverGuid { get; set; }
        public Guid? fleetGuid { get; set; }
        public System.DateTime? currentDate { get; set; }
        public System.DateTime? startDate { get; set; }
        public System.DateTime? endDate { get; set; }
        public string timeZone { get; set; }
      
    }
    public class TripListItem
    {
        public Guid Guid { get; set; }
        public string TripId { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid FleetGuid { get; set; }
        public Guid DriverGuid { get; set; }
        public string SourceLocation { get; set; }
        public string DestinationLocation { get; set; }
        public string SourceLatitude { get; set; }
        public string SourceLongitude  { get; set; }
        public string DestinationLatitude { get; set; }
        public string DestinationLongitude { get; set; }
        public string FleetName { get; set; }
        public string MaterialType { get; set; }
        public string Weight { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string  Status { get; set; }
        public int? TotalMiles { get; set; }
        public string DriverName { get; set; }
        public string UniqueId { get; set; }
        public int? CoveredMiles { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsStarted { get; set; }

    }
   
    public class TripModel : Trip
    {
        [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
        public List<stop> tripStop { get; set; }
        public List<IFormFile> ShipmentFiles { get; set; }
        public DateTime? CurrentDate { get; set; } = null;
        public string TimeZone { get; set; } = "";

    }

    public class TripUpdateStatus
    {
        [Required]
        public string tripGuid { get; set; }
        [Required]
        public DateTime? currentDate { get; set; }
        [Required]
        public string timeZone { get; set; }

        public int? coveredMiles { get; set; }
    }
    public class StartTripModal
    {
        [Required]
        public string tripGuid { get; set; }
        [Required]
        public DateTime? etaEndDateTime { get; set; }
        [Required]
        public DateTime? currentDate { get; set; }
        [Required]
        public string timeZone { get; set; }
        [Required]
        public long? odometer { get; set; }
        public Guid? fleetGuid { get; set; }

    }

}
