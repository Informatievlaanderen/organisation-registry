namespace OrganisationRegistry.Infrastructure.Bus
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading.Tasks;
    using Commands;
    using Events;
    using Messages;
    using Microsoft.Extensions.Logging;

    // Scoped as SingleInstance()
    public class InProcessBus : ICommandSender, IEventPublisher, IHandlerRegistrar
    {
        private readonly ILogger<InProcessBus> _logger;
        private readonly Dictionary<Type, List<Action<DbConnection, DbTransaction, IMessage>>> _eventRoutes = new Dictionary<Type, List<Action<DbConnection, DbTransaction, IMessage>>>();
        private readonly Dictionary<Type, List<Func<IMessage, List<ICommand>>>> _reactionRoutes = new Dictionary<Type, List<Func<IMessage, List<ICommand>>>>();
        private readonly Dictionary<Type, List<Action<IMessage>>> _commandRoutes = new Dictionary<Type, List<Action<IMessage>>>();

        public InProcessBus(ILogger<InProcessBus> logger)
        {
            _logger = logger;
            _logger.LogTrace("Creating InProcessBus.");
        }

        public void RegisterEventHandler<T>(Action<DbConnection, DbTransaction, IEnvelope<T>> handler) where T : IEvent<T>
        {
            if (!_eventRoutes.TryGetValue(typeof(T), out List<Action<DbConnection, DbTransaction, IMessage>> handlers))
            {
                handlers = new List<Action<DbConnection, DbTransaction, IMessage>>();
                _eventRoutes.Add(typeof(T), handlers);
            }

            handlers.Add((dbConnection, dbTransaction, @event) => handler(dbConnection, dbTransaction, (IEnvelope<T>)@event));
        }

        public void RegisterReaction<T>(Func<IEnvelope<T>, List<ICommand>> handler) where T : IEvent<T>
        {
            if (!_reactionRoutes.TryGetValue(typeof(T), out var handlers))
            {
                handlers = new List<Func<IMessage, List<ICommand>>>();
                _reactionRoutes.Add(typeof(T), handlers);
            }

            handlers.Add(envelope => handler((IEnvelope<T>)envelope));
        }

        public void RegisterCommandHandler<T>(Action<T> handler) where T : IMessage
        {
            if (!_commandRoutes.TryGetValue(typeof(T), out var handlers))
            {
                handlers = new List<Action<IMessage>>();
                _commandRoutes.Add(typeof(T), handlers);
            }

            handlers.Add(x => handler((T)x));
        }

        public void Send<T>(T command) where T : ICommand
        {
            if (_commandRoutes.TryGetValue(command.GetType(), out var handlers))
            {
                if (handlers.Count != 1)
                {
                    _logger.LogCritical("Tried to send command {@Command} but got {NumberOfCommandHandlers} handlers.", command, handlers.Count);
                    throw new InvalidOperationException("Cannot send to more than one handler.");
                }

                _logger.LogDebug("Sending command {@Command}", command);
                handlers[0](command);
            }
            else
            {
                _logger.LogCritical("Tried to send command {@Command} but got {NumberOfCommandHandlers} handlers.", command, 0);
                throw new InvalidOperationException("No handler registered.");
            }
        }

        public void Publish<T>(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<T> envelope) where T : IEvent<T>
        {
            if (!_eventRoutes.TryGetValue(envelope.Body.GetType(), out var handlers))
                handlers = new List<Action<DbConnection, DbTransaction, IMessage>>();

            _logger.LogDebug(
                $"Publishing event {{@Event}} to {{NumberOfEventHandlers}} event {(handlers.Count == 1 ? "handler" : "handlers")}.",
                envelope,
                handlers.Count);

            foreach (var handler in handlers)
                handler(dbConnection, dbTransaction, envelope);
        }

        public void ProcessReactions<T>(IEnvelope<T> envelope) where T : IEvent<T>
        {
            if (!_reactionRoutes.TryGetValue(envelope.Body.GetType(), out var reactions))
                reactions = new List<Func<IMessage, List<ICommand>>>();

            _logger.LogDebug(
                $"Publishing event {{@Event}} to {{NumberOfReactionHandlers}} reaction {(reactions.Count == 1 ? "handler" : "handlers")}.",
                envelope,
                reactions.Count);

            Task.Run(() =>
            {
                try
                {
                    reactions.ForEach(reaction =>
                        reaction(envelope).ForEach(Send));

                    if (reactions.Count > 0)
                        _logger.LogInformation("Processed all reactions.");
                }
                catch (Exception e)
                {
                    _logger.LogCritical(0, e, "Error while handling reactions.");
                }
            });
        }
    }
}
