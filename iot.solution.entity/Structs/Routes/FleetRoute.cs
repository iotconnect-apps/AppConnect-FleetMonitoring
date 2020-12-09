using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public struct FleetRoute
    {
        public struct Name
        {
            public const string Add = "fleet.add";
            public const string GetList = "fleet.list";
            public const string GetById = "fleet.getdevicebyid";
            public const string Delete = "fleet.deletedevice";
            public const string DeleteImage = "fleet.deletedeviceimage";
            public const string BySearch = "fleet.search";
            public const string ByMapSearch = "fleet.mapsearch";
            public const string UpdateStatus = "fleet.updatestatus";
            public const string DeletePermissionFile = "fleet.deletepermissionfile";
        }

        public struct Route
        {
            public const string Global = "api/fleet";
            public const string Manage = "manage";
            public const string GetList = "";
            public const string GetById = "{id}";
            public const string Delete = "delete";
            public const string DeleteImage = "deleteimage";
            public const string UpdateStatus = "updatestatus";
            public const string BySearch = "search";
            public const string ByMapSearch = "maplist";
            public const string DeletePermissionFile = "deletepermissionfile/{fleetId}/{fileId?}";


        }
    }
}
