// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    public partial class OrganisationKeyListItem
    {
        /// <summary>
        /// Initializes a new instance of the OrganisationKeyListItem class.
        /// </summary>
        public OrganisationKeyListItem() { }

        /// <summary>
        /// Initializes a new instance of the OrganisationKeyListItem class.
        /// </summary>
        public OrganisationKeyListItem(System.Guid? organisationKeyId = default(System.Guid?), System.Guid? organisationId = default(System.Guid?), System.Guid? keyTypeId = default(System.Guid?), string keyTypeName = default(string), string keyValue = default(string), System.DateTime? validFrom = default(System.DateTime?), System.DateTime? validTo = default(System.DateTime?))
        {
            OrganisationKeyId = organisationKeyId;
            OrganisationId = organisationId;
            KeyTypeId = keyTypeId;
            KeyTypeName = keyTypeName;
            KeyValue = keyValue;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "organisationKeyId")]
        public System.Guid? OrganisationKeyId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "organisationId")]
        public System.Guid? OrganisationId { get; set; }

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
        [Newtonsoft.Json.JsonProperty(PropertyName = "keyValue")]
        public string KeyValue { get; set; }

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
