using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class AllRuleResponse
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Template { get; set; }
        public string Entity { get; set; }
        public string RuleType { get; set; }
        public string ConditionText { get; set; }
        public int UserCount { get; set; }
        public int RoleCount { get; set; }
        public string Createdby { get; set; }
        public string Updatedby { get; set; }
        public DateTime Createddate { get; set; }
        public DateTime Updateddate { get; set; }
        public bool IsActive { get; set; }
    }
}
