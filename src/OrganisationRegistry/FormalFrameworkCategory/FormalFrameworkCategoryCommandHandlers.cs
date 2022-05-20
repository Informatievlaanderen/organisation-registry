namespace OrganisationRegistry.FormalFrameworkCategory
{
    using System.Threading.Tasks;
    using Commands;
    using Exceptions;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class FormalFrameworkCategoryCommandHandlers :
        BaseCommandHandler<FormalFrameworkCategoryCommandHandlers>,
        ICommandEnvelopeHandler<CreateFormalFrameworkCategory>,
        ICommandEnvelopeHandler<UpdateFormalFrameworkCategory>
    {
        private readonly IUniqueNameValidator<FormalFrameworkCategory> _uniqueNameValidator;

        public FormalFrameworkCategoryCommandHandlers(
            ILogger<FormalFrameworkCategoryCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<FormalFrameworkCategory> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public async Task Handle(ICommandEnvelope<CreateFormalFrameworkCategory> envelope)
        {
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                throw new NameNotUnique();

            var formalFrameworkCategory = new FormalFrameworkCategory(envelope.Command.FormalFrameworkCategoryId, envelope.Command.Name);
            Session.Add(formalFrameworkCategory);
            await Session.Commit(envelope.User);
        }

        public async Task Handle(ICommandEnvelope<UpdateFormalFrameworkCategory> envelope)
        {
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.FormalFrameworkCategoryId, envelope.Command.Name))
                throw new NameNotUnique();

            var formalFrameworkCategory = Session.Get<FormalFrameworkCategory>(envelope.Command.FormalFrameworkCategoryId);
            formalFrameworkCategory.Update(envelope.Command.Name);
            await Session.Commit(envelope.User);
        }
    }
}
