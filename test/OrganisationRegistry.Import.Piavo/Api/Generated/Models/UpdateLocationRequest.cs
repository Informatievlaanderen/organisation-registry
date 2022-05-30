// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models;

public partial class UpdateLocationRequest
{
    /// <summary>
    /// Initializes a new instance of the UpdateLocationRequest class.
    /// </summary>
    public UpdateLocationRequest() { }

    /// <summary>
    /// Initializes a new instance of the UpdateLocationRequest class.
    /// </summary>
    public UpdateLocationRequest(string crabLocationId = default(string), string street = default(string), string zipCode = default(string), string city = default(string), string country = default(string))
    {
        CrabLocationId = crabLocationId;
        Street = street;
        ZipCode = zipCode;
        City = city;
        Country = country;
    }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "crabLocationId")]
    public string CrabLocationId { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "street")]
    public string Street { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "zipCode")]
    public string ZipCode { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "city")]
    public string City { get; set; }

    /// <summary>
    /// </summary>
    [Newtonsoft.Json.JsonProperty(PropertyName = "country")]
    public string Country { get; set; }

}