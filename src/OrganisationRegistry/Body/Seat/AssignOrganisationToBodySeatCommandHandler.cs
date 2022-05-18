namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
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
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);
        var organisation = Session.Get<Organisation>(envelope.Command.OrganisationId);

        body.AssignOrganisationToBodySeat(
            organisation,
            envelope.Command.BodyMandateId,
            envelope.Command.BodySeatId,
            envelope.Command.Validity);

        await Session.Commit(envelope.User);
    }
}
