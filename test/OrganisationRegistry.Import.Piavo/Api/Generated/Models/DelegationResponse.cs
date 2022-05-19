// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    public partial class DelegationResponse
    {
        /// <summary>
        /// Initializes a new instance of the DelegationResponse class.
        /// </summary>
        public DelegationResponse() { }

        /// <summary>
        /// Initializes a new instance of the DelegationResponse class.
        /// </summary>
        public DelegationResponse(System.Guid? id = default(System.Guid?), System.Guid? bodyId = default(System.Guid?), string bodyName = default(string), string bodyOrganisationName = default(string), string organisationName = default(string), string functionTypeName = default(string), System.Guid? bodySeatId = default(System.Guid?), string bodySeatNumber = default(string), string bodySeatName = default(string), bool? isDelegated = default(bool?), System.DateTime? validFrom = default(System.DateTime?), System.DateTime? validTo = default(System.DateTime?))
        {
            Id = id;
            BodyId = bodyId;
            BodyName = bodyName;
            BodyOrganisationName = bodyOrganisationName;
            OrganisationName = organisationName;
            FunctionTypeName = functionTypeName;
            BodySeatId = bodySeatId;
            BodySeatNumber = bodySeatNumber;
            BodySeatName = bodySeatName;
            IsDelegated = isDelegated;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public System.Guid? Id { get; set; }

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
        [Newtonsoft.Json.JsonProperty(PropertyName = "bodyOrganisationName")]
        public string BodyOrganisationName { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "organisationName")]
        public string OrganisationName { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "functionTypeName")]
        public string FunctionTypeName { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "bodySeatId")]
        public System.Guid? BodySeatId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "bodySeatNumber")]
        public string BodySeatNumber { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "bodySeatName")]
        public string BodySeatName { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "isDelegated")]
        public bool? IsDelegated { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "validFrom")]
        public System.DateTime? ValidFrom { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "validTo")]
        public System.DateTime? ValidTo { get; set; }

    }
}