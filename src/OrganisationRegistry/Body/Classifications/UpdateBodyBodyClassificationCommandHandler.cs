namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using BodyClassification;
using BodyClassificationType;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateBodyBodyClassificationCommandHandler
    : BaseCommandHandler<UpdateBodyBodyClassificationCommandHandler>,
        ICommandEnvelopeHandler<UpdateBodyBodyClassification>
{
    public UpdateBodyBodyClassificationCommandHandler(ILogger<UpdateBodyBodyClassificationCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<UpdateBodyBodyClassification> envelope)
    {
        var bodyClassification = Session.Get<BodyClassification>(envelope.Command.BodyClassificationId);
        var bodyClassificationType = Session.Get<BodyClassificationType>(envelope.Command.BodyClassificationTypeId);
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.UpdateBodyClassification(
            envelope.Command.BodyBodyClassificationId,
            bodyClassificationType,
            bodyClassification,
            new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));

        await Session.Commit(envelope.User);
    }
}
