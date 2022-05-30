// ReSharper disable ContextualLoggerProblem
namespace OrganisationRegistry;

using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public abstract class BaseCommandHandler<T>
{
    protected ILogger<T> Logger { get; }
    protected ISession Session { get; }

    protected BaseCommandHandler(
        ILogger<T> logger,
        ISession session)
    {
        Logger = logger;
        Session = session;

        Logger.LogTrace("Created CommandHandler {CommandHandlerName}", typeof(T));
    }
}