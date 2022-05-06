namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using KeyTypes;
using Microsoft.Extensions.Logging;

public class AddOrganisationKeyCommandHandler :
    BaseCommandHandler<AddOrganisationKeyCommandHandler>,
    ICommandEnvelopeHandler<AddOrganisationKey>
{
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;
    private readonly ISecurityService _securityService;

    public AddOrganisationKeyCommandHandler(
        ILogger<AddOrganisationKeyCommandHandler> logger,
        ISession session,
        IOrganisationRegistryConfiguration organisationRegistryConfiguration,
        ISecurityService securityService) : base(logger, session)
    {
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
        _securityService = securityService;
    }

    public Task Handle(ICommandEnvelope<AddOrganisationKey> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithKeyPolicy(_organisationRegistryConfiguration, envelope.Command)
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
                        keyTypeId => _securityService.CanUseKeyType(envelope.User, keyTypeId));
                });
}
