using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Response
{

    public class TimeZoneData
    {
        public string id { get; set; }
        public string utcName { get; set; }
        public double offset { get; set; }
        public bool isdst { get; set; }
        public bool isDeleted { get; set; }
        public string timeZoneGuid { get; set; }
        public string name { get; set; }
    }

    public class TimezoneParameter
    {
        public int count { get; set; }
    }

    public class TimezoneResponse
    {
        public List<TimeZoneData> data { get; set; }
        public TimezoneParameter @params { get; set; }
    }
}
