// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models;

public partial class OrganisationLabel
{
    /// <summary>
    /// Initializes a new instance of the OrganisationLabel class.
    /// </summary>
    public OrganisationLabel() { }

    /// <summary>
    /// Initializes a new instance of the OrganisationLabel class.
    /// </summary>
    public OrganisationLabel(System.Guid? organisationLabelId = default(System.Guid?), System.Guid? labelTypeId = default(System.Guid?), string labelTypeName = default(string), string value = default(string), Period validity = default(Period))
    {
        OrganisationLabelId = organisationLabelId;
        LabelTypeId = labelTypeId;
        LabelTypeName = labelTypeName;
        Value = value;
        Validity = validity;
    }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "organisationLabelId")]
    public System.Guid? OrganisationLabelId { get; set; }

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
    [Newtonsoft.Json.JsonProperty(PropertyName = "value")]
    public string Value { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "validity")]
    public Period Validity { get; set; }

}