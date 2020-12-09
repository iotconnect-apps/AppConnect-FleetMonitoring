using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// All Event(s).
    /// </summary>
    public class EventResult
    {
        /// <summary>
        /// Returns AutoSubscribed
        /// </summary>
        public bool autoSubscribed { get; set; }
        /// <summary>
        /// Returns DeliveryMethod
        /// </summary>
        public int deliveryMethod { get; set; }
        /// <summary>
        /// Returns EventId
        /// </summary>
        public int eventId { get; set; }
        /// <summary>
        /// Returns EventTopicGuid
        /// </summary>
        public string eventTopicGuid { get; set; }
        /// <summary>
        /// Returns Guid
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Returns Name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Returns PlaceHolder
        /// </summary>
        public string placeHolder { get; set; }
        /// <summary>
        /// Returns SeverityLevel
        /// </summary>
        public string severityLevel { get; set; }
        /// <summary>
        /// Returns SubscriptionCount
        /// </summary>
        public int subscriptionCount { get; set; }
    }
}
