namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using FormalFramework;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateBodyFormalFrameworkCommandHandler
    :BaseCommandHandler<UpdateBodyFormalFrameworkCommandHandler>,
        ICommandEnvelopeHandler<UpdateBodyFormalFramework>
{
    public UpdateBodyFormalFrameworkCommandHandler(ILogger<UpdateBodyFormalFrameworkCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<UpdateBodyFormalFramework> envelope)
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var formalFramework = session.Get<FormalFramework>(envelope.Command.FormalFrameworkId);
                    var body = session.Get<Body>(envelope.Command.BodyId);

                    body.UpdateFormalFramework(
                        envelope.Command.BodyFormalFrameworkId,
                        formalFramework,
                        envelope.Command.Validity);
                });
}
