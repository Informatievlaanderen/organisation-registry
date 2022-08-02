namespace OrganisationRegistry.RegulationSubTheme;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using RegulationTheme;

public class RegulationSubThemeCommandHandlers :
    BaseCommandHandler<RegulationSubThemeCommandHandlers>,
    ICommandEnvelopeHandler<CreateRegulationSubTheme>,
    ICommandEnvelopeHandler<UpdateRegulationSubTheme>
{
    private readonly IUniqueNameWithinTypeValidator<RegulationSubTheme> _uniqueNameValidator;

    public RegulationSubThemeCommandHandlers(
        ILogger<RegulationSubThemeCommandHandlers> logger,
        ISession session,
        IUniqueNameWithinTypeValidator<RegulationSubTheme> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateRegulationSubTheme> envelope)
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    var regulationTheme = session.Get<RegulationTheme>(envelope.Command.RegulationThemeId);

                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name, envelope.Command.RegulationThemeId))
                        throw new NameNotUniqueWithinType();

                    var regulationSubTheme =
                        new RegulationSubTheme(
                            envelope.Command.RegulationSubThemeId,
                            envelope.Command.Name,
                            regulationTheme);

                    session.Add(regulationSubTheme);
                });

    public async Task Handle(ICommandEnvelope<UpdateRegulationSubTheme> envelope)
        => await UpdateHandler<RegulationTheme>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.RegulationSubThemeId, envelope.Command.Name, envelope.Command.RegulationThemeId))
                        throw new NameNotUniqueWithinType();

                    var regulationTheme = session.Get<RegulationTheme>(envelope.Command.RegulationThemeId);
                    var regulationSubTheme = session.Get<RegulationSubTheme>(envelope.Command.RegulationSubThemeId);
                    regulationSubTheme.Update(envelope.Command.Name, regulationTheme);
                });
}
