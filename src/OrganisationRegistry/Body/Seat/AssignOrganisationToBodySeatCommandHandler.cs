namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Organisation;

public class AssignOrganisationToBodySeatCommandHandler
:BaseCommandHandler<AssignOrganisationToBodySeatCommandHandler>,
    ICommandEnvelopeHandler<AssignOrganisationToBodySeat>
{
    public AssignOrganisationToBodySeatCommandHandler(ILogger<AssignOrganisationToBodySeatCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<AssignOrganisationToBodySeat> envelope)
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var body = session.Get<Body>(envelope.Command.BodyId);
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);

                    body.AssignOrganisationToBodySeat(
                        organisation,
                        envelope.Command.BodyMandateId,
                        envelope.Command.BodySeatId,
                        envelope.Command.Validity);
                });
}
