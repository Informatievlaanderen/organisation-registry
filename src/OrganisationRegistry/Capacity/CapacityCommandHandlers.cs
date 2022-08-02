namespace OrganisationRegistry.Capacity;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
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
        await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    var command = envelope.Command;

                    if (_uniqueNameValidator.IsNameTaken(command.Name))
                        throw new NameNotUnique();

                    var capacity = new Capacity(command.CapacityId, command.Name);
                    session.Add(capacity);
                });
    }

    public async Task Handle(ICommandEnvelope<UpdateCapacity> envelope)
    {
        await UpdateHandler<Capacity>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    var command = envelope.Command;

                    if (_uniqueNameValidator.IsNameTaken(command.CapacityId, command.Name))
                        throw new NameNotUnique();

                    var capacity = session.Get<Capacity>(command.CapacityId);
                    capacity.Update(command.Name);
                });
    }

    public async Task Handle(ICommandEnvelope<RemoveCapacity> envelope)
    {
        await UpdateHandler<Capacity>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    var capacity = session.Get<Capacity>(envelope.Command.CapacityId);
                    capacity.Remove();
                });
    }
}
