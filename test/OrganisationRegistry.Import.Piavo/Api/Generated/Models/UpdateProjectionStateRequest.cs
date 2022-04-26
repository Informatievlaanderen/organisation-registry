// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    public partial class UpdateProjectionStateRequest
    {
        /// <summary>
        /// Initializes a new instance of the UpdateProjectionStateRequest
        /// class.
        /// </summary>
        public UpdateProjectionStateRequest() { }

        /// <summary>
        /// Initializes a new instance of the UpdateProjectionStateRequest
        /// class.
        /// </summary>
        public UpdateProjectionStateRequest(string name = default(string), int? eventNumber = default(int?))
        {
            Name = name;
            EventNumber = eventNumber;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "eventNumber")]
        public int? EventNumber { get; set; }

    }
}