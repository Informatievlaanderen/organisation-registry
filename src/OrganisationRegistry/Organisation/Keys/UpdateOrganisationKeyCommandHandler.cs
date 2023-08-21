namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Handling.Authorization;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using KeyTypes;
using Microsoft.Extensions.Logging;

public class UpdateOrganisationKeyCommandHandler :
    BaseCommandHandler<UpdateOrganisationKeyCommandHandler>,
    ICommandEnvelopeHandler<UpdateOrganisationKey>
{
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;

    public UpdateOrganisationKeyCommandHandler(
        ILogger<UpdateOrganisationKeyCommandHandler> logger,
        ISession session,
        IOrganisationRegistryConfiguration organisationRegistryConfiguration) : base(logger, session)
    {
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationKey> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithKeyPolicy(_organisationRegistryConfiguration, envelope.Command)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);

                    var keyType = session.Get<KeyType>(envelope.Command.KeyTypeId);

                    organisation.UpdateKey(
                        envelope.Command.OrganisationKeyId,
                        keyType,
                        envelope.Command.Value,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)),
                        keyTypeId => new KeyPolicy(
                            organisation.State.OvoNumber,
                            _organisationRegistryConfiguration,
                            keyTypeId).Check(envelope.User).IsSuccessful);
                });
}
