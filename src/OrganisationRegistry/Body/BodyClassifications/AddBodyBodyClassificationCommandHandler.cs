namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using BodyClassification;
using BodyClassificationType;
using Infrastructure.Authorization;
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

    public Task Handle(ICommandEnvelope<AddBodyBodyClassification> envelope)
        => Handle(envelope.Command, envelope.User);

    private async Task Handle(AddBodyBodyClassification message, IUser user)
    {
        var bodyClassification = Session.Get<BodyClassification>(message.BodyClassificationId);
        var bodyClassificationType = Session.Get<BodyClassificationType>(message.BodyClassificationTypeId);
        var body = Session.Get<Body>(message.BodyId);

        body.AddBodyClassification(
            message.BodyBodyClassificationId,
            bodyClassificationType,
            bodyClassification,
            new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

        await Session.Commit(user);
    }
}
