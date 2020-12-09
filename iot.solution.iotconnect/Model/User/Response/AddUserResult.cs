namespace IoTConnect.Model
{
    /// <summary>
    /// Returns Add User Result
    /// </summary>
    public class AddUserResult
    {
        /// <summary>
        /// User New Guid.
        /// </summary>
        public string newId { get; set; }
        /// <summary>
        /// Returns Invitation guid
        /// </summary>
        public string invitationGuid { get; set; }
    }
}
