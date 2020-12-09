using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace component.services.logger.viewer.Common
{
    public enum HoursBeforeEnum
    {
        OneHours = 1,
        TowHours = 2,
        SixHours = 6,
        OneDay = 24,
        TwoDay = 48
    }
    
    public enum ConnectionStringName
    {
        Local = 1,
        Dev = 2,
        QA = 3,
        //Stage = 4,
        //Stage = 5
        //Production = 6
    }
}
