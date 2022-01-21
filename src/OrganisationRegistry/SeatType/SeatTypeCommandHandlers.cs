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
        ICommandHandler<CreateSeatType>,
        ICommandHandler<UpdateSeatType>
    {
        private readonly IUniqueNameValidator<SeatType> _uniqueNameValidator;

        public SeatTypeCommandHandlers(
            ILogger<SeatTypeCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<SeatType> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public async Task Handle(CreateSeatType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUnique();

            var seatType = new SeatType(message.SeatTypeId, message.Name, message.Order, message.IsEffective);
            Session.Add(seatType);
            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateSeatType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.SeatTypeId, message.Name))
                throw new NameNotUnique();

            var seatType = Session.Get<SeatType>(message.SeatTypeId);
            seatType.Update(message.Name, message.Order, message.IsEffective);
            await Session.Commit(message.User);
        }
    }
}
