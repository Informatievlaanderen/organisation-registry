namespace OrganisationRegistry.SeatType
{
    using System.Threading.Tasks;
    using Commands;
    using Exceptions;
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
        {
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                throw new NameNotUnique();

            var seatType = new SeatType(envelope.Command.SeatTypeId, envelope.Command.Name, envelope.Command.Order, envelope.Command.IsEffective);
            Session.Add(seatType);
            await Session.Commit(envelope.User);
        }

        public async Task Handle(ICommandEnvelope<UpdateSeatType> envelope)
        {
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.SeatTypeId, envelope.Command.Name))
                throw new NameNotUnique();

            var seatType = Session.Get<SeatType>(envelope.Command.SeatTypeId);
            seatType.Update(envelope.Command.Name, envelope.Command.Order, envelope.Command.IsEffective);
            await Session.Commit(envelope.User);
        }
    }
}
