using Newtonsoft.Json;
using System;

namespace component.messaging.Model
{
    public class MessageModel
    {
        [JsonProperty("msgType")]
        public string MsgType { get; set; }

        [JsonProperty("msgId")]
        public int MsgId { get; set; }

        [JsonProperty("msgTypeCode")]
        public int MsgTypeCode { get; set; }

        [JsonProperty("company")]
        public Guid Company { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("invokinguser")]
        public Guid Invokinguser { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }

    public class DeviceConnectMessageModel : MessageModel
    {
        [JsonProperty("msgCount")]
        public int MsgCount { get; set; }
        [JsonProperty("isConnected")]
        public bool IsConnected { get; set; }
    }
}
