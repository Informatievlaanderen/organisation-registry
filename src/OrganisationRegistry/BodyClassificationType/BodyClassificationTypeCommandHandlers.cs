namespace OrganisationRegistry.BodyClassificationType
{
    using System.Threading.Tasks;
    using Commands;
    using Exceptions;
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
        {
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                throw new NameNotUnique();

            var bodyClassificationType = new BodyClassificationType(envelope.Command.BodyClassificationTypeId, envelope.Command.Name);
            Session.Add(bodyClassificationType);
            await Session.Commit(envelope.User);
        }

        public async Task Handle(ICommandEnvelope<UpdateBodyClassificationType> envelope)
        {
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.BodyClassificationTypeId, envelope.Command.Name))
                throw new NameNotUnique();

            var bodyClassificationType = Session.Get<BodyClassificationType>(envelope.Command.BodyClassificationTypeId);
            bodyClassificationType.Update(envelope.Command.Name);
            await Session.Commit(envelope.User);
        }
    }
}
