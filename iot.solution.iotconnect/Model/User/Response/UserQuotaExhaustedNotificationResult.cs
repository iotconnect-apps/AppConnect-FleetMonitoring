using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// Update User.
    /// </summary>
    public class UserQuotaExhaustedNotificationResult
    {
        /// <summary>
        /// User Plan Name.
        /// </summary>
        public string planName { get; set; }
        public string expirednotification { get; set; }
        public string exhaustedMessageNotification { get; set; }
        public string exhaustedUserNotification { get; set; }
        public DateTime expiredDate { get; set; }
        public string userQuota { get; set; }
        public string totalMessage { get; set; }
        public string userCount { get; set; }
        public string messageConsumed { get; set; }
        public string messageConsumedPercentage { get; set; }
        public bool isExistQuota { get; set; }
        public string deviceCount { get; set; }
        public string deviceQuota { get; set; }
        
    }
}
