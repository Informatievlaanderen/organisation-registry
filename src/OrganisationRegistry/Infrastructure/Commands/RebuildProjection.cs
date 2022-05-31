namespace OrganisationRegistry.Infrastructure.Commands;

using System;

public class RebuildProjection : BaseCommand<ProjectionRebuilderId>
{
    private static readonly ProjectionRebuilderId ManMadeGuid = new ProjectionRebuilderId(Guid.Parse("00000000-0000-4000-0000-000000000002"));

    public ProjectionRebuilderId ProjectionRebuilderId => Id;

    public string ProjectionName { get; set; }

    public RebuildProjection(string projectionName)
    {
        Id = ManMadeGuid;
        ProjectionName = projectionName;
    }
}
