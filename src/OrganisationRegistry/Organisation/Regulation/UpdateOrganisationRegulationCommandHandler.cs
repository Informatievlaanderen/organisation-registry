namespace OrganisationRegistry.Organisation;

using System;
using System.Threading.Tasks;
using Commands;
using Handling;
using Handling.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using RegulationSubTheme;
using RegulationTheme;

public class UpdateOrganisationRegulationCommandHandler:
    BaseCommandHandler<UpdateOrganisationRegulationCommandHandler>,
    ICommandEnvelopeHandler<UpdateOrganisationRegulation>
{
    public UpdateOrganisationRegulationCommandHandler(ILogger<UpdateOrganisationRegulationCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationRegulation> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithPolicy(_ => new RegulationPolicy())
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var regulationTheme = envelope.Command.RegulationThemeId != Guid.Empty
                        ? session.Get<RegulationTheme>(envelope.Command.RegulationThemeId)
                        : null;

                    var regulationSubTheme = envelope.Command.RegulationSubThemeId != Guid.Empty
                        ? session.Get<RegulationSubTheme>(envelope.Command.RegulationSubThemeId)
                        : null;

                    organisation.UpdateRegulation(
                        envelope.Command.OrganisationRegulationId,
                        regulationTheme,
                        regulationSubTheme,
                        envelope.Command.Name,
                        envelope.Command.Link,
                        new WorkRulesUrl(envelope.Command.WorkRulesUrl),
                        envelope.Command.Date,
                        envelope.Command.Description,
                        envelope.Command.DescriptionRendered,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
