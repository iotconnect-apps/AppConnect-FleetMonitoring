namespace IoTConnect.Model
{
    /// <summary>
    /// Returns Forgot Passwords.
    /// </summary>
    public class ForgotPasswordResult
    {
        /// <summary>
        /// Return User Guid.
        /// </summary>
        public string UserGuid { get; set; }
        /// <summary>
        /// Return Invitation Guid.
        /// </summary>
        public string InvitationGuid { get; set; }
        /// <summary>
        /// Return First Name.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Return Last Name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Return Company Guid.
        /// </summary>
        public string CompanyGuid { get; set; }
        /// <summary>
        /// Return Event Place holders.
        /// </summary>
        public string EventPlaceHolders { get; set; }
        /// <summary>
        /// Return Invoking User.
        /// </summary>
        public string InvokingUser { get; set; }
    }
}
