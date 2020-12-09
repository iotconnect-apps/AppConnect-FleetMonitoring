using System.ComponentModel;

namespace component.logger.platform.common.Enums
{
    /// <summary>
    /// ResponseStatus
    /// </summary>
    public enum ResponseStatus
    {
        /// <summary>
        /// The success
        /// </summary>
        [Description("Request Success.")]
        Success = 1,

        /// <summary>
        /// The business empty data
        /// </summary>
        [Description("{0} can not be empty.")]
        Business_EmptyData = 51,

        /// <summary>
        /// The business invalid data
        /// </summary>
        [Description("Invalid {0}.")]
        Business_InvalidData = 52,

        /// <summary>
        /// The business null request
        /// </summary>
        [Description("Request can not be null.")]
        Business_NullRequest = 54,
    }
}
