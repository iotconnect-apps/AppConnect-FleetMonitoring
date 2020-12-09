using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity = iot.solution.entity;

namespace iot.solution.service.Interface
{
    public interface ILookupService
    {
        List<Entity.LookupItem> Get(string type, string param);
        List<Entity.LookupItem> GetTemplate(bool isGateway);
        List<Entity.LookupItem> GetAllTemplate();
        List<Entity.TagLookup> GetTagLookup(Guid templateId);
        List<Entity.LookupItem> GetSensors(Guid deviceId);
        List<Entity.LookupItemWithDescription> GetTemplateAttribute(Guid templateId);
        List<Entity.LookupItem> GetTemplateCommands(Guid templateId);
        string GetIotTemplateGuidByName(string templateName);
        List<Entity.LookupItem> GetAllTemplateFromIoT();
        List<Entity.KitTypeAttribute> GetAllAttributesFromIoT(string templateGuid);
        List<Entity.LookupItem> GetAllCommandsFromIoT(string templateGuid);

        List<Entity.LookupItemWithStatus> FacilityLookup(Guid companyId);

        List<Entity.LookupItem> ZoneLookup(Guid entityId);
        List<Entity.LookupItemWithTemplateGuid> DeviceTypeLookup(Guid entityId);
        List<Entity.LookupItemWithStatus> DeviceLookup(Guid subEntituId);
        List<Entity.LookupItemWithStatus> GetDeviceByTemplateLookup(Guid templateGuid,Guid? deviceGuid);
        Entity.BaseResponse<List<Entity.AttributeItem>> DeviceAttributeLookup(Guid deviceId);
        List<Entity.LookupItem> GetDriverFleetLookup(Guid? fleetId);
    }
}