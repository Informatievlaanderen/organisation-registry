namespace OrganisationRegistry.KeyTypes
{
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class KeyTypeCommandHandlers :
        BaseCommandHandler<KeyTypeCommandHandlers>,
        ICommandHandler<CreateKeyType>,
        ICommandHandler<UpdateKeyType>
    {
        private readonly IUniqueNameValidator<KeyType> _uniqueNameValidator;

        public KeyTypeCommandHandlers(
            ILogger<KeyTypeCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<KeyType> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public void Handle(CreateKeyType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            var keyType = new KeyType(message.KeyTypeId, message.Name);
            Session.Add(keyType);
            Session.Commit();
        }

        public void Handle(UpdateKeyType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.KeyTypeId, message.Name))
                throw new NameNotUniqueException();

            var keyType = Session.Get<KeyType>(message.KeyTypeId);
            keyType.Update(message.Name);
            Session.Commit();
        }
    }
}
