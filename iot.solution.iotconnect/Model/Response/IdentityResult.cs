using System.Collections.Generic;

namespace IoTConnect.Model
{
    /// <summary>
    /// IotConnect IdentityResult class.
    /// </summary>
    public class IdentityResult
    { 
        /// <summary>
        /// Http Status code.
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// Data.
        /// </summary>
        public IdentityResultData data { get; set; }

        /// <summary>
        /// IotConnect response message.
        /// </summary>
        public string message { get; set; }


    }

    public class IdentityResultData
    {
        public string companyLogo { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string timezoneGuid { get; set; }
        public string version { get; set; }
        public string uiVersion { get; set; }
        public string parentEntityGuid { get; set; }
        public string pageSize { get; set; }
        public string userGuid { get; set; }
        public string roleGuid { get; set; }
        public string companyGuid { get; set; }
        public string companyLogoUrl { get; set; }
        public string permissions { get; set; }
        public string companyName { get; set; }
        public string roleName { get; set; }
        public string cpId { get; set; }
        public string entityGuid { get; set; }
        public string solutionGuid { get; set; }
        public string isCompanyAdmin { get; set; }
    }

}
