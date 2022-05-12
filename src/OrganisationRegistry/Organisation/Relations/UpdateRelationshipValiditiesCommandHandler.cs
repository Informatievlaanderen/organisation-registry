namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateRelationshipValiditiesCommandHandler
    : BaseCommandHandler<UpdateRelationshipValiditiesCommandHandler>,
        ICommandEnvelopeHandler<UpdateRelationshipValidities>
{
    public UpdateRelationshipValiditiesCommandHandler(ILogger<UpdateRelationshipValiditiesCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<UpdateRelationshipValidities> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    organisation.UpdateRelationshipValidities(envelope.Command.Date);
                });
}
