namespace OrganisationRegistry.SeatType;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class SeatTypeCommandHandlers :
    BaseCommandHandler<SeatTypeCommandHandlers>,
    ICommandEnvelopeHandler<CreateSeatType>,
    ICommandEnvelopeHandler<UpdateSeatType>
{
    private readonly IUniqueNameValidator<SeatType> _uniqueNameValidator;

    public SeatTypeCommandHandlers(
        ILogger<SeatTypeCommandHandlers> logger,
        ISession session,
        IUniqueNameValidator<SeatType> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateSeatType> envelope)
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                        throw new NameNotUnique();

                    var seatType = new SeatType(envelope.Command.SeatTypeId, envelope.Command.Name, envelope.Command.Order, envelope.Command.IsEffective);
                    session.Add(seatType);
                });

    public async Task Handle(ICommandEnvelope<UpdateSeatType> envelope)
        => await UpdateHandler<SeatType>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.SeatTypeId, envelope.Command.Name))
                        throw new NameNotUnique();

                    var seatType = session.Get<SeatType>(envelope.Command.SeatTypeId);
                    seatType.Update(envelope.Command.Name, envelope.Command.Order, envelope.Command.IsEffective);
                });
}
