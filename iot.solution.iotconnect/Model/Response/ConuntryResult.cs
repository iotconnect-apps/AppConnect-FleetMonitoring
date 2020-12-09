using System;
using System.Collections.Generic;
using System.Text;

namespace IoTConnect.Model
{
    /// <summary>
    /// Country Class.
    /// </summary>
    public class Conuntry
    {
        /// <summary>
        /// Country Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Country Name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Country Code.
        /// </summary>
        public string countryCode { get; set; }
    }

    /// <summary>
    /// Get All Country Result. 
    /// </summary>
    public class CountryResult
    {
        /// <summary>
        /// List of Country Data.
        /// </summary>
        public List<Conuntry> data { get; set; }
        /// <summary>
        /// Returns Status. API execute or not.
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// Return Result Messages.
        /// </summary>
        public string message { get; set; }
    }
}
