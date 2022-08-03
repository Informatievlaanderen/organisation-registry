namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Organisation;

public class ReassignOrganisationToBodySeatCommandHandler
:BaseCommandHandler<ReassignOrganisationToBodySeatCommandHandler>,
    ICommandEnvelopeHandler<ReassignOrganisationToBodySeat>
{
    public ReassignOrganisationToBodySeatCommandHandler(ILogger<ReassignOrganisationToBodySeatCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<ReassignOrganisationToBodySeat> envelope)
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var body = session.Get<Body>(envelope.Command.BodyId);
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);

                    body.ReassignOrganisationToBodySeat(
                        organisation,
                        envelope.Command.BodyMandateId,
                        envelope.Command.BodySeatId,
                        envelope.Command.Validity);
                });
}
