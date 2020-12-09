using System;
using System.Collections.Generic;

namespace iot.solution.entity.Response
{
    public class DeviceUsageResponse
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class EnergyUsageResponse
    {
        public string Name { get; set; }    
        public string EnergyConsumption { get; set; }
        public string MileageAverage { get; set; }
    }
    public class OdometerResponse
    {
        public string Name { get; set; }
        public string Odometer { get; set; }
        
    }
    public class TripsByDriverResponse {
        public string Name { get; set; }
        public int TotalTrips { get; set; }
    }
    public class FleetStatusResponse
    {
        public Guid Guid { get; set; }
        public string FleetId { get; set; }

        public string ActiveCount { get; set; }
        public string HaltCount { get; set; }
        public string IdleCount { get; set; }
    }
    public class CompanyUsageResponse
    {
        
        public string UtilizationPer { get; set; }
    }
    public class FleetTypeUsageResponse
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string UtilizationPer { get; set; }
    }
    public class DeviceStatisticsResponse
    {
        public string Name { get; set; }
        public string Value { get; set; }
        //public string Attribute { get; set; }
        //public string DeviceName { get; set; }
    }
    public class InventoryConsumptionResponse
    {
        public string Name { get; set; }
        public string Consumption { get; set; }
    }
    public class InventoryStatusResponse
    {
        public string DeviceName { get; set; }
        public string UniqueID { get; set; }
        public string TotalAvailableQty { get; set; }
        public   string Color { get; set; }
    }
    public class InventoryConsumptionByProductResponse
    {
        public string ProductName { get; set; }
        public string TotalCapacity { get; set; }
        public string TotalAvailableQty { get; set; }
        public string ProductImage{get;set;}
        public string Color { get; set; }
    }

    public class ConfgurationResponse
    {
        public string cpId { get; set; }
        public string host { get; set; }
        public int isSecure { get; set; }
        public string password { get; set; }
        public int port { get; set; }
        public string url { get; set; }
        public string user { get; set; }
        public string vhost { get; set; }
    }
}
