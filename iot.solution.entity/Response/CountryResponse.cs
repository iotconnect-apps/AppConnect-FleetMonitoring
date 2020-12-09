using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Response
{
    public class CountryData
    {
        public string countryId { get; set; }
        public string countryName { get; set; }
        public string countryCode { get; set; }
    }

    public class CountryParameter
    {
        public int count { get; set; }
    }

    public class CountryResponse
    {
        public List<CountryData> data { get; set; }
        public CountryParameter @params { get; set; }
    }
}
