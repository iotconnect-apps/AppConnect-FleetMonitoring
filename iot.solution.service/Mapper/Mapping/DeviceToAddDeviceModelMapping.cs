using AutoMapper;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;

namespace iot.solution.service.Mapper.Mapping
{
    class DeviceToAddDeviceModelMapping : ITypeConverter<Entity.Device, IOT.AddDeviceModel>
    {
        public IOT.AddDeviceModel Convert(Entity.Device source, IOT.AddDeviceModel destination, ResolutionContext context)
        {
            if (source == null)
            {
                return null;
            }

            if (destination == null)
            {
                destination = new IOT.AddDeviceModel();
                destination.properties = new System.Collections.Generic.List<IOT.AddProperties>();
            }

            //destination.DisplayName = source.Name;
            //destination.uniqueId = source.UniqueId;
            //destination.entityGuid = source.EntityGuid.ToString().ToUpper();
            // destination.deviceTemplateGuid = source.DeviceTemplateGuid.ToString().ToUpper();
            // destination.parentDeviceGuid = source.ParentDeviceGuid.ToString().ToUpper();
            // destination.note = source.Note;
            // destination.tag = source.Tag;
            //destination.primaryThumbprint = ;
            //destination.secondaryThumbprint = ;
            //destination.endorsementKey = ;

            destination.DisplayName = source.Name;
            destination.uniqueId = source.UniqueId;
            destination.entityGuid = source.EntityGuid.ToString().ToUpper();
            destination.deviceTemplateGuid = source.TemplateGuid.ToString().ToUpper();
            // destination.parentDeviceGuid = source.ParentGensetGuid?.ToString().ToUpper();
            //destination.note = source.Note;
            //destination.tag = source.Tag;
            //destination.primaryThumbprint = ;
            //destination.secondaryThumbprint = ;
            //destination.endorsementKey = ;
            return destination;            
        }
    }
}
