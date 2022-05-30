namespace OrganisationRegistry.Infrastructure.Events;

using System;

public class RebuildProjection : BaseEvent<RebuildProjection>
{
    public string ProjectionName { get; }

    public RebuildProjection(string projectionName)
    {
        Id = Guid.NewGuid();
        ProjectionName = projectionName;
    }
}