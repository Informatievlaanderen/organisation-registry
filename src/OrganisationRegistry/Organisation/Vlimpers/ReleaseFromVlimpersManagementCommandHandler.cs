namespace OrganisationRegistry.Organisation.Vlimpers;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class ReleaseFromVlimpersManagementCommandHandler :
    BaseCommandHandler<ReleaseFromVlimpersManagementCommandHandler>,
    ICommandEnvelopeHandler<ReleaseFromVlimpersManagement>
{
    public ReleaseFromVlimpersManagementCommandHandler(ILogger<ReleaseFromVlimpersManagementCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<ReleaseFromVlimpersManagement> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .RequiresAdmin()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);

                    organisation.ReleaseFromVlimpersManagement();
                });
}
