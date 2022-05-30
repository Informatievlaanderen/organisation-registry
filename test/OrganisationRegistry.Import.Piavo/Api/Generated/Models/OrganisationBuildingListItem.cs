// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models;

public partial class OrganisationBuildingListItem
{
    /// <summary>
    /// Initializes a new instance of the OrganisationBuildingListItem
    /// class.
    /// </summary>
    public OrganisationBuildingListItem() { }

    /// <summary>
    /// Initializes a new instance of the OrganisationBuildingListItem
    /// class.
    /// </summary>
    public OrganisationBuildingListItem(System.Guid? organisationBuildingId = default(System.Guid?), System.Guid? organisationId = default(System.Guid?), System.Guid? buildingId = default(System.Guid?), string buildingName = default(string), bool? isMainBuilding = default(bool?), System.DateTime? validFrom = default(System.DateTime?), System.DateTime? validTo = default(System.DateTime?))
    {
        OrganisationBuildingId = organisationBuildingId;
        OrganisationId = organisationId;
        BuildingId = buildingId;
        BuildingName = buildingName;
        IsMainBuilding = isMainBuilding;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "organisationBuildingId")]
    public System.Guid? OrganisationBuildingId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "organisationId")]
    public System.Guid? OrganisationId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "buildingId")]
    public System.Guid? BuildingId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "buildingName")]
    public string BuildingName { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "isMainBuilding")]
    public bool? IsMainBuilding { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "validFrom")]
    public System.DateTime? ValidFrom { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "validTo")]
    public System.DateTime? ValidTo { get; set; }

}