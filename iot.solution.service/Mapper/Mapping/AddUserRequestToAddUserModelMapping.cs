using AutoMapper;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;

namespace iot.solution.service.Mapper.Mapping
{
    public class AddUserRequestToAddUserModelMapping : ITypeConverter<Entity.AddUserRequest, IOT.AddUserModel>
    {
        public IOT.AddUserModel Convert(Entity.AddUserRequest source, IOT.AddUserModel destination, ResolutionContext context)
        {
            if (source == null)
            {
                return null;
            }

            if (destination == null)
            {
                destination = new IOT.AddUserModel();
            }

            destination.FirstName = source.FirstName;
            destination.LastName = source.LastName;
            destination.UserId = source.Email.ToLower();
            destination.TimezoneGuid = source.TimeZoneGuid.ToString();
            destination.RoleGuid = source.RoleGuid.ToString();
            destination.ContactNo = source.ContactNo;
            destination.IsActive = source.IsActive ? 1 : 0;
            destination.EntityGuid = source.EntityGuid.ToString();
            return destination;
        }
    }
}
