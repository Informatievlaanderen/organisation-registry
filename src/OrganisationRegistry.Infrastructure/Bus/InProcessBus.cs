namespace OrganisationRegistry.Infrastructure.Bus;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Authorization;
using Commands;
using Events;
using Messages;
using Microsoft.Extensions.Logging;

// Scoped as SingleInstance()
public class InProcessBus : ICommandSender, IEventPublisher, IHandlerRegistrar
{
    private readonly ILogger<InProcessBus> _logger;
    private readonly ISecurityService _securityService;
    private readonly Dictionary<Type, List<Func<DbConnection?, DbTransaction?, IMessage, Task>>> _eventRoutes = new();
    private readonly Dictionary<Type, List<Func<IMessage, Task<List<ICommand>>>>> _reactionRoutes = new();

    private readonly Dictionary<Type, List<Func<IMessage, Task>>> _commandRoutes = new();
    private readonly List<ICommandEnvelopeHandlerWrapper> _commandEnvelopeRoutes = new();

    public InProcessBus(ILogger<InProcessBus> logger, ISecurityService securityService)
    {
        _logger = logger;
        _securityService = securityService;
        _logger.LogTrace("Creating InProcessBus");
    }

    public void RegisterEventHandler<T>(Func<DbConnection?, DbTransaction?, IEnvelope<T>, Task> handler) where T : IEvent<T>
    {
        if (!_eventRoutes.TryGetValue(typeof(T), out var handlers))
        {
            handlers = new List<Func<DbConnection?, DbTransaction?, IMessage, Task>>();
            _eventRoutes.Add(typeof(T), handlers);
        }

        handlers.Add(async (dbConnection, dbTransaction, @event) => await handler(dbConnection, dbTransaction, (IEnvelope<T>)@event));
    }

    public void RegisterReaction<T>(Func<IEnvelope<T>, Task<List<ICommand>>> handler) where T : IEvent<T>
    {
        if (!_reactionRoutes.TryGetValue(typeof(T), out var handlers))
        {
            handlers = new List<Func<IMessage, Task<List<ICommand>>>>();
            _reactionRoutes.Add(typeof(T), handlers);
        }

        handlers.Add(async envelope => await handler((IEnvelope<T>)envelope));
    }

    public void RegisterCommandEnvelopeHandler<THandler>(THandler handler)
        where THandler : ICommandEnvelopeHandlerWrapper
    {
        _commandEnvelopeRoutes.Add(handler);
    }

    public void RegisterCommandHandler<T>(Func<T, Task> handler) where T : IMessage
    {
        if (!_commandRoutes.TryGetValue(typeof(T), out var handlers))
        {
            handlers = new List<Func<IMessage, Task>>();
            _commandRoutes.Add(typeof(T), handlers);
        }

        handlers.Add(async x => await handler((T)x));
    }

    public async Task Send<T>(T command, IUser? user = null) where T : ICommand
    {
        if (_commandEnvelopeRoutes.SingleOrDefault(x => x.CanHandle(command.GetType())) is { } envelopeHandler)
        {
            user ??= await _securityService.GetRequiredUser(ClaimsPrincipal.Current);
            var envelope = new CommandEnvelope<T>(command, user);

            await envelopeHandler.Handle(envelope);

            return;
        }

        if (_commandRoutes.TryGetValue(command.GetType(), out var handlers))
        {
            if (handlers.Count != 1)
            {
                _logger.LogCritical("Tried to send command {@Command} but got {NumberOfCommandHandlers} handlers", command, handlers.Count);
                throw new InvalidOperationException("Cannot send to more than one handler.");
            }

            _logger.LogDebug("Sending command {@Command}", command);
            await handlers[0](command);

            return;
        }

        _logger.LogCritical("Tried to send command {@Command} but got {NumberOfCommandHandlers} handlers", command, 0);
        throw new InvalidOperationException("No handler registered.");
    }

    public async Task Publish<T>(DbConnection? dbConnection, DbTransaction? dbTransaction, IEnvelope<T> envelope) where T : IEvent<T>
    {
        if (!_eventRoutes.TryGetValue(envelope.Body.GetType(), out var handlers))
            handlers = new List<Func<DbConnection?, DbTransaction?, IMessage, Task>>();

        _logger.LogDebug(
            "Publishing event {@Event} to {NumberOfEventHandlers} event handler(s)",
            envelope,
            handlers.Count);

        foreach (var handler in handlers)
            await handler(dbConnection, dbTransaction, envelope);
    }
}