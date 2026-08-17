namespace OrganisationRegistry.ElasticSearch.Projections.IndividualRebuild;

using System;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.SqlServer.Infrastructure;

public record IndividualRebuildRunnerConfig<TAggregate, TDocument, TToRebuild>(
    string ProjectionStateKey,
    Type[] EventHandlers,
    Func<OrganisationRegistryContext, DbSet<TToRebuild>> GetToRebuildSet,
    Func<TToRebuild, Guid> GetAggregateId)
    where TToRebuild : class;
