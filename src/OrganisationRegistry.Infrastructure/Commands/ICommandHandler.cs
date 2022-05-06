﻿namespace OrganisationRegistry.Infrastructure.Commands
{
    using System;
    using System.Threading.Tasks;
    using Messages;

    public interface ICommandHandler<in T> : IHandler<T> where T : ICommand
    {
        Task Handle(T message);
    }

    public interface ICommandEnvelopeHandler
    {
    }

    public interface ICommandEnvelopeHandler<TCommand>: ICommandEnvelopeHandler
        where TCommand : ICommand
    {
        Task Handle(ICommandEnvelope<TCommand> envelope);
    }

    public interface ICommandEnvelopeHandlerWrapper
    {
        public bool CanHandle<T>();

        public Task Handle(ICommandEnvelope command);
    }

    public class CommandEnvelopeHandlerWrapper<TCommand> : ICommandEnvelopeHandlerWrapper
        where TCommand : ICommand
    {
        private readonly Func<IServiceProvider> _requestScopedServiceProvider;

        /// <summary>
        /// used from reflection, don't add parameters
        /// you may create an overload
        /// </summary>
        /// <param name="requestScopedServiceProvider"></param>
        public CommandEnvelopeHandlerWrapper(Func<IServiceProvider> requestScopedServiceProvider)
        {
            _requestScopedServiceProvider = requestScopedServiceProvider;
        }

        public bool CanHandle<T>()
            => typeof(T) == typeof(TCommand) && MaybeGetCommandEnvelopeHandler() != null;


        public Task Handle(ICommandEnvelope commandEnvelope)
        {
            if (commandEnvelope is not ICommandEnvelope<TCommand> theCommandEnvelope)
                return Task.CompletedTask;

            if (MaybeGetCommandEnvelopeHandler() is { } commandEnvelopeHandler)
                return commandEnvelopeHandler.Handle(theCommandEnvelope);

            return Task.CompletedTask;
        }

        private ICommandEnvelopeHandler<TCommand>? MaybeGetCommandEnvelopeHandler()
        {
            var serviceProvider = _requestScopedServiceProvider();
            var maybeCommandEnvelopeHandler = serviceProvider.GetService(typeof(ICommandEnvelopeHandler<TCommand>));
            return (ICommandEnvelopeHandler<TCommand>?)maybeCommandEnvelopeHandler;
        }
    }
}
