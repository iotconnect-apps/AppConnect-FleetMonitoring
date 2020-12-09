using System;
using System.Collections.Generic;
using System.Text;
using IoTConnect.DeviceProvider;
using IoTConnect.EntityProvider;
using IoTConnect.UserProvider;

namespace component.helper.Interface
{
    public interface IIotConnectProvider
    {
        public Entity GreenHouse { get; }
        public Device Device { get; }
        public User User { get; }
    }
}
