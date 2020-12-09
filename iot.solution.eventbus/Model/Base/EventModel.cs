using component.eventbus.Common;
using Newtonsoft.Json;

namespace component.eventbus.Model.Base
{
    /// <summary>
    /// EventModel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventModel<T> where T : class
    {
        /// <summary>
        /// Gets or sets the dtype.
        /// </summary>
        /// <value>
        /// The dtype.
        /// </value>
        [JsonProperty("dtype")]
        public int Dtype { get; set; }

        /// <summary>
        /// Gets or sets the type of the enc.
        /// </summary>
        /// <value>
        /// The type of the enc.
        /// </value>
        [JsonProperty("encType")]
        public int EncType { get; set; }

        /// <summary>
        /// The original value
        /// </summary>
        private T _originalValue;

        /// <summary>
        /// Gets the original value.
        /// </summary>
        /// <value>
        /// The original value.
        /// </value>
        [JsonIgnore]
        //[JsonProperty("originalvalue")]
        public T OriginalValue
        {
            get { return _originalValue; }
        }

        /// <summary>
        /// The value
        /// </summary>
        private string _value;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [JsonProperty("value")]
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                this._originalValue = JsonConvert.DeserializeObject<T>(_value.ToBase64Decode());
            }
        }
    }
}
