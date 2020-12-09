using System;

namespace iot.solution.entity
{
   
    public class DashboardOverviewByEntityResponse
    {
        public Guid Guid { get; set; }
       
        public string Name { get; set; }
        
        public int TotalSubEntities { get; set; }
        public int TotalDevices { get; set; }
        
       
        public int TotalAlerts { get; set; }
    }
    public class DashboardOverviewResponse
    {
        public int TotalUserCount { get; set; }
        public int ActiveUserCount { get; set; }
        public int InActiveUserCount { get; set; }
        public int InTransitFleetCount { get; set; }
        public int InGarageFleetCount { get; set; }
        public int TotalFleetCount { get; set; }
        public int TotalSubEntities { get; set; }
        public int TotalDevices { get; set; }
        public int DriverUtilizationPer { get; set; }
        public int FleetUtilizationPer { get; set; }
        public string TotalFuelConsumption { get; set; }

        public int TotalAlerts { get; set; }
        public int TotalRunningCount {get;set;}
    }
    public class DriverDashboardOverviewResponse
    {
        public string DriverId { get; set; }
        public Guid FleetGuid { get; set; }
        public Guid? TripGuid { get; set; }
        public string DriverStatus { get; set; }
       
        public int TotalTripCount { get; set; }
        public int TotalUnderTripCount { get; set; }
        public int TotalScheduledTripCount { get; set; }
        public int TotalCompletedTripCount { get; set; }

      public int HarshBreakingCount { get; set; }
        public int OverSpeedCount { get; set; }
        public string IdleTimeHours { get; set; }
        public int TotalAlerts { get; set; }
       
    }
    public class FleetDashboardOverviewResponse
    {
        public string FleetId { get; set; }
        public Guid DeviceGuid { get; set; }
        public string AverageRotationPerMin { get; set; }
        public string HighestSpeed { get; set; }
        public string UniqueId { get; set; }
        public Guid? TripGuid { get; set; }
        public string FleetStatus { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string SourceLatitude { get; set; }
        public string SourceLongitude { get; set; }
        public string DestinationLatitude { get; set; }
        public string DestinationLongitude { get; set; }
        public int? Radius { get; set; }
        public int TotalTripCount { get; set; }
        public int TotalUnderTripCount { get; set; }
        public int TotalScheduledTripCount { get; set; }
        public int TotalCompletedTripCount { get; set; }
      
        public int TotalDevices { get; set; }
        public int TotalMaintenanceCount { get; set; }
        public int TotalScheduledCount { get; set; }
        public int TotalUnderMaintenanceCount { get; set; }
        public int TotalCompletedMaintenanceCount { get; set; }
        public int TotalAlerts { get; set; }
        public string TotalFuelConsumption { get; set; }
        public DateTime? NextMaintenanceDateTime { get; set; }
    }

    public class TripDashboardOverviewResponse
    {
        public Guid? Guid { get; set; }
        public string TripId { get; set; }
        public string SourceLocation { get; set; }
        public string SourceLatitude { get; set; }
        public string SourceLongitude { get; set; }
        public string DestinationLocation { get; set; }
        public string DestinationLatitude { get; set; }
        public string DestinationLongitude { get; set; }
        public string Weight { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int? TotalMiles { get; set; }
        public string DriverName { get; set; }
        public string Email { get; set; }
        public string DriverId { get; set; }
        public string ContactNo { get; set; }
        public string DriverImage { get; set; }
        public string FleetId { get; set; }
        public string MaterialType { get; set; }
        public string TripStatus { get; set; }
        public int HarshBraking { get; set; }
        public int AggressiveAcceleration { get; set; }
        public int OverSpeed { get; set; }
        public int IdleTime { get; set; }
        public Guid? DeviceGuid { get; set; }
        public string UniqueId { get; set; }
        public string FuelLevel { get; set; }
        public string EngineTemp { get; set; }
        public string Oil { get; set; }
        public string TyrePressure { get; set; }
        public string CurrentSpeed { get; set; }
        public string SpeedLimit { get; set; }
        public Guid? DriverGuid { get; set; }
        public Guid? FleetGuid { get; set; }
        public string FleetType { get; set; }

    }
}
