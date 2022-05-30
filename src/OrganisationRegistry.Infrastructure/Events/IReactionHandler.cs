namespace OrganisationRegistry.Infrastructure.Events;

using System.Collections.Generic;
using System.Threading.Tasks;
using Commands;
using Messages;

public interface IReactionHandler<in T> : IHandler<T> where T : IEvent<T>
{
    Task<List<ICommand>> Handle(IEnvelope<T> message);
}