// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models;

public partial class BodyContactListItem
{
    /// <summary>
    /// Initializes a new instance of the BodyContactListItem class.
    /// </summary>
    public BodyContactListItem() { }

    /// <summary>
    /// Initializes a new instance of the BodyContactListItem class.
    /// </summary>
    public BodyContactListItem(System.Guid? bodyContactId = default(System.Guid?), System.Guid? bodyId = default(System.Guid?), System.Guid? contactTypeId = default(System.Guid?), string contactTypeName = default(string), string contactValue = default(string), System.DateTime? validFrom = default(System.DateTime?), System.DateTime? validTo = default(System.DateTime?))
    {
        BodyContactId = bodyContactId;
        BodyId = bodyId;
        ContactTypeId = contactTypeId;
        ContactTypeName = contactTypeName;
        ContactValue = contactValue;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "bodyContactId")]
    public System.Guid? BodyContactId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "bodyId")]
    public System.Guid? BodyId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "contactTypeId")]
    public System.Guid? ContactTypeId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "contactTypeName")]
    public string ContactTypeName { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "contactValue")]
    public string ContactValue { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "validFrom")]
    public System.DateTime? ValidFrom { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "validTo")]
    public System.DateTime? ValidTo { get; set; }

}