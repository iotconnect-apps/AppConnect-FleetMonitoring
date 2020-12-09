using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Response
{
    public class SubscriptionEndData
    {
        public Guid Guid { get; set; }
        public string Email { get; set; }
        public string CustomerName { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
