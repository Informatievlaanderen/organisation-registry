// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models;

public partial class UpdateLocationTypeRequest
{
    /// <summary>
    /// Initializes a new instance of the UpdateLocationTypeRequest class.
    /// </summary>
    public UpdateLocationTypeRequest() { }

    /// <summary>
    /// Initializes a new instance of the UpdateLocationTypeRequest class.
    /// </summary>
    public UpdateLocationTypeRequest(string name = default(string))
    {
        Name = name;
    }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

}