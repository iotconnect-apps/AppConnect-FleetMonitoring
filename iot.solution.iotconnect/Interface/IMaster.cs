using IoTConnect.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTConnect.Common.Interface
{
    internal interface IMaster
    {
        /// <summary>
        /// Get IotConnect Country list.
        /// </summary>
        /// <returns></returns>
        Task<DataResponse<List<Conuntry>>> Countries();

        /// <summary>
        /// Get IotConnect TimeZone list.
        /// </summary>
        /// <returns></returns>
        Task<DataResponse<List<AllTimeZoneResult>>> TimeZones();


        /// <summary>
        /// Get IotConnect State list by Country.
        /// </summary>
        /// <returns></returns>
        Task<DataResponse<List<StateResult>>> States(string CountryGuid);
    }
}
