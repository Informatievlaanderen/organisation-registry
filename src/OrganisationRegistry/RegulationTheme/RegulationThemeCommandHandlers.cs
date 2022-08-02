namespace OrganisationRegistry.RegulationTheme;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
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
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                        throw new NameNotUnique();

                    var regulationTheme = new RegulationTheme(envelope.Command.RegulationThemeId, envelope.Command.Name);
                    session.Add(regulationTheme);
                });

    public async Task Handle(ICommandEnvelope<UpdateRegulationTheme> envelope)
        => await UpdateHandler<RegulationTheme>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.RegulationThemeId, envelope.Command.Name))
                        throw new NameNotUnique();

                    var regulationTheme = session.Get<RegulationTheme>(envelope.Command.RegulationThemeId);
                    regulationTheme.Update(envelope.Command.Name);
                });
}
