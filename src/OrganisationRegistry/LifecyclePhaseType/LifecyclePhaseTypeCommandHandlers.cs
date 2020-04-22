namespace OrganisationRegistry.LifecyclePhaseType
{
    using System.Threading.Tasks;
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class LifecyclePhaseTypeCommandHandlers :
        BaseCommandHandler<LifecyclePhaseTypeCommandHandlers>,
        ICommandHandler<CreateLifecyclePhaseType>,
        ICommandHandler<UpdateLifecyclePhaseType>
    {
        private readonly IUniqueNameValidator<LifecyclePhaseType> _uniqueNameValidator;
        private readonly IOnlyOneDefaultLifecyclePhaseTypeValidator _onlyOneDefaultLifecyclePhaseTypeValidator;

        public LifecyclePhaseTypeCommandHandlers(
            ILogger<LifecyclePhaseTypeCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<LifecyclePhaseType> uniqueNameValidator,
            IOnlyOneDefaultLifecyclePhaseTypeValidator onlyOneDefaultLifecyclePhaseTypeValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
            _onlyOneDefaultLifecyclePhaseTypeValidator = onlyOneDefaultLifecyclePhaseTypeValidator;
        }

        public async Task Handle(CreateLifecyclePhaseType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            if (_onlyOneDefaultLifecyclePhaseTypeValidator.ViolatesOnlyOneDefaultLifecyclePhaseTypeConstraint(message.LifecyclePhaseTypeIsRepresentativeFor, message.Status))
                throw new DefaultLifecyclePhaseAlreadyPresentException(message.LifecyclePhaseTypeIsRepresentativeFor);

            var lifecyclePhaseType = new LifecyclePhaseType(
                message.LifecyclePhaseTypeId,
                message.Name,
                message.LifecyclePhaseTypeIsRepresentativeFor,
                message.Status);

            Session.Add(lifecyclePhaseType);
            await Session.Commit();
        }

        public async Task Handle(UpdateLifecyclePhaseType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.LifecyclePhaseTypeId, message.Name))
                throw new NameNotUniqueException();

            if (_onlyOneDefaultLifecyclePhaseTypeValidator.ViolatesOnlyOneDefaultLifecyclePhaseTypeConstraint(message.LifecyclePhaseTypeId, message.LifecyclePhaseTypeIsRepresentativeFor, message.Status))
                throw new DefaultLifecyclePhaseAlreadyPresentException(message.LifecyclePhaseTypeIsRepresentativeFor);

            var lifecyclePhaseType = Session.Get<LifecyclePhaseType>(message.LifecyclePhaseTypeId);

            lifecyclePhaseType.Update(
                message.Name,
                message.LifecyclePhaseTypeIsRepresentativeFor,
                message.Status);

            await Session.Commit();
        }
    }
}
