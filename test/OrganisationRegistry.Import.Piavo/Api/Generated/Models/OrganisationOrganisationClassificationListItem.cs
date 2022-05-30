// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models;

public partial class OrganisationOrganisationClassificationListItem
{
    /// <summary>
    /// Initializes a new instance of the
    /// OrganisationOrganisationClassificationListItem class.
    /// </summary>
    public OrganisationOrganisationClassificationListItem() { }

    /// <summary>
    /// Initializes a new instance of the
    /// OrganisationOrganisationClassificationListItem class.
    /// </summary>
    public OrganisationOrganisationClassificationListItem(System.Guid? organisationOrganisationClassificationId = default(System.Guid?), System.Guid? organisationId = default(System.Guid?), System.Guid? organisationClassificationTypeId = default(System.Guid?), string organisationClassificationTypeName = default(string), System.Guid? organisationClassificationId = default(System.Guid?), string organisationClassificationName = default(string), System.DateTime? validFrom = default(System.DateTime?), System.DateTime? validTo = default(System.DateTime?))
    {
        OrganisationOrganisationClassificationId = organisationOrganisationClassificationId;
        OrganisationId = organisationId;
        OrganisationClassificationTypeId = organisationClassificationTypeId;
        OrganisationClassificationTypeName = organisationClassificationTypeName;
        OrganisationClassificationId = organisationClassificationId;
        OrganisationClassificationName = organisationClassificationName;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "organisationOrganisationClassificationId")]
    public System.Guid? OrganisationOrganisationClassificationId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "organisationId")]
    public System.Guid? OrganisationId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "organisationClassificationTypeId")]
    public System.Guid? OrganisationClassificationTypeId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "organisationClassificationTypeName")]
    public string OrganisationClassificationTypeName { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "organisationClassificationId")]
    public System.Guid? OrganisationClassificationId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "organisationClassificationName")]
    public string OrganisationClassificationName { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "validFrom")]
    public System.DateTime? ValidFrom { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "validTo")]
    public System.DateTime? ValidTo { get; set; }

}