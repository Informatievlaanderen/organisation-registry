namespace OrganisationRegistry.BodyClassification
{
    using BodyClassificationType;
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class BodyClassificationCommandHandlers :
        BaseCommandHandler<BodyClassificationCommandHandlers>,
        ICommandHandler<CreateBodyClassification>,
        ICommandHandler<UpdateBodyClassification>
    {
        private readonly IUniqueNameWithinTypeValidator<BodyClassification> _uniqueNameValidator;

        public BodyClassificationCommandHandlers(
            ILogger<BodyClassificationCommandHandlers> logger,
            ISession session,
            IUniqueNameWithinTypeValidator<BodyClassification> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public void Handle(CreateBodyClassification message)
        {
            var bodyClassificationType = Session.Get<BodyClassificationType>(message.BodyClassificationTypeId);

            if (_uniqueNameValidator.IsNameTaken(message.Name, message.BodyClassificationTypeId))
                throw new NameNotUniqueWithinTypeException();

            var bodyClassification = new BodyClassification(message.BodyClassificationId, message.Name, message.Order, message.Active, bodyClassificationType);
            Session.Add(bodyClassification);
            Session.Commit();
        }

        public void Handle(UpdateBodyClassification message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.BodyClassificationId, message.Name, message.BodyClassificationTypeId))
                throw new NameNotUniqueWithinTypeException();

            var bodyClassificationType = Session.Get<BodyClassificationType>(message.BodyClassificationTypeId);
            var bodyClassification = Session.Get<BodyClassification>(message.BodyClassificationId);
            bodyClassification.Update(message.Name, message.Order, message.Active, bodyClassificationType);
            Session.Commit();
        }
    }
}
