// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    public partial class CreateOrganisationRelationTypeRequest
    {
        /// <summary>
        /// Initializes a new instance of the
        /// CreateOrganisationRelationTypeRequest class.
        /// </summary>
        public CreateOrganisationRelationTypeRequest() { }

        /// <summary>
        /// Initializes a new instance of the
        /// CreateOrganisationRelationTypeRequest class.
        /// </summary>
        public CreateOrganisationRelationTypeRequest(System.Guid? id = default(System.Guid?), string name = default(string), string inverseName = default(string))
        {
            Id = id;
            Name = name;
            InverseName = inverseName;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public System.Guid? Id { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "inverseName")]
        public string InverseName { get; set; }

    }
}