namespace OrganisationRegistry.Building
{
    using System.Threading.Tasks;
    using Commands;
    using Exceptions;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class BuildingCommandHandlers :
        BaseCommandHandler<BuildingCommandHandlers>,
        ICommandEnvelopeHandler<CreateBuilding>,
        ICommandEnvelopeHandler<UpdateBuilding>
    {
        private readonly IUniqueNameValidator<Building> _uniqueNameValidator;

        public BuildingCommandHandlers(
            ILogger<BuildingCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<Building> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public async Task Handle(ICommandEnvelope<CreateBuilding> envelope)
        {
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                throw new NameNotUnique();

            var building = new Building(envelope.Command.BuildingId, envelope.Command.Name, envelope.Command.VimId);
            Session.Add(building);
            await Session.Commit(envelope.User);
        }

        public async Task Handle(ICommandEnvelope<UpdateBuilding> envelope)
        {
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.BuildingId, envelope.Command.Name))
                throw new NameNotUnique();

            var building = Session.Get<Building>(envelope.Command.BuildingId);
            building.Update(envelope.Command.Name, envelope.Command.VimId);
            await Session.Commit(envelope.User);
        }
    }
}
