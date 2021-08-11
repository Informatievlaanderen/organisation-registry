namespace OrganisationRegistry.RegulationType
{
    using System.Threading.Tasks;
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class RegulationTypeCommandHandlers :
        BaseCommandHandler<RegulationTypeCommandHandlers>,
        ICommandHandler<CreateRegulationType>,
        ICommandHandler<UpdateRegulationType>
    {
        private readonly IUniqueNameValidator<RegulationType> _uniqueNameValidator;

        public RegulationTypeCommandHandlers(
            ILogger<RegulationTypeCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<RegulationType> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public async Task Handle(CreateRegulationType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            var regulationType = new RegulationType(message.RegulationTypeId, message.Name);
            Session.Add(regulationType);
            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateRegulationType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.RegulationTypeId, message.Name))
                throw new NameNotUniqueException();

            var regulationType = Session.Get<RegulationType>(message.RegulationTypeId);
            regulationType.Update(message.Name);
            await Session.Commit(message.User);
        }
    }
}
