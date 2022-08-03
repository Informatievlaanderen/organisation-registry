namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using BodyClassification;
using BodyClassificationType;
using Handling;
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
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var bodyClassification = session.Get<BodyClassification>(envelope.Command.BodyClassificationId);
                    var bodyClassificationType = session.Get<BodyClassificationType>(envelope.Command.BodyClassificationTypeId);
                    var body = session.Get<Body>(envelope.Command.BodyId);

                    body.UpdateBodyClassification(
                        envelope.Command.BodyBodyClassificationId,
                        bodyClassificationType,
                        bodyClassification,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));

                });
}
