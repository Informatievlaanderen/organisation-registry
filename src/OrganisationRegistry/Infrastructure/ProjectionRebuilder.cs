namespace OrganisationRegistry.Infrastructure;

using Domain;
using Events;

public class ProjectionRebuilder : AggregateRoot
{
    private ProjectionRebuilder() { }

    public ProjectionRebuilder(ProjectionRebuilderId projectionId)
    {
        Id = projectionId;
    }

    public void RebuildProjection(string projectionName)
    {
        ApplyChange(new RebuildProjection(projectionName));
    }
}