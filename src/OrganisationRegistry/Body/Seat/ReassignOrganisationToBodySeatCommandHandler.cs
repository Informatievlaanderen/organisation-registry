namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
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
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);
        var organisation = Session.Get<Organisation>(envelope.Command.OrganisationId);

        body.ReassignOrganisationToBodySeat(
            organisation,
            envelope.Command.BodyMandateId,
            envelope.Command.BodySeatId,
            envelope.Command.Validity);

        await Session.Commit(envelope.User);
    }
}
