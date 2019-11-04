namespace OrganisationRegistry.FormalFrameworkCategory
{
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class FormalFrameworkCategoryCommandHandlers :
        BaseCommandHandler<FormalFrameworkCategoryCommandHandlers>,
        ICommandHandler<CreateFormalFrameworkCategory>,
        ICommandHandler<UpdateFormalFrameworkCategory>
    {
        private readonly IUniqueNameValidator<FormalFrameworkCategory> _uniqueNameValidator;

        public FormalFrameworkCategoryCommandHandlers(
            ILogger<FormalFrameworkCategoryCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<FormalFrameworkCategory> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public void Handle(CreateFormalFrameworkCategory message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            var formalFrameworkCategory = new FormalFrameworkCategory(message.FormalFrameworkCategoryId, message.Name);
            Session.Add(formalFrameworkCategory);
            Session.Commit();
        }

        public void Handle(UpdateFormalFrameworkCategory message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.FormalFrameworkCategoryId, message.Name))
                throw new NameNotUniqueException();

            var formalFrameworkCategory = Session.Get<FormalFrameworkCategory>(message.FormalFrameworkCategoryId);
            formalFrameworkCategory.Update(message.Name);
            Session.Commit();
        }
    }
}
