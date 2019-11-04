namespace OrganisationRegistry.Purpose
{
    using Commands;
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

        public void Handle(CreatePurpose message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            var purpose = new Purpose(message.PurposeId, message.Name);
            Session.Add(purpose);
            Session.Commit();
        }

        public void Handle(UpdatePurpose message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.PurposeId, message.Name))
                throw new NameNotUniqueException();

            var purpose = Session.Get<Purpose>(message.PurposeId);
            purpose.Update(message.Name);
            Session.Commit();
        }
    }
}
