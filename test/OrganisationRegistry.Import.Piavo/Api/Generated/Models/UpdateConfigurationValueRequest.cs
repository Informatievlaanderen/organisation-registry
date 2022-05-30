// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models;

public partial class UpdateConfigurationValueRequest
{
    /// <summary>
    /// Initializes a new instance of the UpdateConfigurationValueRequest
    /// class.
    /// </summary>
    public UpdateConfigurationValueRequest() { }

    /// <summary>
    /// Initializes a new instance of the UpdateConfigurationValueRequest
    /// class.
    /// </summary>
    public UpdateConfigurationValueRequest(string description = default(string), string value = default(string))
    {
        Description = description;
        Value = value;
    }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "value")]
    public string Value { get; set; }

}