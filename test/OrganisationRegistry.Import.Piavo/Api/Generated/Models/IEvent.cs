// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    public partial class IEvent
    {
        /// <summary>
        /// Initializes a new instance of the IEvent class.
        /// </summary>
        public IEvent() { }

        /// <summary>
        /// Initializes a new instance of the IEvent class.
        /// </summary>
        public IEvent(int? version = default(int?), System.DateTime? timestamp = default(System.DateTime?), System.Guid? id = default(System.Guid?))
        {
            Version = version;
            Timestamp = timestamp;
            Id = id;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "version")]
        public int? Version { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "timestamp")]
        public System.DateTime? Timestamp { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public System.Guid? Id { get; set; }

    }
}
