using System.Collections.Generic;
using System.Threading.Tasks;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;

namespace component.helper.Interface
{
    public interface IEmailHelper
    {
        Task SendFleetDelayNotificationEmail(string fleetName, string OwnerName, string fleetInfo, string ownerEmail, string driverEmail);
        Task SendFleetRadiusNotificationEmail(string fleetName, string OwnerName, string radiusInfo, string ownerEmail,string driverEmail);
        Task SendCompanyRegistrationEmail(string userName, string companyName, string userId, string password);
        Task SendSubscriptionOverEmail(string userName, string expiryDate,string userEmail);
    }
}
