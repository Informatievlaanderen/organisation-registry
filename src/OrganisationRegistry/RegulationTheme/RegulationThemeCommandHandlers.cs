namespace OrganisationRegistry.RegulationTheme
{
    using System.Threading.Tasks;
    using Commands;
    using Exceptions;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class RegulationThemeCommandHandlers :
        BaseCommandHandler<RegulationThemeCommandHandlers>,
        ICommandHandler<CreateRegulationTheme>,
        ICommandHandler<UpdateRegulationTheme>
    {
        private readonly IUniqueNameValidator<RegulationTheme> _uniqueNameValidator;

        public RegulationThemeCommandHandlers(
            ILogger<RegulationThemeCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<RegulationTheme> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public async Task Handle(CreateRegulationTheme message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUnique();

            var regulationTheme = new RegulationTheme(message.RegulationThemeId, message.Name);
            Session.Add(regulationTheme);
            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateRegulationTheme message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.RegulationThemeId, message.Name))
                throw new NameNotUnique();

            var regulationTheme = Session.Get<RegulationTheme>(message.RegulationThemeId);
            regulationTheme.Update(message.Name);
            await Session.Commit(message.User);
        }
    }
}
