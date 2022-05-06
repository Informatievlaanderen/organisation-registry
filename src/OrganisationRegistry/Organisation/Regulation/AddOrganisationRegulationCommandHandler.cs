namespace OrganisationRegistry.Organisation;

using System;
using System.Threading.Tasks;
using Commands;
using Handling;
using Handling.Authorization;
using Infrastructure.Authorization;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Commands;
using Infrastructure.Domain;
using RegulationSubTheme;
using RegulationTheme;

public class AddOrganisationRegulationCommandHandler :
    BaseCommandHandler<AddOrganisationRegulationCommandHandler>,
    ICommandEnvelopeHandler<AddOrganisationRegulation>
{
    public AddOrganisationRegulationCommandHandler(ILogger<AddOrganisationRegulationCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<AddOrganisationRegulation> envelope)
        => Handle(envelope.Command, envelope.User);

    public Task Handle(AddOrganisationRegulation message, IUser user) =>
        UpdateHandler<Organisation>.For(message, user, Session)
            .WithPolicy(_ => new RegulationPolicy())
            .Handle(session =>
            {
                var organisation = session.Get<Organisation>(message.OrganisationId);
                organisation.ThrowIfTerminated(user);

                var regulationTheme = message.RegulationThemeId != Guid.Empty ?
                    session.Get<RegulationTheme>(message.RegulationThemeId) : null;

                var regulationSubTheme = message.RegulationSubThemeId != Guid.Empty ?
                    session.Get<RegulationSubTheme>(message.RegulationSubThemeId) : null;

                organisation.AddRegulation(
                    message.OrganisationRegulationId,
                    regulationTheme,
                    regulationSubTheme,
                    message.Name,
                    message.Url,
                    new WorkRulesUrl(message.WorkRulesUrl),
                    message.Date,
                    message.Description,
                    message.DescriptionRendered,
                    new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));
            });
}
