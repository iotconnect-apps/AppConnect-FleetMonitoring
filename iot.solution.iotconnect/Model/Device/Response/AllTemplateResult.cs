using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// All Template(s).
    /// </summary>
    public class AllTemplateResult
    {
        /// <summary>
        /// Template guid.
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// Template Code.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Template Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Template Description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// firmware guid.
        /// </summary>
        public object FirmwareGuid { get; set; }
        /// <summary>
        /// Template Created date.
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Template Updated date.
        /// </summary>
        public DateTime UpdatedDate { get; set; }
        /// <summary>
        /// Template Attribute count.
        /// </summary>
        public int AttributeCount { get; set; }
        /// <summary>
        /// Template Setting Count.
        /// </summary>
        public int SettingCount { get; set; }
        /// <summary>
        /// Tempalte Command Count.
        /// </summary>
        public int CommandCount { get; set; }
        /// <summary>
        /// Template Device Count.
        /// </summary>
        public int DeviceCount { get; set; }
        /// <summary>
        /// Tempate Rule count.
        /// </summary>
        public int RuleCount { get; set; }
        /// <summary>
        /// Is Validate Template?
        /// </summary>
        public bool IsValidateTemplate { get; set; }
        /// <summary>
        /// Is Valid Edge Support?
        /// </summary>
        public bool IsValidEdgeSupport { get; set; }
        /// <summary>
        /// Is Valid Type2 Support?
        /// </summary>
        public bool IsValidType2Support { get; set; }
        /// <summary>
        /// Tag.
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// Template Authentication Type(s).
        /// 1 = Key.
        /// 2 = CA Signed Certificate.
        /// 3 = Self Signed Certificate.
        /// 4 = TPM.
        /// </summary>
        public int AuthType { get; set; }
        /// <summary>
        /// Is edge Support?
        /// </summary>
        public bool IsEdgeSupport { get; set; }
    }
}
