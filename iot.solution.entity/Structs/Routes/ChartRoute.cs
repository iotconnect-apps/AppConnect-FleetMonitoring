using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class ChartRoute
    {
        public struct Name
        {
            public const string GetStatisticsByDevice = "chart.getstatisticsbydevice";
            public const string EnergyUsageByFleet = "chart.energyusagebyfleet";
            public const string OdometerReadingByFleet = "chart.odometerreadingbyfleet";
            public const string EnergyUsage = "chart.energyusage";
            public const string TripsByDriver = "chart.tripsbydriver";
            public const string DeviceUsage = "chart.deviceusage";
            public const string CompanyUsage = "chart.companyusage";
            public const string FleetTypeUsage = "chart.fleettypeusage";
            public const string FleetStatus = "chart.fleetstatus";
            public const string ExecuteCron = "chart.executecron";
        }

        public struct Route
        {
            public const string Global = "api/chart";
            public const string GetStatisticsByDevice = "getstatisticsbydevice";
            public const string EnergyUsage = "getenergyusage";
            public const string TripsByDriver = "gettripsbydriver";
            public const string EnergyUsageByFleet = "getenergyusagebyfleet";
            public const string OdometerReadingByFleet = "getodometerreadingbyfleet";
            public const string DeviceUsage = "getdeviceusage";
            public const string CompanyUsage = "getcompanyusage";
            public const string FleetTypeUsage = "getfleettypeusage";
            public const string FleetStatus = "getfleetstatus";
            public const string ExecuteCron = "executecron";
        }
    }
}
