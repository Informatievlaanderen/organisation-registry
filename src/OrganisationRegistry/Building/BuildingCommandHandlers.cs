namespace OrganisationRegistry.Building
{
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class BuildingCommandHandlers :
        BaseCommandHandler<BuildingCommandHandlers>,
        ICommandHandler<CreateBuilding>,
        ICommandHandler<UpdateBuilding>
    {
        private readonly IUniqueNameValidator<Building> _uniqueNameValidator;

        public BuildingCommandHandlers(
            ILogger<BuildingCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<Building> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public void Handle(CreateBuilding message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            var building = new Building(message.BuildingId, message.Name, message.VimId);
            Session.Add(building);
            Session.Commit();
        }

        public void Handle(UpdateBuilding message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.BuildingId, message.Name))
                throw new NameNotUniqueException();

            var building = Session.Get<Building>(message.BuildingId);
            building.Update(message.Name, message.VimId);
            Session.Commit();
        }
    }
}
