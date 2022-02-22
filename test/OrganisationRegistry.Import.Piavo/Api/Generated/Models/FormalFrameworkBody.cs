// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    public partial class FormalFrameworkBody
    {
        /// <summary>
        /// Initializes a new instance of the FormalFrameworkBody class.
        /// </summary>
        public FormalFrameworkBody() { }

        /// <summary>
        /// Initializes a new instance of the FormalFrameworkBody class.
        /// </summary>
        public FormalFrameworkBody(System.Guid? bodyId = default(System.Guid?), string bodyName = default(string), string bodyShortName = default(string), string bodyNumber = default(string), System.Guid? organisationId = default(System.Guid?), string organisationName = default(string))
        {
            BodyId = bodyId;
            BodyName = bodyName;
            BodyShortName = bodyShortName;
            BodyNumber = bodyNumber;
            OrganisationId = organisationId;
            OrganisationName = organisationName;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "bodyId")]
        public System.Guid? BodyId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "bodyName")]
        public string BodyName { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "bodyShortName")]
        public string BodyShortName { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "bodyNumber")]
        public string BodyNumber { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "organisationId")]
        public System.Guid? OrganisationId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "organisationName")]
        public string OrganisationName { get; set; }

    }
}
