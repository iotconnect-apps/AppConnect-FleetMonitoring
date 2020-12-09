using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class AdminRule
    {
        public Guid Guid { get; set; }
        public Guid kittypeGuid { get; set; }
        public string KitTypeName { get; set; }
        public short RuleType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AttributeGuid { get; set; }
        public string ConditionText { get; set; }
        public string ConditionValue { get; set; }
        public Guid? SeverityLevelGuid { get; set; }
        public long? NotificationType { get; set; }
        public string CommandText { get; set; }
        public string CommandValue { get; set; }
        public string CommandName { get; set; }
        public string LocalName { get; set; }
        public string tag { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
