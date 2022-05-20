namespace OrganisationRegistry.BodyClassification
{
    using System.Threading.Tasks;
    using BodyClassificationType;
    using Commands;
    using Exceptions;
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
            var bodyClassificationType = Session.Get<BodyClassificationType>(envelope.Command.BodyClassificationTypeId);

            if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name, envelope.Command.BodyClassificationTypeId))
                throw new NameNotUniqueWithinType();

            var bodyClassification = new BodyClassification(envelope.Command.BodyClassificationId, envelope.Command.Name, envelope.Command.Order, envelope.Command.Active, bodyClassificationType);
            Session.Add(bodyClassification);
            await Session.Commit(envelope.User);
        }

        public async Task Handle(ICommandEnvelope<UpdateBodyClassification> envelope)
        {
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.BodyClassificationId, envelope.Command.Name, envelope.Command.BodyClassificationTypeId))
                throw new NameNotUniqueWithinType();

            var bodyClassificationType = Session.Get<BodyClassificationType>(envelope.Command.BodyClassificationTypeId);
            var bodyClassification = Session.Get<BodyClassification>(envelope.Command.BodyClassificationId);
            bodyClassification.Update(envelope.Command.Name, envelope.Command.Order, envelope.Command.Active, bodyClassificationType);
            await Session.Commit(envelope.User);
        }
    }
}
