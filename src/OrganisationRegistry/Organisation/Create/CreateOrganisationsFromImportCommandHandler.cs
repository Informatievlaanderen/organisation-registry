namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Handling;
using Import;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class CreateOrganisationsFromImportCommandHandler :
    BaseCommandHandler<CreateOrganisationsFromImportCommandHandler>,
    ICommandEnvelopeHandler<CreateOrganisationsFromImport>
{
    private readonly IOvoNumberGenerator _ovoNumberGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateOrganisationsFromImportCommandHandler(
        ILogger<CreateOrganisationsFromImportCommandHandler> logger,
        IOvoNumberGenerator ovoNumberGenerator,
        IDateTimeProvider dateTimeProvider,
        ISession session
    ) : base(logger, session)
    {
        _ovoNumberGenerator = ovoNumberGenerator;
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<CreateOrganisationsFromImport> envelope)
    {
        return Handler.For(envelope.User, Session)
            .RequiresAdmin()
            .HandleWithCombinedTransaction(
                session => ImportOrganisations(envelope, session));
    }

    private void ImportOrganisations(ICommandEnvelope<CreateOrganisationsFromImport> envelope, ISession session)
    {
        var parentCache = new Dictionary<string, Organisation>();

        var sortedRecords = SortRecords(envelope.Command.Records.ToImmutableList());

        foreach (var record in sortedRecords)
        {
            var parent = GetParent(session, parentCache, record.ParentIdentifier);

            var ovoNumber = _ovoNumberGenerator.GenerateNextNumber();
            var validFrom = record.Validity_Start.HasValue
                ? new ValidFrom(record.Validity_Start.Value.ToDateTime(new TimeOnly()))
                : new ValidFrom();

            var operationalValidFrom = record.OperationalValidity_Start.HasValue
                ? new ValidFrom(record.OperationalValidity_Start.Value.ToDateTime(new TimeOnly()))
                : new ValidFrom();

            var organisation = Organisation.CreateFromImport(
                OrganisationId.New(),
                record.Name,
                ovoNumber,
                record.ShortName,
                record.Article ?? Article.None,
                parent,
                default,
                new Period(validFrom, new ValidTo()),
                new Period(operationalValidFrom, new ValidTo()),
                _dateTimeProvider,
                new OrganisationSourceId(envelope.Command.ImportFileId),
                record.Reference);

            parentCache.Add(record.Reference.ToLowerInvariant(), organisation);

            session.Add(organisation);
        }
    }

    private static IEnumerable<OutputRecord> SortRecords(ImmutableList<OutputRecord> records)
    {
        if (!records.Any(r => r.ParentIdentifier.Type == OrganisationParentIdentifier.IdentifierType.Id))
            throw new AtLeastOneOrganisationMustHaveAnExistingOrganisationAsParent();

        if (!records.Any(r => r.ParentIdentifier.Type == OrganisationParentIdentifier.IdentifierType.Reference))
            return records;

        var result = new List<OutputRecord>();

        var rootRecords = records.Where(r => r.ParentIdentifier.Type == OrganisationParentIdentifier.IdentifierType.Id).ToList();
        while (records.Any())
        {
            foreach (var record in rootRecords)
            {
                result.Add(record);
                records = records.Remove(record);
            }

            rootRecords = records
                .Where(r => rootRecords.Any(res => string.Equals(res.Reference, r.ParentIdentifier, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();
            if (!rootRecords.Any())
                break;
        }

        if (records.Any())
            throw new CircularDependencyOrFaultyReferenceDiscoveredBetweenOrganisations(records.Select(r => (r.SortOrder, r.ParentIdentifier.ToString())).ToImmutableArray());

        return result;
    }

    private static Organisation GetParent(ISession session, IReadOnlyDictionary<string, Organisation> parentCache, OrganisationParentIdentifier organisationParentIdentifier)
        => organisationParentIdentifier.Type == OrganisationParentIdentifier.IdentifierType.Id
            ? session.Get<Organisation>(organisationParentIdentifier)
            : parentCache[organisationParentIdentifier];
}
