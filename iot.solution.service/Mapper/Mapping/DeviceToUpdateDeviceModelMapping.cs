using AutoMapper;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;

namespace iot.solution.service.Mapper.Mapping
{
    public class DeviceToUpdateDeviceModelMapping : ITypeConverter<Entity.Device, IOT.UpdateDeviceModel>
    {
        public IOT.UpdateDeviceModel Convert(Entity.Device source, IOT.UpdateDeviceModel destination, ResolutionContext context)
        {
            if (source == null)
            {
                return null;
            }

            if (destination == null)
            {
                destination = new IOT.UpdateDeviceModel();
            }

            destination.displayName = source.Name;
            destination.entityGuid = source.EntityGuid.ToString();
            destination.deviceTemplateGuid = source.TemplateGuid.ToString().ToUpper();
            //destination.parentDeviceGuid = source.ParentDeviceGuid.ToString().ToUpper();
            //destination.note = source.Note;
            //destination.tag = source.Tag;
            destination.properties = new List<IOT.UpdateProperties>();
            //destination.primaryThumbprint = ;
            //destination.secondaryThumbprint = ;
            //destination.endorsementKey = ;
            return destination;
        }
    }
}
