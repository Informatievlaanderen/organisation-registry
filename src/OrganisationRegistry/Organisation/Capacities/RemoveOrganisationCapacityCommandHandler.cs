namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class RemoveOrganisationCapacityCommandHandler:
    BaseCommandHandler<RemoveOrganisationCapacityCommandHandler>,
    ICommandEnvelopeHandler<RemoveOrganisationCapacity>
{
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;

    public RemoveOrganisationCapacityCommandHandler(
        ILogger<RemoveOrganisationCapacityCommandHandler> logger,
        ISession session,
        IOrganisationRegistryConfiguration organisationRegistryConfiguration) : base(logger, session)
    {
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
    }

    public Task Handle(ICommandEnvelope<RemoveOrganisationCapacity> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithCapacityPolicy(_organisationRegistryConfiguration, envelope.Command)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);

                    organisation.RemoveOrganisationCapacity(envelope.Command.OrganisationCapacityId);
                });
}
