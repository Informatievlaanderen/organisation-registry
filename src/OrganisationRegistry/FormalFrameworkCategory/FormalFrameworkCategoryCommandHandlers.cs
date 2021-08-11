namespace OrganisationRegistry.FormalFrameworkCategory
{
    using System.Threading.Tasks;
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

        public async Task Handle(CreateFormalFrameworkCategory message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            var formalFrameworkCategory = new FormalFrameworkCategory(message.FormalFrameworkCategoryId, message.Name);
            Session.Add(formalFrameworkCategory);
            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateFormalFrameworkCategory message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.FormalFrameworkCategoryId, message.Name))
                throw new NameNotUniqueException();

            var formalFrameworkCategory = Session.Get<FormalFrameworkCategory>(message.FormalFrameworkCategoryId);
            formalFrameworkCategory.Update(message.Name);
            await Session.Commit(message.User);
        }
    }
}
