namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class AssignBodyNumberCommandHandler
:BaseCommandHandler<AssignBodyNumberCommandHandler>,
    ICommandEnvelopeHandler<AssignBodyNumber>
{
    private readonly IBodyNumberGenerator _bodyNumberGenerator;

    public AssignBodyNumberCommandHandler(
        ILogger<AssignBodyNumberCommandHandler> logger,
        ISession session,
        IBodyNumberGenerator bodyNumberGenerator) : base(logger, session)
    {
        _bodyNumberGenerator = bodyNumberGenerator;
    }

    public async Task Handle(ICommandEnvelope<AssignBodyNumber> envelope)
    {
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.AssignBodyNumber(_bodyNumberGenerator.GenerateNumber());

        await Session.Commit(envelope.User);
    }
}
