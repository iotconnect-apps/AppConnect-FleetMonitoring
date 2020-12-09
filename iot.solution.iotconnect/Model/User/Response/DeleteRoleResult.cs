using System;
using System.Collections.Generic;
using System.Text;

namespace IoTConnect.Model
{
    /// <summary>
    /// Delete Role Result.
    /// </summary>
    public class DeleteRoleResult
    {
        /// <summary>
        /// List of Error.
        /// </summary>
        public List<Error> error { get; set; }
    }

    /// <summary>
    /// Error Message Class.
    /// </summary>
    public class Error
    {
        public string param { get; set; }
        public string message { get; set; }
    }
}
