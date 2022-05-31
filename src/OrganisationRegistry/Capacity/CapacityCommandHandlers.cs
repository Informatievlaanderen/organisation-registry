namespace OrganisationRegistry.Capacity;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class CapacityCommandHandlers :
    BaseCommandHandler<CapacityCommandHandlers>,
    ICommandEnvelopeHandler<CreateCapacity>,
    ICommandEnvelopeHandler<UpdateCapacity>,
    ICommandEnvelopeHandler<RemoveCapacity>
{
    private readonly IUniqueNameValidator<Capacity> _uniqueNameValidator;

    public CapacityCommandHandlers(
        ILogger<CapacityCommandHandlers> logger,
        ISession session,
        IUniqueNameValidator<Capacity> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateCapacity> envelope)
    {
        var command = envelope.Command;

        if (_uniqueNameValidator.IsNameTaken(command.Name))
            throw new NameNotUnique();

        var capacity = new Capacity(command.CapacityId, command.Name);
        Session.Add(capacity);
        await Session.Commit(envelope.User);
    }

    public async Task Handle(ICommandEnvelope<UpdateCapacity> envelope)
    {
        var command = envelope.Command;

        if (_uniqueNameValidator.IsNameTaken(command.CapacityId, command.Name))
            throw new NameNotUnique();

        var capacity = Session.Get<Capacity>(command.CapacityId);
        capacity.Update(command.Name);
        await Session.Commit(envelope.User);
    }

    public async Task Handle(ICommandEnvelope<RemoveCapacity> envelope)
    {
        var capacity = Session.Get<Capacity>(envelope.Command.CapacityId);
        capacity.Remove();
        await Session.Commit(envelope.User);
    }
}
