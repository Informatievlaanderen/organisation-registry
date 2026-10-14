namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Commands;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;


public class RemoveOrganisationKeyCommandHandler :
    BaseCommandHandler<RemoveOrganisationKeyCommandHandler>,
    ICommandEnvelopeHandler<RemoveOrganisationKey>
{
    public RemoveOrganisationKeyCommandHandler(
        ILogger<RemoveOrganisationKeyCommandHandler> logger,
        ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<RemoveOrganisationKey> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithKeyPolicy(envelope.Command)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);

                    organisation.RemoveOrganisationKey(envelope.Command.OrganisationKeyId);
                });
}
