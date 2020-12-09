using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class ListRequest
    {
        public string searchText { get; set; } = "";
        public int? pageNo { get; set; } = 1;
        public int? pageSize { get; set; } = 10;
        public string orderBy { get; set; } = "";
       
    }
}
