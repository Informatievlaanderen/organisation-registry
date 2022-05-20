namespace OrganisationRegistry.KeyTypes
{
    using System.Threading.Tasks;
    using Commands;
    using Exceptions;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class KeyTypeCommandHandlers :
        BaseCommandHandler<KeyTypeCommandHandlers>,
        ICommandEnvelopeHandler<CreateKeyType>,
        ICommandEnvelopeHandler<UpdateKeyType>,
        ICommandEnvelopeHandler<RemoveKeyType>
    {
        private readonly IUniqueNameValidator<KeyType> _uniqueNameValidator;

        public KeyTypeCommandHandlers(
            ILogger<KeyTypeCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<KeyType> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public async Task Handle(ICommandEnvelope<CreateKeyType> envelope)
        {
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                throw new NameNotUnique();

            var keyType = new KeyType(envelope.Command.KeyTypeId, envelope.Command.Name);
            Session.Add(keyType);
            await Session.Commit(envelope.User);
        }

        public async Task Handle(ICommandEnvelope<UpdateKeyType> envelope)
        {
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.KeyTypeId, envelope.Command.Name))
                throw new NameNotUnique();

            var keyType = Session.Get<KeyType>(envelope.Command.KeyTypeId);
            keyType.Update(envelope.Command.Name);
            await Session.Commit(envelope.User);
        }

        public async Task Handle(ICommandEnvelope<RemoveKeyType> envelope)
        {
            var keyType = Session.Get<KeyType>(envelope.Command.KeyTypeId);
            keyType.Remove();
            await Session.Commit(envelope.User);
        }
    }
}
