using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace component.services.logger.viewer.Models
{
    public class BaseResponse<T>
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public ResponseMessage Message { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public Dictionary<string, object> Params { get; set; }
    }
}
