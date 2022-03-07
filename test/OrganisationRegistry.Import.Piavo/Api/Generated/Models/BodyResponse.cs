// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    public partial class BodyResponse
    {
        /// <summary>
        /// Initializes a new instance of the BodyResponse class.
        /// </summary>
        public BodyResponse() { }

        /// <summary>
        /// Initializes a new instance of the BodyResponse class.
        /// </summary>
        public BodyResponse(System.Guid? id = default(System.Guid?), string bodyNumber = default(string), string name = default(string), string shortName = default(string), string organisation = default(string), System.Guid? organisationId = default(System.Guid?), string description = default(string), System.DateTime? formalValidFrom = default(System.DateTime?), System.DateTime? formalValidTo = default(System.DateTime?), bool? lifecycleValid = default(bool?), bool? hasAllSeatsAssigned = default(bool?), bool? isMepCompliant = default(bool?))
        {
            Id = id;
            BodyNumber = bodyNumber;
            Name = name;
            ShortName = shortName;
            Organisation = organisation;
            OrganisationId = organisationId;
            Description = description;
            FormalValidFrom = formalValidFrom;
            FormalValidTo = formalValidTo;
            LifecycleValid = lifecycleValid;
            HasAllSeatsAssigned = hasAllSeatsAssigned;
            IsMepCompliant = isMepCompliant;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public System.Guid? Id { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "bodyNumber")]
        public string BodyNumber { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "shortName")]
        public string ShortName { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "organisation")]
        public string Organisation { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "organisationId")]
        public System.Guid? OrganisationId { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "description")]
        public string Description { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "formalValidFrom")]
        public System.DateTime? FormalValidFrom { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "formalValidTo")]
        public System.DateTime? FormalValidTo { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "lifecycleValid")]
        public bool? LifecycleValid { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "hasAllSeatsAssigned")]
        public bool? HasAllSeatsAssigned { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "isMepCompliant")]
        public bool? IsMepCompliant { get; private set; }

    }
}