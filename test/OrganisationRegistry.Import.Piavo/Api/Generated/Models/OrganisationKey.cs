// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    public partial class OrganisationKey
    {
        /// <summary>
        /// Initializes a new instance of the OrganisationKey class.
        /// </summary>
        public OrganisationKey() { }

        /// <summary>
        /// Initializes a new instance of the OrganisationKey class.
        /// </summary>
        public OrganisationKey(System.Guid? organisationKeyId = default(System.Guid?), System.Guid? keyTypeId = default(System.Guid?), string keyTypeName = default(string), string value = default(string), Period validity = default(Period))
        {
            OrganisationKeyId = organisationKeyId;
            KeyTypeId = keyTypeId;
            KeyTypeName = keyTypeName;
            Value = value;
            Validity = validity;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "organisationKeyId")]
        public System.Guid? OrganisationKeyId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "keyTypeId")]
        public System.Guid? KeyTypeId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "keyTypeName")]
        public string KeyTypeName { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "validity")]
        public Period Validity { get; set; }

    }
}
