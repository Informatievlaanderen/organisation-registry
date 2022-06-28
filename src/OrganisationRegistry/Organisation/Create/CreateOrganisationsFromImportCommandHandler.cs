namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Handling;
using Handling.Authorization;
using Import;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using LabelType;
using Microsoft.Extensions.Logging;

public class CreateOrganisationsFromImportCommandHandler :
    BaseCommandHandler<CreateOrganisationsFromImportCommandHandler>,
    ICommandEnvelopeHandler<CreateOrganisationsFromImport>
{
    private readonly IOvoNumberGenerator _ovoNumberGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;

    public CreateOrganisationsFromImportCommandHandler(
        ILogger<CreateOrganisationsFromImportCommandHandler> logger,
        IOvoNumberGenerator ovoNumberGenerator,
        IDateTimeProvider dateTimeProvider,
        ISession session,
        IOrganisationRegistryConfiguration organisationRegistryConfiguration
    ) : base(logger, session)
    {
        _ovoNumberGenerator = ovoNumberGenerator;
        _dateTimeProvider = dateTimeProvider;
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
    }

    public Task Handle(ICommandEnvelope<CreateOrganisationsFromImport> envelope)
        => Handler.For(envelope.User, Session)
            .WithImportPolicy(envelope.Command.Records.Where(r => r.ParentIdentifier.IsId).Select(r => (Guid)r.ParentIdentifier))
            .HandleWithCombinedTransaction(session => CreateOrganisations(envelope, session));

    private void CreateOrganisations(ICommandEnvelope<CreateOrganisationsFromImport> envelope, ISession session)
    {
        var parentCache = new Dictionary<string, Organisation>();
        var sortedRecords = SortRecords(envelope.Command.Records.ToImmutableList());
        var importFileId = envelope.Command.ImportFileId;

        var combinedException = new CreateOrganisationsImportException();

        foreach (var record in sortedRecords)
        {
            try
            {
                var organisation = CreateOrganisation(session, parentCache, importFileId, record);

                AddLabels(session, envelope.User, organisation, record.Labels);
            }
            catch (DomainException e)
            {
                combinedException.Add(e, record.Reference);
            }
        }

        if (combinedException.Exceptions.Any())
            throw combinedException;
    }

    private Organisation CreateOrganisation(
        ISession session,
        Dictionary<string, Organisation> parentCache,
        Guid importFileId,
        CreateOrganisationsFromImportCommandItem record)
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
            new OrganisationSourceId(importFileId),
            record.Reference);

        parentCache.Add(record.Reference.ToLowerInvariant(), organisation);

        session.Add(organisation);

        return organisation;
    }

    private void AddLabels(
        ISession session,
        IUser user,
        Organisation organisation,
        ImmutableList<Label> labelsToAdd)
    {
        var labelPolicy = LabelPolicy.ForCreate(
            organisation.State.OvoNumber,
            organisation.State.UnderVlimpersManagement,
            _organisationRegistryConfiguration,
            labelsToAdd.Select(l => l.LabelTypeId).ToArray());

        var result = labelPolicy.Check(user);

        if (result.Exception is { } exception)
            throw exception;

        foreach (var label in labelsToAdd)
        {
            organisation.AddLabel(
                _organisationRegistryConfiguration.Kbo,
                user,
                Guid.NewGuid(),
                session.Get<LabelType>(label.LabelTypeId),
                label.Value,
                Period.Infinity);
        }
    }

    private static IEnumerable<CreateOrganisationsFromImportCommandItem> SortRecords(ImmutableList<CreateOrganisationsFromImportCommandItem> records)
    {
        if (!records.Any(r => r.ParentIdentifier.IsId))
            throw new AtLeastOneOrganisationMustHaveAnExistingOrganisationAsParent();

        if (!records.Any(r => r.ParentIdentifier.IsReference))
            return records;

        var result = new List<CreateOrganisationsFromImportCommandItem>();

        var rootRecords = records.Where(r => r.ParentIdentifier.IsId).ToList();
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
        => organisationParentIdentifier.IsId
            ? session.Get<Organisation>(organisationParentIdentifier)
            : parentCache[organisationParentIdentifier];
}
