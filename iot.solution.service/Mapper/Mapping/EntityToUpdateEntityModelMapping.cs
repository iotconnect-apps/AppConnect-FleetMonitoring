using AutoMapper;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;

namespace iot.solution.service.Mapper.Mapping
{
    public class EntityToUpdateEntityModelMapping : ITypeConverter<Entity.Entity, IOT.UpdateEntityModel>
    {
        public IOT.UpdateEntityModel Convert(Entity.Entity source, IOT.UpdateEntityModel destination, ResolutionContext context)
        {
            if (source == null)
            {
                return null;
            }

            if (destination == null)
            {
                destination = new IOT.UpdateEntityModel();
                
            }
            destination.Name = source.Name;
            destination.Description = source.Description;
            destination.Address = source.Address;
            destination.Address2 = source.Address2;
            destination.City = source.City;
            destination.StateGuid = source.StateGuid?.ToString();
            destination.CountryGuid = source.CountryGuid?.ToString();
            destination.ZipCode = source.Zipcode;
            destination.ParentEntityGuid = source.ParentEntityGuid.ToString();

            //destination.TimezoneGuid = source.Time;
            //destination.ParentEntityGuid = ;
            //destination.ChildEntityLable = ;

            return destination;
        }
    }
}
