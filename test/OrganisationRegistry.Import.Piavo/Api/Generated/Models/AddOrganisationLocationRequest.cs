// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models;

public partial class AddOrganisationLocationRequest
{
    /// <summary>
    /// Initializes a new instance of the AddOrganisationLocationRequest
    /// class.
    /// </summary>
    public AddOrganisationLocationRequest() { }

    /// <summary>
    /// Initializes a new instance of the AddOrganisationLocationRequest
    /// class.
    /// </summary>
    public AddOrganisationLocationRequest(System.Guid? organisationLocationId = default(System.Guid?), System.Guid? locationId = default(System.Guid?), bool? isMainLocation = default(bool?), System.Guid? locationTypeId = default(System.Guid?), System.DateTime? validFrom = default(System.DateTime?), System.DateTime? validTo = default(System.DateTime?))
    {
        OrganisationLocationId = organisationLocationId;
        LocationId = locationId;
        IsMainLocation = isMainLocation;
        LocationTypeId = locationTypeId;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "organisationLocationId")]
    public System.Guid? OrganisationLocationId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "locationId")]
    public System.Guid? LocationId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "isMainLocation")]
    public bool? IsMainLocation { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "locationTypeId")]
    public System.Guid? LocationTypeId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "validFrom")]
    public System.DateTime? ValidFrom { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "validTo")]
    public System.DateTime? ValidTo { get; set; }

}