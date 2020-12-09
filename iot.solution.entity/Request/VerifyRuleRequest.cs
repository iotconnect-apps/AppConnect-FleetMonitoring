using System;
using System.ComponentModel.DataAnnotations;

namespace iot.solution.entity
{
    public class VerifyRuleRequest
    {
        public string deviceTemplateGuid { get; set; }
        public string expression { get; set; }

    }
}
