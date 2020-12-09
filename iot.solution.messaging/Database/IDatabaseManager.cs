using component.messaging.Model;

namespace component.messaging.Database
{
    public interface IDatabaseManager
    {
        void CompanyProcessMessage(MessageModel subscribeData);

        void UserProcessMessage(MessageModel subscribeData);

        void EntityProcessMessage(MessageModel subscribeData);

        void TemplateProcessMessage(MessageModel subscribeData);

        void RoleProcessMessage(MessageModel subscribeData);

        void DeviceProcessMessage(MessageModel subscribeData);
    }
}
