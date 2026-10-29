namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Authorization;
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
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithBodyPolicy(Permission.BodiesCanManageMandates)
            .Handle(
                session =>
                {
                    var body = session.Get<Body>(envelope.Command.BodyId);

                    body.RemoveBodyMandate(
                        envelope.Command.BodySeatId,
                        envelope.Command.BodyMandateId);
                });
}
