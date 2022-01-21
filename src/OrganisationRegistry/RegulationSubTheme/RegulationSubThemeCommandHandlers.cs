namespace OrganisationRegistry.RegulationSubTheme
{
    using System.Threading.Tasks;
    using Commands;
    using Exceptions;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;
    using RegulationTheme;

    public class RegulationSubThemeCommandHandlers :
        BaseCommandHandler<RegulationSubThemeCommandHandlers>,
        ICommandHandler<CreateRegulationSubTheme>,
        ICommandHandler<UpdateRegulationSubTheme>
    {
        private readonly IUniqueNameWithinTypeValidator<RegulationSubTheme> _uniqueNameValidator;

        public RegulationSubThemeCommandHandlers(
            ILogger<RegulationSubThemeCommandHandlers> logger,
            ISession session,
            IUniqueNameWithinTypeValidator<RegulationSubTheme> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public async Task Handle(CreateRegulationSubTheme message)
        {
            var regulationTheme = Session.Get<RegulationTheme>(message.RegulationThemeId);

            if (_uniqueNameValidator.IsNameTaken(message.Name, message.RegulationThemeId))
                throw new NameNotUniqueWithinType();

            var regulationSubTheme =
                new RegulationSubTheme(
                    message.RegulationSubThemeId,
                    message.Name,
                    regulationTheme);

            Session.Add(regulationSubTheme);
            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateRegulationSubTheme message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.RegulationSubThemeId, message.Name, message.RegulationThemeId))
                throw new NameNotUniqueWithinType();

            var regulationTheme = Session.Get<RegulationTheme>(message.RegulationThemeId);
            var regulationSubTheme = Session.Get<RegulationSubTheme>(message.RegulationSubThemeId);
            regulationSubTheme.Update(message.Name, regulationTheme);
            await Session.Commit(message.User);
        }
    }
}
