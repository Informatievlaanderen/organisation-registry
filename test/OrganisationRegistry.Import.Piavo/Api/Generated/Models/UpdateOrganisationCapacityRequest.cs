// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models;

public partial class UpdateOrganisationCapacityRequest
{
    /// <summary>
    /// Initializes a new instance of the
    /// UpdateOrganisationCapacityRequest class.
    /// </summary>
    public UpdateOrganisationCapacityRequest() { }

    /// <summary>
    /// Initializes a new instance of the
    /// UpdateOrganisationCapacityRequest class.
    /// </summary>
    public UpdateOrganisationCapacityRequest(System.Guid? organisationCapacityId = default(System.Guid?), System.Guid? capacityId = default(System.Guid?), System.Guid? personId = default(System.Guid?), System.Guid? functionId = default(System.Guid?), System.Collections.Generic.IDictionary<string, string> contacts = default(System.Collections.Generic.IDictionary<string, string>), System.DateTime? validFrom = default(System.DateTime?), System.DateTime? validTo = default(System.DateTime?))
    {
        OrganisationCapacityId = organisationCapacityId;
        CapacityId = capacityId;
        PersonId = personId;
        FunctionId = functionId;
        Contacts = contacts;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "organisationCapacityId")]
    public System.Guid? OrganisationCapacityId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "capacityId")]
    public System.Guid? CapacityId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "personId")]
    public System.Guid? PersonId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "functionId")]
    public System.Guid? FunctionId { get; set; }

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