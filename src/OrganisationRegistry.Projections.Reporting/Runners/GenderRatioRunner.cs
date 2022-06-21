namespace OrganisationRegistry.Projections.Reporting.Runners;

using Microsoft.Extensions.Logging;
using SqlServer.Infrastructure;
using SqlServer.ProjectionState;
using System;
using Projections;
using OrganisationRegistry.Infrastructure.Events;

public class GenderRatioRunner : BaseRunner
{
    private const string DbProjectionsProjectionName = "GenderRatioRunner";
    private static readonly string ProjectionFullName = typeof(BodySeatGenderRatioProjection).FullName!;
    private new const string ProjectionName = nameof(BodySeatGenderRatioProjection);

    private new static readonly Type[] EventHandlers =
    {
        typeof(MemoryCachesMaintainer),
        typeof(BodySeatGenderRatioProjection),
    };

    private new static readonly Type[] ReactionHandlers = { };

    public GenderRatioRunner(
        ILogger<GenderRatioRunner> logger,
        IEventStore store,
        IProjectionStates projectionStates,
        IEventPublisher bus) :
        base(
            logger,
            store,
            projectionStates,
            bus,
            DbProjectionsProjectionName,
            ProjectionFullName,
            ProjectionName,
            EventHandlers,
            ReactionHandlers)
    {
    }
}
