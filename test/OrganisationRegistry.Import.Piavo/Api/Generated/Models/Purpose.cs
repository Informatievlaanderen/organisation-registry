// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models;

public partial class Purpose
{
    /// <summary>
    /// Initializes a new instance of the Purpose class.
    /// </summary>
    public Purpose() { }

    /// <summary>
    /// Initializes a new instance of the Purpose class.
    /// </summary>
    public Purpose(System.Guid? purposeId = default(System.Guid?), string purposeName = default(string))
    {
        PurposeId = purposeId;
        PurposeName = purposeName;
    }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "purposeId")]
    public System.Guid? PurposeId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "purposeName")]
    public string PurposeName { get; set; }

}