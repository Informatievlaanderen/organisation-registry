namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class RemoveOrganisationFormalFrameworkCommandHandler
    : BaseCommandHandler<RemoveOrganisationFormalFrameworkCommandHandler>
    , ICommandEnvelopeHandler<RemoveOrganisationFormalFramework>
{
    public RemoveOrganisationFormalFrameworkCommandHandler(ILogger<RemoveOrganisationFormalFrameworkCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<RemoveOrganisationFormalFramework> envelope)
        => await UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .RequiresAdmin()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.Id);
                    organisation.RemoveFormalFramework(envelope.Command.OrganisationFormalFrameworkId);
                });
}
