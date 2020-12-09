using component.helper.Interface;
using IoTConnect.DeviceProvider;
using IoTConnect.UserProvider;
using IoTConnect.EntityProvider;
using IoTConnect.Common;
using IoTConnect.TemplateProvider;
using IoTConnect.RuleProvider;
using iot.solution.iotconnect.Repository;

namespace component.helper
{
    //public class IotConnectProvider : IIotConnectProvider
    //{
    //    public IotConnectProvider(string token, string environmentCode, string solutionKey)
    //    {
    //        Device = new Device(token, environmentCode, solutionKey);
    //        User = new User(token, environmentCode, solutionKey);
    //        GreenHouse = new Entity(token, environmentCode, solutionKey);
    //    }
    //    public Entity GreenHouse { get; }
    //    public Device Device { get; }
    //    public User User { get; }
    //}

    public abstract class BaseIotConnectClient
    {
        public BaseIotConnectClient(string token, string environmentCode, string solutionKey)
        {
            Device = new Device(token, environmentCode, solutionKey);
            User = new User(token, environmentCode, solutionKey);
            Role = new Role(token, environmentCode, solutionKey);
            Entity = new Entity(token, environmentCode, solutionKey);
            Master = new Master(token, environmentCode, solutionKey);
            Template = new Template(token, environmentCode, solutionKey);
            Login = new Auth(environmentCode, solutionKey);
            Rule = new Rule(token, environmentCode, solutionKey);

        }

        public Entity Entity { get; }
        public Auth Login { get; }
        public Device Device { get; }
        public User User { get; }
        public Role Role { get; }
        public Template Template { get; }
        public Master Master { get; }
        public Rule Rule { get; }
    }

    public class IotConnectClient : BaseIotConnectClient
    {
        public IotConnectClient(string token, string environmentCode, string solutionKey) : base(token, environmentCode, solutionKey)
        {

        }

    }
}
