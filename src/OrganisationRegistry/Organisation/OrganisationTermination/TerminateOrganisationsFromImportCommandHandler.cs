namespace OrganisationRegistry.Organisation;

using System;
using System.Threading.Tasks;
using Handling;
using Import;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class TerminateOrganisationsFromImportCommandHandler :
    BaseCommandHandler<TerminateOrganisationsFromImportCommandHandler>,
    ICommandEnvelopeHandler<TerminateOrganisationsFromImport>
{
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TerminateOrganisationsFromImportCommandHandler(
        ILogger<TerminateOrganisationsFromImportCommandHandler> logger,
        ISession session,
        IOrganisationRegistryConfiguration organisationRegistryConfiguration,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<TerminateOrganisationsFromImport> envelope)
        => Handler.For(envelope.User, Session)
            .RequiresAdmin()
            .HandleWithCombinedTransaction(session => TerminateOrganisations(envelope, session));

    private void TerminateOrganisations(ICommandEnvelope<TerminateOrganisationsFromImport> envelope, ISession session)
    {
        foreach (var record in envelope.Command.Records)
        {
            TerminateOrganisation(session, record);
        }
    }

    private void TerminateOrganisation(ISession session, TerminateOrganisationsFromImportCommandItem record)
    {
        var organisation = session.Get<Organisation>(record.OrganisationId);
        var fieldsToTerminateConfig = OrganisationTerminationCalculator.GetFieldsToTerminate(_organisationRegistryConfiguration);

        organisation.TerminateOrganisation(
            record.Organisation_End.ToDateTime(new TimeOnly()),
            fieldsToTerminateConfig,
            _dateTimeProvider,
            false);
    }
}
