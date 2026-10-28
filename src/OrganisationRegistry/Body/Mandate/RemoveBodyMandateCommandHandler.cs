namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class RemoveBodyMandateCommandHandler
    : BaseCommandHandler<RemoveBodyMandateCommandHandler>,
      ICommandEnvelopeHandler<RemoveBodyMandate>
{
    public RemoveBodyMandateCommandHandler(
        ILogger<RemoveBodyMandateCommandHandler> logger,
        ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<RemoveBodyMandate> envelope)
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.RemoveBodyMandate(
            envelope.Command.BodySeatId,
            envelope.Command.BodyMandateId);

        await Session.Commit(envelope.User);
    }
}