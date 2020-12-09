namespace IoTConnect.Model
{
    /// <summary>
    /// All Role Look up Response
    /// </summary>
    public class AllRoleLookupResult//AllRoleResult
    {
        /// <summary>
        /// Returns Guid.
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// Returns Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Returns Description.
        /// </summary>
        public object Description { get; set; }
        /// <summary>
        /// Returns Status.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Returns Row Count.
        /// </summary>
        public int Row_num { get; set; }
    }
}
