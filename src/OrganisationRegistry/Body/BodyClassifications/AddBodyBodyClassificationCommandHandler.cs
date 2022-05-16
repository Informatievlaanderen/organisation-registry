namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using BodyClassification;
using BodyClassificationType;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class AddBodyBodyClassificationCommandHandler
    : BaseCommandHandler<AddBodyBodyClassificationCommandHandler>,
        ICommandEnvelopeHandler<AddBodyBodyClassification>
{
    public AddBodyBodyClassificationCommandHandler(ILogger<AddBodyBodyClassificationCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<AddBodyBodyClassification> envelope)
    {
        var bodyClassification = Session.Get<BodyClassification>(envelope.Command.BodyClassificationId);
        var bodyClassificationType = Session.Get<BodyClassificationType>(envelope.Command.BodyClassificationTypeId);
        var body = Session.Get<Body>(envelope.Command.BodyId);

        body.AddBodyClassification(
            envelope.Command.BodyBodyClassificationId,
            bodyClassificationType,
            bodyClassification,
            new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));

        await Session.Commit(envelope.User);
    }
}
