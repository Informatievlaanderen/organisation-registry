namespace OrganisationRegistry.KeyTypes;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
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
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                        throw new NameNotUnique();

                    var keyType = new KeyType(envelope.Command.KeyTypeId, envelope.Command.Name);
                    session.Add(keyType);
                });

    public async Task Handle(ICommandEnvelope<UpdateKeyType> envelope)
        => await UpdateHandler<KeyType>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.KeyTypeId, envelope.Command.Name))
                        throw new NameNotUnique();

                    var keyType = session.Get<KeyType>(envelope.Command.KeyTypeId);
                    keyType.Update(envelope.Command.Name);
                });

    public async Task Handle(ICommandEnvelope<RemoveKeyType> envelope)
        => await UpdateHandler<KeyType>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    var keyType = session.Get<KeyType>(envelope.Command.KeyTypeId);
                    keyType.Remove();
                });
}
