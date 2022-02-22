// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    public partial class OrganisationLabelListItem
    {
        /// <summary>
        /// Initializes a new instance of the OrganisationLabelListItem class.
        /// </summary>
        public OrganisationLabelListItem() { }

        /// <summary>
        /// Initializes a new instance of the OrganisationLabelListItem class.
        /// </summary>
        public OrganisationLabelListItem(System.Guid? organisationLabelId = default(System.Guid?), System.Guid? organisationId = default(System.Guid?), System.Guid? labelTypeId = default(System.Guid?), string labelTypeName = default(string), string labelValue = default(string), System.DateTime? validFrom = default(System.DateTime?), System.DateTime? validTo = default(System.DateTime?))
        {
            OrganisationLabelId = organisationLabelId;
            OrganisationId = organisationId;
            LabelTypeId = labelTypeId;
            LabelTypeName = labelTypeName;
            LabelValue = labelValue;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "organisationLabelId")]
        public System.Guid? OrganisationLabelId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "organisationId")]
        public System.Guid? OrganisationId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "labelTypeId")]
        public System.Guid? LabelTypeId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "labelTypeName")]
        public string LabelTypeName { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "labelValue")]
        public string LabelValue { get; set; }

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
