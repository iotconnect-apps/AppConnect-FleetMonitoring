using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class SubscriberItem
    {
        private string _Value;
        public string Value { get { return _Value.ToUpper(); } set { _Value = value; } }
        public string Text { get; set; }
    }
}
