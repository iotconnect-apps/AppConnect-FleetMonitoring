namespace IoTConnect.Model
{
    /// <summary>
    /// Delete User Result.
    /// </summary>
    public class DeleteUserResult
    {
        /// <summary>
        /// Returns User Guid.
        /// </summary>
        public string userguid { get; set; }
        /// <summary>
        /// Returns Event Place holders.
        /// </summary>
        public string eventPlaceHolders { get; set; }
    }
}
