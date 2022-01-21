namespace OrganisationRegistry.Purpose
{
    using System.Threading.Tasks;
    using Commands;
    using Exceptions;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class PurposeCommandHandlers :
        BaseCommandHandler<PurposeCommandHandlers>,
        ICommandHandler<CreatePurpose>,
        ICommandHandler<UpdatePurpose>
    {
        private readonly IUniqueNameValidator<Purpose> _uniqueNameValidator;

        public PurposeCommandHandlers(
            ILogger<PurposeCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<Purpose> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public async Task Handle(CreatePurpose message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUnique();

            var purpose = new Purpose(message.PurposeId, message.Name);
            Session.Add(purpose);
            await Session.Commit(message.User);
        }

        public async Task Handle(UpdatePurpose message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.PurposeId, message.Name))
                throw new NameNotUnique();

            var purpose = Session.Get<Purpose>(message.PurposeId);
            purpose.Update(message.Name);
            await Session.Commit(message.User);
        }
    }
}
