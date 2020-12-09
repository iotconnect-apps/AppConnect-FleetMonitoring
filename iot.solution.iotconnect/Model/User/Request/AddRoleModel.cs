using System.Collections.Generic;

namespace IoTConnect.Model
{
    public class AddRoleModel
    {
        /// <summary>
        /// Role Name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Role Description.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Solution Guids. Supply Solutions guid(s) to which the newly created role will have access.
        /// </summary>
        public List<string> solutions { get; set; }
    }
}
