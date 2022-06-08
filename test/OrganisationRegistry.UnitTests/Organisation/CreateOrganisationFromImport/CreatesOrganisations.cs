namespace OrganisationRegistry.UnitTests.Organisation.CreateOrganisationFromImport;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
using Xunit;
using Xunit.Abstractions;

public class
    CreatesOrganisations : Specification<CreateOrganisationsFromImportCommandHandler, CreateOrganisationsFromImport>
{
    private readonly Guid _parentOrganisationId;
    private readonly DateTime _tomorow;

    public CreatesOrganisations(ITestOutputHelper helper) : base(helper)
    {
        _parentOrganisationId = Guid.NewGuid();
        _tomorow = DateTime.Today.AddDays(1);
    }

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator())
                .WithId(new OrganisationId(_parentOrganisationId))
                .Build()
        };

    private static CreateOrganisationsFromImport CreateOrganisationsFromImportCommand(
        IEnumerable<IOutputRecord> records)
        => new(Guid.NewGuid(), records);

    protected override CreateOrganisationsFromImportCommandHandler BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<CreateOrganisationsFromImportCommandHandler>>(),
            new SequentialOvoNumberGenerator(),
            new DateTimeProviderStub(DateTime.Now),
            session);

    [Fact]
    public async Task PublishesOrganisationCreatedEventWithMinimumFields()
    {
        var outputRecords = new List<IOutputRecord>()
        {
            new OutputRecordStub
            {
                ParentOrganisationId = _parentOrganisationId,
                Name = "name1",
            }
        };

        await Given(Events).When(CreateOrganisationsFromImportCommand(outputRecords), TestUser.AlgemeenBeheerder)
            .Then();
        PublishedEvents[0].Body.Should().BeEquivalentTo(
            new OrganisationCreated(
                Guid.NewGuid(),
                "name1",
                "OVO000000",
                shortName: null,
                Article.None,
                description: string.Empty,
                new List<Purpose>(),
                showOnVlaamseOverheidSites: false,
                validFrom: null,
                validTo: null,
                operationalValidFrom: null,
                operationalValidTo: null),
            opt =>
                opt.Excluding(e => e.OvoNumber)
                    .Excluding(e => e.OrganisationId)
                    .ExcludeEventProperties());
    }

    [Fact]
    public async Task PublishesOrganisationCreatedEventWithAllField()
    {

        var outputRecords = new List<IOutputRecord>()
        {
            new OutputRecordStub
            {
                ParentOrganisationId = _parentOrganisationId,
                Name = "name1",
                Article = Article.De,
                ShortName = "sn1",
                Validity_Start = DateOnly.FromDateTime(_tomorow),
                OperationalValidity_Start = DateOnly.FromDateTime(_tomorow)
            }
        };

        await Given(Events).When(CreateOrganisationsFromImportCommand(outputRecords), TestUser.AlgemeenBeheerder)
            .Then();
        PublishedEvents[0].Body.Should().BeEquivalentTo(
            new OrganisationCreated(
                Guid.NewGuid(),
                "name1",
                "OVO000000",
                shortName: "sn1",
                Article.De,
                description: string.Empty,
                new List<Purpose>(),
                showOnVlaamseOverheidSites: false,
                validFrom: _tomorow,
                validTo: null,
                operationalValidFrom: _tomorow,
                operationalValidTo: null),
            opt =>
                opt.Excluding(e => e.OvoNumber)
                    .Excluding(e => e.OrganisationId)
                    .ExcludeEventProperties());
    }

    [Fact]
    public async Task PublishesMultipleOrganisationCreatedEvents()
    {

        var outputRecords = new List<IOutputRecord>()
        {
            new OutputRecordStub
            {
                ParentOrganisationId = _parentOrganisationId,
                Name = "name1",
            },
            new OutputRecordStub
            {
                ParentOrganisationId = _parentOrganisationId,
                Name = "name2",
                Article = Article.Het,
                ShortName = "sn2",
                Validity_Start = DateOnly.FromDateTime(_tomorow),
            }
        };

        await Given(Events).When(CreateOrganisationsFromImportCommand(outputRecords), TestUser.AlgemeenBeheerder)
            .Then();
        var publishedOrganisationCreatedEvents = PublishedEvents.Where(e => e.Body is OrganisationCreated).ToArray();
        publishedOrganisationCreatedEvents[0].Body.Should().BeEquivalentTo(
            new OrganisationCreated(
                Guid.NewGuid(),
                "name1",
                "OVO000000",
                shortName: null,
                Article.None,
                description: string.Empty,
                new List<Purpose>(),
                showOnVlaamseOverheidSites: false,
                validFrom: null,
                validTo: null,
                operationalValidFrom: null,
                operationalValidTo: null),
            opt =>
                opt.Excluding(e => e.OvoNumber)
                    .Excluding(e => e.OrganisationId)
                    .ExcludeEventProperties());
        publishedOrganisationCreatedEvents[1].Body.Should().BeEquivalentTo(
            new OrganisationCreated(
                Guid.NewGuid(),
                "name2",
                "OVO000000",
                shortName: "sn2",
                Article.Het,
                description: string.Empty,
                new List<Purpose>(),
                showOnVlaamseOverheidSites: false,
                validFrom: _tomorow,
                validTo: null,
                operationalValidFrom: null,
                operationalValidTo: null),
            opt =>
                opt.Excluding(e => e.OvoNumber)
                    .Excluding(e => e.OrganisationId)
                    .ExcludeEventProperties());
    }
}
