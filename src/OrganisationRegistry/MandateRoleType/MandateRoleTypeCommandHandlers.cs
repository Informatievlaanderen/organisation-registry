namespace OrganisationRegistry.MandateRoleType
{
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class MandateRoleTypeCommandHandlers :
        BaseCommandHandler<MandateRoleTypeCommandHandlers>,
        ICommandHandler<CreateMandateRoleType>,
        ICommandHandler<UpdateMandateRoleType>
    {
        private readonly IUniqueNameValidator<MandateRoleType> _uniqueNameValidator;

        public MandateRoleTypeCommandHandlers(
            ILogger<MandateRoleTypeCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<MandateRoleType> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public void Handle(CreateMandateRoleType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            var mandateRoleType = new MandateRoleType(message.MandateRoleTypeId, message.Name);
            Session.Add(mandateRoleType);
            Session.Commit();
        }

        public void Handle(UpdateMandateRoleType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.MandateRoleTypeId, message.Name))
                throw new NameNotUniqueException();

            var mandateRoleType = Session.Get<MandateRoleType>(message.MandateRoleTypeId);
            mandateRoleType.Update(message.Name);
            Session.Commit();
        }
    }
}
