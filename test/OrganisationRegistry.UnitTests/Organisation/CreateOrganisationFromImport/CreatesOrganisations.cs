namespace OrganisationRegistry.UnitTests.Organisation.CreateOrganisationFromImport;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Import;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
using Xunit;
using Xunit.Abstractions;

public class CreatesOrganisations
    : Specification<CreateOrganisationsFromImportCommandHandler, CreateOrganisationsFromImport>
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
        IEnumerable<OutputRecord> records)
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
        var fixture = new Fixture();
        var reference = fixture.Create<string>();

        var outputRecords = new List<OutputRecord>
        {
            new OutputRecordStub(parentOrganisationId: _parentOrganisationId, name: "name1", reference: reference)
        };

        var command = CreateOrganisationsFromImportCommand(outputRecords);
        await Given(Events).When(command, TestUser.AlgemeenBeheerder)
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
                operationalValidTo: null,
                sourceType: OrganisationSource.CsvImport,
                sourceId: command.ImportFileId,
                sourceOrganisationIdentifier: reference),
            opt =>
                opt.Excluding(e => e.OvoNumber)
                    .Excluding(e => e.OrganisationId)
                    .ExcludeEventProperties());
    }

    [Fact]
    public async Task PublishesOrganisationCreatedEventWithAllField()
    {
        var fixture = new Fixture();
        var reference = fixture.Create<string>();

        var shortName = "sn1";
        var outputRecords = new List<OutputRecord>()
        {
            new OutputRecordStub(parentOrganisationId: _parentOrganisationId, name: "name1", reference: reference)
            {
                Article = Article.De,
                ShortName = shortName,
                Validity_Start = DateOnly.FromDateTime(_tomorow),
                OperationalValidity_Start = DateOnly.FromDateTime(_tomorow)
            }
        };

        var command = CreateOrganisationsFromImportCommand(outputRecords);
        await Given(Events).When(command, TestUser.AlgemeenBeheerder)
            .Then();
        PublishedEvents[0].Body.Should().BeEquivalentTo(
            new OrganisationCreated(
                Guid.NewGuid(),
                "name1",
                "OVO000000",
                shortName: shortName,
                Article.De,
                description: string.Empty,
                new List<Purpose>(),
                showOnVlaamseOverheidSites: false,
                validFrom: _tomorow,
                validTo: null,
                operationalValidFrom: _tomorow,
                operationalValidTo: null,
                sourceType: OrganisationSource.CsvImport,
                sourceId: command.ImportFileId,
                sourceOrganisationIdentifier: reference),
            opt =>
                opt.Excluding(e => e.OvoNumber)
                    .Excluding(e => e.OrganisationId)
                    .ExcludeEventProperties());
    }

    [Fact]
    public async Task PublishesMultipleOrganisationCreatedEvents()
    {
        var fixture = new Fixture();
        var ref1 = fixture.Create<string>();
        var ref2 = fixture.Create<string>();
        var shortName2 = "sn2";

        var outputRecords = new List<OutputRecord>()
        {
            new OutputRecordStub(parentOrganisationId: _parentOrganisationId, name: "name1", reference: ref1),
            new OutputRecordStub(parentOrganisationId: _parentOrganisationId, name: "name2", reference: ref2)
            {
                Article = Article.Het,
                ShortName = shortName2,
                Validity_Start = DateOnly.FromDateTime(_tomorow),
            }
        };

        var command = CreateOrganisationsFromImportCommand(outputRecords);
        await Given(Events).When(command, TestUser.AlgemeenBeheerder)
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
                operationalValidTo: null,
                sourceType: OrganisationSource.CsvImport,
                sourceId: command.ImportFileId,
                sourceOrganisationIdentifier: ref1),
            opt =>
                opt.Excluding(e => e.OvoNumber)
                    .Excluding(e => e.OrganisationId)
                    .ExcludeEventProperties());
        publishedOrganisationCreatedEvents[1].Body.Should().BeEquivalentTo(
            new OrganisationCreated(
                Guid.NewGuid(),
                "name2",
                "OVO000000",
                shortName: shortName2,
                Article.Het,
                description: string.Empty,
                new List<Purpose>(),
                showOnVlaamseOverheidSites: false,
                validFrom: _tomorow,
                validTo: null,
                operationalValidFrom: null,
                operationalValidTo: null,
                sourceType: OrganisationSource.CsvImport,
                sourceId: command.ImportFileId,
                sourceOrganisationIdentifier: ref2),
            opt =>
                opt.Excluding(e => e.OvoNumber)
                    .Excluding(e => e.OrganisationId)
                    .ExcludeEventProperties());
    }
}
