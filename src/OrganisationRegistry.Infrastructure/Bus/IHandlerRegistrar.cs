namespace OrganisationRegistry.Infrastructure.Bus
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using Commands;
    using Events;
    using Messages;

    public interface IHandlerRegistrar
    {
        void RegisterCommandHandler<T>(Action<T> handler) where T : IMessage;
        void RegisterEventHandler<T>(Action<DbConnection, DbTransaction, IEnvelope<T>> handler) where T : IEvent<T>;
        void RegisterReaction<T>(Func<IEnvelope<T>, List<ICommand>> handler) where T : IEvent<T>;
    }
}
