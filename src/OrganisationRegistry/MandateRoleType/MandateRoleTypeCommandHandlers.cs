namespace OrganisationRegistry.MandateRoleType
{
    using System.Threading.Tasks;
    using Commands;
    using Exceptions;
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

        public async Task Handle(CreateMandateRoleType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUnique();

            var mandateRoleType = new MandateRoleType(message.MandateRoleTypeId, message.Name);
            Session.Add(mandateRoleType);
            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateMandateRoleType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.MandateRoleTypeId, message.Name))
                throw new NameNotUnique();

            var mandateRoleType = Session.Get<MandateRoleType>(message.MandateRoleTypeId);
            mandateRoleType.Update(message.Name);
            await Session.Commit(message.User);
        }
    }
}
