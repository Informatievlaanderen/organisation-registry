namespace OrganisationRegistry.Infrastructure.Events
{
    using System.Collections.Generic;
    using Commands;
    using Messages;

    public interface IReactionHandler<in T> : IHandler<T> where T : IEvent<T>
    {
        List<ICommand> Handle(IEnvelope<T> message);
    }
}
