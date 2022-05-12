namespace OrganisationRegistry.Organisation;

using System;
using System.Linq;
using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class TerminateOrganisationCommandHandler
    : BaseCommandHandler<TerminateOrganisationCommandHandler>,
        ICommandEnvelopeHandler<TerminateOrganisation>
{
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TerminateOrganisationCommandHandler(
        ILogger<TerminateOrganisationCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider,
        IOrganisationRegistryConfiguration organisationRegistryConfiguration) : base(logger, session)
    {
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<TerminateOrganisation> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithVlimpersOnlyPolicy()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);

                    var fieldsToTerminateConfig = new OrganisationTerminationCalculator.FieldsToTerminateConfig(
                        _organisationRegistryConfiguration.Kbo.FormalFrameworkIdsToTerminateEndOfNextYear
                            ?.FirstOrDefault() ?? Guid.Empty,
                        _organisationRegistryConfiguration.Kbo.OrganisationCapacityIdsToTerminateEndOfNextYear
                            ?.FirstOrDefault() ?? Guid.Empty,
                        _organisationRegistryConfiguration.Kbo.OrganisationClassificationTypeIdsToTerminateEndOfNextYear
                            ?.FirstOrDefault() ?? Guid.Empty,
                        _organisationRegistryConfiguration.VlimpersKeyTypeId);

                    organisation.TerminateOrganisation(
                        envelope.Command.DateOfTermination,
                        fieldsToTerminateConfig,
                        _dateTimeProvider,
                        envelope.Command.ForceKboTermination);
                });
}
