namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using FormalFramework;
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
    {
        var formalFramework = Session.Get<FormalFramework>(envelope.Command.FormalFrameworkId);
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.UpdateFormalFramework(
            envelope.Command.BodyFormalFrameworkId,
            formalFramework,
            envelope.Command.Validity);

        await Session.Commit(envelope.User);
    }
}
