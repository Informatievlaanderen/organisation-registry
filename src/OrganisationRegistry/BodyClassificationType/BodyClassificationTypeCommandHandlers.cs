namespace OrganisationRegistry.BodyClassificationType
{
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class BodyClassificationTypeCommandHandlers :
        BaseCommandHandler<BodyClassificationTypeCommandHandlers>,
        ICommandHandler<CreateBodyClassificationType>,
        ICommandHandler<UpdateBodyClassificationType>
    {
        private readonly IUniqueNameValidator<BodyClassificationType> _uniqueNameValidator;

        public BodyClassificationTypeCommandHandlers(
            ILogger<BodyClassificationTypeCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<BodyClassificationType> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public void Handle(CreateBodyClassificationType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            var bodyClassificationType = new BodyClassificationType(message.BodyClassificationTypeId, message.Name);
            Session.Add(bodyClassificationType);
            Session.Commit();
        }

        public void Handle(UpdateBodyClassificationType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.BodyClassificationTypeId, message.Name))
                throw new NameNotUniqueException();

            var bodyClassificationType = Session.Get<BodyClassificationType>(message.BodyClassificationTypeId);
            bodyClassificationType.Update(message.Name);
            Session.Commit();
        }
    }
}
