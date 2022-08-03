namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using BodyClassification;
using BodyClassificationType;
using Handling;
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
        => await UpdateHandler<Body>.For(envelope.Command, envelope.User, Session)
            .WithEditBodyPolicy()
            .Handle(
                session =>
                {
                    var bodyClassification = session.Get<BodyClassification>(envelope.Command.BodyClassificationId);
                    var bodyClassificationType = session.Get<BodyClassificationType>(envelope.Command.BodyClassificationTypeId);
                    var body = session.Get<Body>(envelope.Command.BodyId);

                    body.AddBodyClassification(
                        envelope.Command.BodyBodyClassificationId,
                        bodyClassificationType,
                        bodyClassification,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
