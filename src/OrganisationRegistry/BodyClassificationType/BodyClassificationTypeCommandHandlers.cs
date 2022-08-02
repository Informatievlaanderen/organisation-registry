namespace OrganisationRegistry.BodyClassificationType;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class BodyClassificationTypeCommandHandlers :
    BaseCommandHandler<BodyClassificationTypeCommandHandlers>,
    ICommandEnvelopeHandler<CreateBodyClassificationType>,
    ICommandEnvelopeHandler<UpdateBodyClassificationType>
{
    private readonly IUniqueNameValidator<BodyClassificationType> _uniqueNameValidator;

    public BodyClassificationTypeCommandHandlers(
        ILogger<BodyClassificationTypeCommandHandlers> logger,
        ISession session,
        IUniqueNameValidator<BodyClassificationType> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateBodyClassificationType> envelope)
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                        throw new NameNotUnique();

                    var bodyClassificationType = new BodyClassificationType(envelope.Command.BodyClassificationTypeId, envelope.Command.Name);
                    session.Add(bodyClassificationType);
                });

    public async Task Handle(ICommandEnvelope<UpdateBodyClassificationType> envelope)
        => await UpdateHandler<BodyClassificationType>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.BodyClassificationTypeId, envelope.Command.Name))
                        throw new NameNotUnique();

                    var bodyClassificationType = session.Get<BodyClassificationType>(envelope.Command.BodyClassificationTypeId);
                    bodyClassificationType.Update(envelope.Command.Name);
                });
}
