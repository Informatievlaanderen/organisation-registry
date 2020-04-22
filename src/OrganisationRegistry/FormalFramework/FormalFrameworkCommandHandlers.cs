namespace OrganisationRegistry.FormalFramework
{
    using System.Threading.Tasks;
    using Commands;
    using FormalFrameworkCategory;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class FormalFrameworkCommandHandlers :
        BaseCommandHandler<FormalFrameworkCommandHandlers>,
        ICommandHandler<CreateFormalFramework>,
        ICommandHandler<UpdateFormalFramework>
    {
        private readonly IUniqueNameWithinTypeValidator<FormalFramework> _uniqueNameValidator;
        private readonly IUniqueCodeValidator<FormalFramework> _uniqueCodeValidator;

        public FormalFrameworkCommandHandlers(
            ILogger<FormalFrameworkCommandHandlers> logger,
            ISession session,
            IUniqueNameWithinTypeValidator<FormalFramework> uniqueNameValidator,
            IUniqueCodeValidator<FormalFramework> uniqueCodeValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
            _uniqueCodeValidator = uniqueCodeValidator;
        }

        public async Task Handle(CreateFormalFramework message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name, message.FormalFrameworkCategoryId))
                throw new NameNotUniqueWithinTypeException();

            if (_uniqueCodeValidator.IsCodeTaken(message.Code))
                throw new CodeNotUniqueException();

            var formalFrameworkCategory = Session.Get<FormalFrameworkCategory>(message.FormalFrameworkCategoryId);

            var formalFramework = new FormalFramework(message.FormalFrameworkId, message.Name, message.Code, formalFrameworkCategory);
            Session.Add(formalFramework);
            await Session.Commit();
        }

        public async Task Handle(UpdateFormalFramework message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.FormalFrameworkId, message.Name, message.FormalFrameworkCategoryId))
                throw new NameNotUniqueWithinTypeException();

            if (_uniqueCodeValidator.IsCodeTaken(message.FormalFrameworkId, message.Code))
                throw new CodeNotUniqueException();

            var formalFrameworkCategory = Session.Get<FormalFrameworkCategory>(message.FormalFrameworkCategoryId);

            var formalFramework = Session.Get<FormalFramework>(message.FormalFrameworkId);
            formalFramework.Update(message.Name, message.Code, formalFrameworkCategory);
            await Session.Commit();
        }
    }
}
