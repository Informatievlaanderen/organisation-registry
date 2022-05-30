// ReSharper disable ContextualLoggerProblem
namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure;

using Microsoft.Extensions.Logging;

public abstract class BaseProjectionMarker { }

public abstract class BaseProjection<T> : BaseProjectionMarker
{
    protected readonly ILogger<T> Logger;

    protected BaseProjection(ILogger<T> logger)
    {
        Logger = logger;

        Logger.LogTrace("Created EventHandler {ProjectionName}", typeof(T));
    }
}