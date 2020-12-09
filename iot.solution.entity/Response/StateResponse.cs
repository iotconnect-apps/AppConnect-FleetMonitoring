using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Response
{
    public class StateData
    {
        public Guid stateId { get; set; }
        public string stateName { get; set; }
        public string countryId { get; set; }
        public string stateCode { get; set; }
    }
    public class StateParameter
    {
        public int count { get; set; }
    }

    public class StateResponse
    {
        public List<StateData> data { get; set; }
        public StateParameter @params { get; set; }
    }
}
