using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public struct TripRoute
    {
        public struct Name
        {
            public const string Add = "trip.add";
            public const string GetList = "trip.list";
            public const string GetById = "trip.getdevicebyid";
            public const string Delete = "trip.deletedevice";
            public const string DeleteImage = "trip.deletedeviceimage";
            public const string BySearch = "trip.search";            
            public const string UpdateStatus = "trip.updatestatus";
            public const string StartTrip = "trip.starttrip";
            public const string UpdateTripStatus = "trip.updatetripstatus";
            public const string DeletePermissionFile = "trip.deleteshipmentfile";
        }

        public struct Route
        {
            public const string Global = "api/trip";
            public const string Manage = "manage";
            public const string GetList = "";
            public const string GetById = "{id}";
            public const string Delete = "delete";
            public const string DeleteImage = "deleteimage";
            public const string UpdateStatus = "updatestatus";
            public const string UpdateTripStatus = "updatetripstatus";
            public const string StartTrip = "starttrip";
            public const string BySearch = "search";
            public const string ShipmentFile = "deleteshipmentfile/{tripId}/{fileId?}";


        }
    }
}
