using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class SubscriberDetails
    {        
        public SubsciberCompanyDetails SubscriberDetail { get; set; }
        public SearchResult<List<HardwareKitResponse>> HardwareKits { get; set; }
    }
}
