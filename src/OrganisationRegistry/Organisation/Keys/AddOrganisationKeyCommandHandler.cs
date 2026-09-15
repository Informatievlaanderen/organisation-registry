namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Handling.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using KeyTypes;
using Microsoft.Extensions.Logging;

public class AddOrganisationKeyCommandHandler :
    BaseCommandHandler<AddOrganisationKeyCommandHandler>,
    ICommandEnvelopeHandler<AddOrganisationKey>
{
    public AddOrganisationKeyCommandHandler(
        ILogger<AddOrganisationKeyCommandHandler> logger,
        ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<AddOrganisationKey> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithKeyPolicy(envelope.Command)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);

                    var keyType = session.Get<KeyType>(envelope.Command.KeyTypeId);

                    organisation.AddKey(
                        envelope.Command.OrganisationKeyId,
                        keyType,
                        envelope.Command.KeyValue,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)),
                        keyTypeId => new KeyPolicy(
                            organisation.State.UnderVlimpersManagement,
                            keyTypeId).Check(envelope.User).IsSuccessful);
                });
}
