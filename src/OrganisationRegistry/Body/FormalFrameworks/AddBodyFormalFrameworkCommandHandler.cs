namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using FormalFramework;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class AddBodyFormalFrameworkCommandHandler
:BaseCommandHandler<AddBodyFormalFrameworkCommandHandler>,
    ICommandEnvelopeHandler<AddBodyFormalFramework>
{
    public AddBodyFormalFrameworkCommandHandler(ILogger<AddBodyFormalFrameworkCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<AddBodyFormalFramework> envelope)
    {
        var formalFramework = Session.Get<FormalFramework>(envelope.Command.FormalFrameworkId);
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.AddFormalFramework(
            envelope.Command.BodyFormalFrameworkId,
            formalFramework,
            envelope.Command.Validity);

        await Session.Commit(envelope.User);
    }
}
