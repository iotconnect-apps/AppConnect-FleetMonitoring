using AutoMapper;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;

namespace iot.solution.service.Mapper.Mapping
{
    public class AddUserRequestToUpdateUserModelMapping : ITypeConverter<Entity.AddUserRequest, IOT.UpdateUserModel>
    {
        public IOT.UpdateUserModel Convert(Entity.AddUserRequest source, IOT.UpdateUserModel destination, ResolutionContext context)
        {
            if (source == null)
            {
                return null;
            }

            if (destination == null)
            {
                destination = new IOT.UpdateUserModel();
            }

            destination.FirstName = source.FirstName;
            destination.LastName = source.LastName;
            destination.UserId = source.Email.ToLower();
            destination.TimezoneGuid = source.TimeZoneGuid.ToString();
            destination.RoleGuid = source.RoleGuid.ToString();
            destination.ContactNo = source.ContactNo;
            destination.EntityGuid = source.EntityGuid.ToString();
            return destination;
        }
    }
}
