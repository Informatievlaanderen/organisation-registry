namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using BodyClassification;
using BodyClassificationType;
using Infrastructure.Authorization;
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

    public Task Handle(ICommandEnvelope<UpdateBodyBodyClassification> envelope)
        => Handle(envelope.Command, envelope.User);

    private async Task Handle(UpdateBodyBodyClassification message, IUser user)
    {
        var bodyClassification = Session.Get<BodyClassification>(message.BodyClassificationId);
        var bodyClassificationType = Session.Get<BodyClassificationType>(message.BodyClassificationTypeId);
        var body = Session.Get<Body>(message.BodyId);

        body.UpdateBodyClassification(
            message.BodyBodyClassificationId,
            bodyClassificationType,
            bodyClassification,
            new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

        await Session.Commit(user);
    }
}
