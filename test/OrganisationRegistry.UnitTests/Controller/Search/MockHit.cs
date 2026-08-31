namespace OrganisationRegistry.UnitTests.Controller.Search;

using System.Collections.Generic;
using ElasticSearch.Organisations;
using Osc;

public class MockHit : IHit<OrganisationDocument>
{
    public MockHit(OrganisationDocument document)
    {
        Source = document;
    }
    public string Id { get; }
    public string Index { get; }
    public long? PrimaryTerm { get; }
    public string Routing { get; }
    public long? SequenceNumber { get; }
    public OrganisationDocument Source { get; }
    public string Type { get; }
    public long Version { get; }
    public Explanation Explanation { get; }
    public FieldValues Fields { get; }
    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Highlight { get; }
    public IReadOnlyDictionary<string, InnerHitsResult> InnerHits { get; }
    public NestedIdentity Nested { get; }
    public IReadOnlyCollection<string> MatchedQueries { get; }
    public double? Score { get; }
    public IReadOnlyCollection<object> Sorts { get; }
}
