namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class DeleteOrganisationLocationCommandHandler : BaseCommandHandler<DeleteOrganisationLocationCommandHandler>,
    ICommandEnvelopeHandler<DeleteOrganisationLocation>
{
    public DeleteOrganisationLocationCommandHandler(
        ILogger<DeleteOrganisationLocationCommandHandler> logger,
        ISession session) : base(
        logger,
        session)
    {
    }

    public Task Handle(ICommandEnvelope<DeleteOrganisationLocation> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);

                    organisation.RemoveLocation(envelope.Command.OrganisationLocationId);
                });
}
