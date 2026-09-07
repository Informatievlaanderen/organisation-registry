namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class RemoveOrganisationCapacityCommandHandler:
    BaseCommandHandler<RemoveOrganisationCapacityCommandHandler>,
    ICommandEnvelopeHandler<RemoveOrganisationCapacity>
{
    public RemoveOrganisationCapacityCommandHandler(
        ILogger<RemoveOrganisationCapacityCommandHandler> logger,
        ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<RemoveOrganisationCapacity> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithCapacityPolicy(envelope.Command)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);

                    organisation.RemoveOrganisationCapacity(envelope.Command.OrganisationCapacityId);
                });
}
