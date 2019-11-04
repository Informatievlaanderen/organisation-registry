namespace OrganisationRegistry.Location
{
    using Commands;
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

        public void Handle(CreateLocation message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Address.FullAddress))
                throw new NameNotUniqueException();

            var location = new Location(message.LocationId, message.CrabLocationId, message.Address);
            Session.Add(location);
            Session.Commit();
        }

        public void Handle(UpdateLocation message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.LocationId, message.Address.FullAddress))
                throw new NameNotUniqueException();

            var location = Session.Get<Location>(message.LocationId);
            location.Update(message.CrabLocationId, message.Address);
            Session.Commit();
        }
    }
}
