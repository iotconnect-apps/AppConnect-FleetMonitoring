using AutoMapper;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;

namespace iot.solution.service.Mapper.Mapping
{
    public class ChangePasswordRequestToChangePasswordModel : ITypeConverter<Entity.ChangePasswordRequest, IOT.ChangePasswordModel>
    {
        public IOT.ChangePasswordModel Convert(Entity.ChangePasswordRequest source, IOT.ChangePasswordModel destination, ResolutionContext context)
        {
            if (source == null)
            {
                return null;
            }

            if (destination == null)
            {
                destination = new IOT.ChangePasswordModel();
            }
            destination.Email = source.Email;
            destination.OldPassword = source.OldPassword;
            destination.NewPassword = source.NewPassword;
            return destination;
        }
    }
}
