using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Add Template.
    /// </summary>
    public class AddTemplateModel
    {
        /// <summary>
        /// Template Name.
        /// </summary>
        [Required(ErrorMessage = "Template name is required")]
        public string Name { get; set; }
        /// <summary>
        /// (Optional) Template Description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// (Optional) Firmware guid.
        /// </summary>
        public string Firmwareguid { get; set; }
        /// <summary>
        /// Template Code.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// (Optional) Set true if device supports Edge.
        /// </summary>
        public bool IsEdgeSupport { get; set; }
        /// <summary>
        /// Authentication Type(s).
        /// 1 = Key.
        /// 2 = CA Signed Certificate.
        /// 3 = Self Signed Certificate.
        /// 4 = TPM.
        /// </summary>
        [Required(ErrorMessage = "AuthType should be 1/2/3/4")]
        public int AuthType { get; set; }
        /// <summary>
        /// (Optional) Template Tag. Call Template.Taglookup() method to get list of tag.
        /// </summary>
        public string Tag { get; set; }

        internal CustomETPlaceHolders CustomETPlaceHolders { get; set; }
    }

    internal class CustomETPlaceHolders
    {
    }
}
