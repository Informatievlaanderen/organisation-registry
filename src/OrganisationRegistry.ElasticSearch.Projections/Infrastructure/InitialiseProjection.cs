namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure;

using System;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Infrastructure.Messages;

public class InitialiseProjection : IEvent<InitialiseProjection>
{
    protected Guid Id { get; set; }

    public int Version { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    Guid IMessage.Id
    {
        get => Id;
        set => Id = value;
    }

    public string ProjectionName { get; }

    public InitialiseProjection(string projectionName)
    {
        Id = Guid.NewGuid();
        ProjectionName = projectionName;
    }
}