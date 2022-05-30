namespace OrganisationRegistry.Infrastructure;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class ProjectionRebuilderId : GuidValueObject<ProjectionRebuilderId>
{
    public ProjectionRebuilderId([JsonProperty("id")] Guid projectionRebuilderId) : base(projectionRebuilderId) { }
}