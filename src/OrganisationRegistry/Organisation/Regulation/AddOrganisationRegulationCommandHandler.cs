namespace OrganisationRegistry.Organisation;

using System;
using System.Threading.Tasks;
using Handling;
using Handling.Authorization;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Commands;
using Infrastructure.Domain;
using RegulationSubTheme;
using RegulationTheme;

public class AddOrganisationRegulationCommandHandler :
    BaseCommandHandler<AddOrganisationRegulationCommandHandler>,
    ICommandEnvelopeHandler<AddOrganisationRegulation>
{
    public AddOrganisationRegulationCommandHandler(
        ILogger<AddOrganisationRegulationCommandHandler> logger,
        ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<AddOrganisationRegulation> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithPolicy(_ => new RegulationPolicy())
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var regulationTheme = envelope.Command.RegulationThemeId != Guid.Empty ? session.Get<RegulationTheme>(envelope.Command.RegulationThemeId) : null;

                    var regulationSubTheme = envelope.Command.RegulationSubThemeId != Guid.Empty ? session.Get<RegulationSubTheme>(envelope.Command.RegulationSubThemeId) : null;

                    organisation.AddRegulation(
                        envelope.Command.OrganisationRegulationId,
                        regulationTheme,
                        regulationSubTheme,
                        envelope.Command.Name,
                        envelope.Command.Url,
                        new WorkRulesUrl(envelope.Command.WorkRulesUrl),
                        envelope.Command.Date,
                        envelope.Command.Description,
                        envelope.Command.DescriptionRendered,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
