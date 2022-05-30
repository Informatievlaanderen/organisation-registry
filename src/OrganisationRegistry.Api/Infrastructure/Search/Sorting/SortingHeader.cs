namespace OrganisationRegistry.Api.Infrastructure.Search.Sorting;

using Newtonsoft.Json;

public enum SortOrder
{
    Ascending,
    Descending
}

public class SortingHeader
{
    [JsonIgnore]
    public bool ShouldSort => !string.IsNullOrWhiteSpace(SortBy);

    public string SortBy { get; }

    public SortOrder SortOrder { get; }

    public SortingHeader(string sortBy, SortOrder sortOrder)
    {
        SortBy = sortBy;
        SortOrder = sortOrder;
    }
}