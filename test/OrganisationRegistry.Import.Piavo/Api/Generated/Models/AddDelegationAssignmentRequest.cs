// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    public partial class AddDelegationAssignmentRequest
    {
        /// <summary>
        /// Initializes a new instance of the AddDelegationAssignmentRequest
        /// class.
        /// </summary>
        public AddDelegationAssignmentRequest() { }

        /// <summary>
        /// Initializes a new instance of the AddDelegationAssignmentRequest
        /// class.
        /// </summary>
        public AddDelegationAssignmentRequest(System.Guid? delegationAssignmentId = default(System.Guid?), System.Guid? bodyId = default(System.Guid?), System.Guid? bodySeatId = default(System.Guid?), System.Guid? personId = default(System.Guid?), System.Collections.Generic.IDictionary<string, string> contacts = default(System.Collections.Generic.IDictionary<string, string>), System.DateTime? validFrom = default(System.DateTime?), System.DateTime? validTo = default(System.DateTime?))
        {
            DelegationAssignmentId = delegationAssignmentId;
            BodyId = bodyId;
            BodySeatId = bodySeatId;
            PersonId = personId;
            Contacts = contacts;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "delegationAssignmentId")]
        public System.Guid? DelegationAssignmentId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "bodyId")]
        public System.Guid? BodyId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "bodySeatId")]
        public System.Guid? BodySeatId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "personId")]
        public System.Guid? PersonId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "contacts")]
        public System.Collections.Generic.IDictionary<string, string> Contacts { get; set; }

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
