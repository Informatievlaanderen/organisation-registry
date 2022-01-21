namespace OrganisationRegistry.Location
{
    using System.Threading.Tasks;
    using Commands;
    using Exceptions;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class LocationCommandHandlers :
        BaseCommandHandler<LocationCommandHandlers>,
        ICommandHandler<CreateLocation>,
        ICommandHandler<UpdateLocation>
    {
        private readonly IUniqueNameValidator<Location> _uniqueNameValidator;

        public LocationCommandHandlers(
            ILogger<LocationCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<Location> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public async Task Handle(CreateLocation message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Address.FullAddress))
                throw new NameNotUnique();

            var location = new Location(message.LocationId, message.CrabLocationId, message.Address);
            Session.Add(location);
            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateLocation message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.LocationId, message.Address.FullAddress))
                throw new NameNotUnique();

            var location = Session.Get<Location>(message.LocationId);
            location.Update(message.CrabLocationId, message.Address);
            await Session.Commit(message.User);
        }
    }
}
