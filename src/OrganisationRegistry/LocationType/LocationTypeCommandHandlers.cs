namespace OrganisationRegistry.LocationType
{
    using System.Threading.Tasks;
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class LocationTypeCommandHandlers :
        BaseCommandHandler<LocationTypeCommandHandlers>,
        ICommandHandler<CreateLocationType>,
        ICommandHandler<UpdateLocationType>
    {
        private readonly IUniqueNameValidator<LocationType> _uniqueNameValidator;

        public LocationTypeCommandHandlers(
            ILogger<LocationTypeCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<LocationType> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public async Task Handle(CreateLocationType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            var locationType = new LocationType(message.LocationTypeId, message.Name);
            Session.Add(locationType);
            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateLocationType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.LocationTypeId, message.Name))
                throw new NameNotUniqueException();

            var locationType = Session.Get<LocationType>(message.LocationTypeId);
            locationType.Update(message.Name);
            await Session.Commit(message.User);
        }
    }
}
