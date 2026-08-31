namespace OrganisationRegistry.UnitTests.Controller.Search;

using System;
using System.Collections.Generic;
using ElasticSearch.Organisations;
using OpenSearch.Net;
using Osc;

public class MockSearchResponse : ISearchResponse<OrganisationDocument>
{
    public MockSearchResponse(MockHit[] hits)
    {
        Hits = hits;
    }
    public IApiCallDetails ApiCall { get; set; }
    public bool TryGetServerErrorReason(out string reason)
        => throw new NotImplementedException();

    public string DebugInformation { get; }

    public bool IsValid
        => true;
    public Exception OriginalException { get; }
    public ServerError ServerError { get; }
    public AggregateDictionary Aggregations { get; }
    public ClusterStatistics Clusters { get; }
    public IReadOnlyCollection<OrganisationDocument> Documents { get; }
    public IReadOnlyCollection<FieldValues> Fields { get; }
    public IReadOnlyCollection<IHit<OrganisationDocument>> Hits { get; }
    public IHitsMetadata<OrganisationDocument> HitsMetadata { get; }
    public double MaxScore { get; }
    public long NumberOfReducePhases { get; }
    public Profile Profile { get; }
    public string ScrollId { get; }
    public ShardStatistics Shards { get; }
    public ISuggestDictionary<OrganisationDocument> Suggest { get; }
    public bool TerminatedEarly { get; }
    public bool TimedOut { get; }
    public long Took { get; }
    public long Total { get; }
}
