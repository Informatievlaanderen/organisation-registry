namespace OrganisationRegistry.BodyClassification;

using System.Threading.Tasks;
using BodyClassificationType;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class BodyClassificationCommandHandlers :
    BaseCommandHandler<BodyClassificationCommandHandlers>,
    ICommandEnvelopeHandler<CreateBodyClassification>,
    ICommandEnvelopeHandler<UpdateBodyClassification>
{
    private readonly IUniqueNameWithinTypeValidator<BodyClassification> _uniqueNameValidator;

    public BodyClassificationCommandHandlers(
        ILogger<BodyClassificationCommandHandlers> logger,
        ISession session,
        IUniqueNameWithinTypeValidator<BodyClassification> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateBodyClassification> envelope)
    {
        await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    var bodyClassificationType = session.Get<BodyClassificationType>(envelope.Command.BodyClassificationTypeId);

                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name, envelope.Command.BodyClassificationTypeId))
                        throw new NameNotUniqueWithinType();

                    var bodyClassification = new BodyClassification(envelope.Command.BodyClassificationId, envelope.Command.Name, envelope.Command.Order, envelope.Command.Active, bodyClassificationType);
                    session.Add(bodyClassification);
                });
    }

    public async Task Handle(ICommandEnvelope<UpdateBodyClassification> envelope)
        => await UpdateHandler<BodyClassification>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.BodyClassificationId, envelope.Command.Name, envelope.Command.BodyClassificationTypeId))
                        throw new NameNotUniqueWithinType();

                    var bodyClassificationType = session.Get<BodyClassificationType>(envelope.Command.BodyClassificationTypeId);
                    var bodyClassification = session.Get<BodyClassification>(envelope.Command.BodyClassificationId);
                    bodyClassification.Update(envelope.Command.Name, envelope.Command.Order, envelope.Command.Active, bodyClassificationType);
                });
}
