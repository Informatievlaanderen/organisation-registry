namespace OrganisationRegistry.Api.Infrastructure;

using System.Runtime.Serialization;
using Newtonsoft.Json;

public class BasicApiProblem
{
    [JsonProperty("problemType", DefaultValueHandling = DefaultValueHandling.Ignore)]
    [DataMember(Name = "problemType", Order = 100, EmitDefaultValue = false)]
    public string? ProblemTypeUrl { get; set; }

    [JsonProperty("title")]
    [DataMember(Name = "title", Order = 200, EmitDefaultValue = false)]
    public string? Title { get; set; }

    [JsonProperty("detail")]
    [DataMember(Name = "detail", Order = 300, EmitDefaultValue = false)]
    public string? Detail { get; set; }

    [JsonProperty("httpStatus")]
    [DataMember(Name = "httpStatus", Order = 400, EmitDefaultValue = false)]
    public string? HttpStatus { get; set; }

    [JsonProperty("details", DefaultValueHandling = DefaultValueHandling.Ignore)]
    [DataMember(Name = "details", Order = 500, EmitDefaultValue = false)]
    public string? Details { get; set; }

    [JsonProperty("problemInstance", DefaultValueHandling = DefaultValueHandling.Ignore)]
    [DataMember(Name = "problemInstance", Order = 600, EmitDefaultValue = false)]
    public string? ProblemInstance { get; set; }

    [JsonProperty("reference", DefaultValueHandling = DefaultValueHandling.Ignore)]
    [DataMember(Name = "reference", Order = 700, EmitDefaultValue = false)]
    public string? Reference { get; set; }
}