using iot.solution.entity;
using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;

namespace iot.solution.model.Repository.Interface
{
    public interface IDeviceRepository : IGenericRepository<Model.Device>
    {
        Entity.DeviceDetailModel Get(Guid device);
        Entity.ActionStatus Manage(Model.DeviceModel request);
        Entity.ActionStatus Delete(Guid id);
        Entity.SearchResult<List<Entity.DeviceListItem>> List(Entity.SearchRequest request);
        List<Response.EntityWiseDeviceResponse> GetEntityWiseDevices(Guid? locationId, Guid? deviceId);
        Entity.BaseResponse<int> ValidateKit(string kitCode);
        Entity.BaseResponse<List<Entity.HardwareKit>> ProvisionKit(Entity.ProvisionKitRequest request); 
        List<LookupItem> GetDeviceLookup();      
        List<Entity.DeviceMediaFiles> GetMediaFiles(Guid deviceId, string type);
        Entity.ActionStatus UploadFiles(string xmlString, string deviceId);
        ActionStatus DeleteMediaFiles(Guid deviceId, Guid? fileId);
    }
}
