namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class PlaceUnderVlimpersManagementCommandHandler
:BaseCommandHandler<PlaceUnderVlimpersManagementCommandHandler>,
    ICommandEnvelopeHandler<PlaceUnderVlimpersManagement>
{
    public PlaceUnderVlimpersManagementCommandHandler(ILogger<PlaceUnderVlimpersManagementCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<PlaceUnderVlimpersManagement> envelope)
        => Handle(envelope.Command, envelope.User);

    private Task Handle(PlaceUnderVlimpersManagement message, IUser user)
        => UpdateHandler<Organisation>.For(message, user, Session)
            .RequiresAdmin()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);

                    organisation.PlaceUnderVlimpersManagement();
                });
}
