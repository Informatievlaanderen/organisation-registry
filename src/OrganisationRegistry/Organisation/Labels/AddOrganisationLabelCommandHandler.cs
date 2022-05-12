namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using LabelType;
using Microsoft.Extensions.Logging;

public class AddOrganisationLabelCommandHandler
    : BaseCommandHandler<AddOrganisationLabelCommandHandler>
        , ICommandEnvelopeHandler<AddOrganisationLabel>
{
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;
    private readonly ISecurityService _securityService;

    public AddOrganisationLabelCommandHandler(
        ILogger<AddOrganisationLabelCommandHandler> logger,
        ISession session,
        IOrganisationRegistryConfiguration organisationRegistryConfiguration,
        ISecurityService securityService) : base(logger, session)
    {
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
        _securityService = securityService;
    }

    public Task Handle(ICommandEnvelope<AddOrganisationLabel> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithLabelPolicy(_organisationRegistryConfiguration, envelope.Command)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var labelType = session.Get<LabelType>(envelope.Command.LabelTypeId);

                    KboV2Guards.ThrowIfFormalName(_organisationRegistryConfiguration, labelType);

                    organisation.AddLabel(
                        envelope.Command.OrganisationLabelId,
                        labelType,
                        envelope.Command.LabelValue,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
