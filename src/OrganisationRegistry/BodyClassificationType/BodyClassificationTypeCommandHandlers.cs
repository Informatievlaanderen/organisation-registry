namespace OrganisationRegistry.BodyClassificationType
{
    using System.Threading.Tasks;
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

        public async Task Handle(CreateBodyClassificationType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            var bodyClassificationType = new BodyClassificationType(message.BodyClassificationTypeId, message.Name);
            Session.Add(bodyClassificationType);
            await Session.Commit();
        }

        public async Task Handle(UpdateBodyClassificationType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.BodyClassificationTypeId, message.Name))
                throw new NameNotUniqueException();

            var bodyClassificationType = Session.Get<BodyClassificationType>(message.BodyClassificationTypeId);
            bodyClassificationType.Update(message.Name);
            await Session.Commit();
        }
    }
}
