using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class AdminRule
    {
        public Guid Guid { get; set; }
        public Guid TemplateGuid { get; set; }
        public string AttributeGuid { get; set; }
        public short RuleType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ConditionText { get; set; }
        public string ConditionValue { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid? SeverityLevelGuid { get; set; }
        public long? NotificationType { get; set; }
        public string CommandText { get; set; }
        public string CommandValue { get; set; }
    }
}
