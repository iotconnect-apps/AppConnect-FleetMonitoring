using IoTConnect.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTConnect.DeviceProvider.Interface
{
    internal interface IDevice
    {
        Task<DataResponse<List<SingleResult>>> AllUniqueDevice(string uniqueId, string parentDeviceUniqueId = "");
        Task<BaseResponse<List<AllDeviceResult>>> AllDevice(AllDeviceModel deviceModel);
        Task<DataResponse<AddDeviceResult>> Add(AddDeviceModel request);
        Task<DataResponse<UpdateDeviceResult>> Update(string deviceGuid, UpdateDeviceModel updateDevice);
        Task<DataResponse<DeleteDeviceResult>> Delete(string deviceGuid);
        Task<DataResponse<AcquireDeviceResult>> AcquireDevice(string uniqeid, AcquireDeviceModel acquireDevice);
        Task<DataResponse<ReleaseDeviceResult>> ReleaseDevice(string UniqueId);
        Task<DataResponse<UpdateDeviceResult>> UpdateDeviceStatus(string deviceGuid, UpdateDeviceStatusModel updateDeviceStatusModel);
        Task<DataResponse<UpdateDeviceResult>> UpdateDeviceEntityBulk(string entityGuid, List<UpdateDeviceEntityBulkModel> updatedeviceEntityModel);
        Task<DataResponse<List<AllChildDeviceResult>>> AllChildDevice(string parentDeviceGuid, PagingModel pagingModel);
        Task<DataResponse<List<DeviceTwinResult>>> DeviceTwinByUniqueId(string uniqueid);
        Task<DataResponse<List<DeviceGrantResult>>> DeviceGrant(string deviceGuid, PagingModel pagingModel);
        Task<DataResponse<UpdateDeviceResult>> UpdateDeviceTwin(string uniqueid, UpdateDeviceTwinModel updatedeviceEntityModel);
        Task<DataResponse<AddDeviceGrantResult>> AddGrant(string deviceGuid, AddGrantModel request);
        Task<DataResponse<DeleteDevicePermissionResult>> DeleteGrant(string UserDevicePermissionGuid);
        Task<DataResponse<AllottedDeviceToUserResult>> AllottedDeviceToUser(string userId, AllottedDeviceUserModel request);
        Task<DataResponse<List<AllotedDeviceResult>>> GetAllAllottedDevice(PagingModel pagingModel);
        Task<DataResponse<AllotedDeviceResult>> GetSingleAllottedDevice(string userGuid, PagingModel pagingModel);
    }
}
