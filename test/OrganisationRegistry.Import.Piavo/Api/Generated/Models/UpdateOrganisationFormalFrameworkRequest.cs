// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models;

public partial class UpdateOrganisationFormalFrameworkRequest
{
    /// <summary>
    /// Initializes a new instance of the
    /// UpdateOrganisationFormalFrameworkRequest class.
    /// </summary>
    public UpdateOrganisationFormalFrameworkRequest() { }

    /// <summary>
    /// Initializes a new instance of the
    /// UpdateOrganisationFormalFrameworkRequest class.
    /// </summary>
    public UpdateOrganisationFormalFrameworkRequest(System.Guid? organisationFormalFrameworkId = default(System.Guid?), System.Guid? formalFrameworkId = default(System.Guid?), System.Guid? parentOrganisationId = default(System.Guid?), System.DateTime? validFrom = default(System.DateTime?), System.DateTime? validTo = default(System.DateTime?))
    {
        OrganisationFormalFrameworkId = organisationFormalFrameworkId;
        FormalFrameworkId = formalFrameworkId;
        ParentOrganisationId = parentOrganisationId;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "organisationFormalFrameworkId")]
    public System.Guid? OrganisationFormalFrameworkId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "formalFrameworkId")]
    public System.Guid? FormalFrameworkId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "parentOrganisationId")]
    public System.Guid? ParentOrganisationId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "validFrom")]
    public System.DateTime? ValidFrom { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "validTo")]
    public System.DateTime? ValidTo { get; set; }

}