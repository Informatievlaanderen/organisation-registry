namespace OrganisationRegistry.Infrastructure.Bus
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading.Tasks;
    using Commands;
    using Events;
    using Messages;

    public interface IHandlerRegistrar
    {
        void RegisterCommandHandler<T>(Func<T, Task> handler) where T : IMessage;
        void RegisterCommandEnvelopeHandler<THandler>(THandler handler)
            where THandler : ICommandEnvelopeHandlerWrapper;
        void RegisterEventHandler<T>(Func<DbConnection?, DbTransaction?, IEnvelope<T>, Task> handler) where T : IEvent<T>;
        void RegisterReaction<T>(Func<IEnvelope<T>, Task<List<ICommand>>> handler) where T : IEvent<T>;
    }
}
