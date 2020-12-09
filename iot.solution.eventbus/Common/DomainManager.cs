using component.eventbus.Common.Enum;
using System;
using System.Collections.Generic;

namespace component.eventbus.Common
{
    /// <summary>
    /// DomainManager
    /// </summary>
    public class DomainManager
    {
        /// <summary>
        /// The service type
        /// </summary>
        public string ServiceType;

        /// <summary>
        /// The domain configuration
        /// </summary>
        public List<Type> DomainConfiguration;

        /// <summary>
        /// The logging
        /// </summary>
        public bool Logging = true;

        /// <summary>
        /// The application code
        /// </summary>
        public string ApplicationType;
    }
}
