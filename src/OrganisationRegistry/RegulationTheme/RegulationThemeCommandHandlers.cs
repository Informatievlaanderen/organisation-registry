namespace OrganisationRegistry.RegulationTheme;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class RegulationThemeCommandHandlers :
    BaseCommandHandler<RegulationThemeCommandHandlers>,
    ICommandEnvelopeHandler<CreateRegulationTheme>,
    ICommandEnvelopeHandler<UpdateRegulationTheme>
{
    private readonly IUniqueNameValidator<RegulationTheme> _uniqueNameValidator;

    public RegulationThemeCommandHandlers(
        ILogger<RegulationThemeCommandHandlers> logger,
        ISession session,
        IUniqueNameValidator<RegulationTheme> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateRegulationTheme> envelope)
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
            throw new NameNotUnique();

        var regulationTheme = new RegulationTheme(envelope.Command.RegulationThemeId, envelope.Command.Name);
        Session.Add(regulationTheme);
        await Session.Commit(envelope.User);
    }

    public async Task Handle(ICommandEnvelope<UpdateRegulationTheme> envelope)
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.RegulationThemeId, envelope.Command.Name))
            throw new NameNotUnique();

        var regulationTheme = Session.Get<RegulationTheme>(envelope.Command.RegulationThemeId);
        regulationTheme.Update(envelope.Command.Name);
        await Session.Commit(envelope.User);
    }
}
