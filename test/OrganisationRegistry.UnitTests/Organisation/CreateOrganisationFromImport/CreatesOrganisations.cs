﻿namespace OrganisationRegistry.UnitTests.Organisation.CreateOrganisationFromImport;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using LabelType.Events;
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
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class CreatesOrganisations
    : Specification<CreateOrganisationsFromImportCommandHandler, CreateOrganisationsFromImport>
{
    private readonly Guid _parentOrganisationId;
    private readonly DateTime _tomorow;
    private static readonly SequentialOvoNumberGenerator SequentialOvoNumberGenerator = new();
    private readonly Fixture _fixture = new();
    private readonly Guid _label1TypeId;
    private readonly Guid _label2TypeId;
    private readonly string _label1TypeName;
    private readonly string _label2TypeName;

    public CreatesOrganisations(ITestOutputHelper helper) : base(helper)
    {
        _parentOrganisationId = Guid.NewGuid();
        _tomorow = DateTime.Today.AddDays(1);
        _label1TypeId = _fixture.Create<Guid>();
        _label2TypeId = _fixture.Create<Guid>();
        _label1TypeName = _fixture.Create<string>();
        _label2TypeName = _fixture.Create<string>();
    }

    private IEvent[] Events
        => new IEvent[]
        {
            new LabelTypeCreated(_label1TypeId, _label1TypeName),
            new LabelTypeCreated(_label2TypeId, _label2TypeName),
            new OrganisationCreatedBuilder(SequentialOvoNumberGenerator)
                .WithId(new OrganisationId(_parentOrganisationId))
                .Build(),
        };

    private static CreateOrganisationsFromImport CreateOrganisationsFromImportCommand(IEnumerable<CreateOrganisationsFromImportCommandItem> records)
        => new(Guid.NewGuid(), records);

    protected override CreateOrganisationsFromImportCommandHandler BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<CreateOrganisationsFromImportCommandHandler>>(),
            SequentialOvoNumberGenerator,
            new DateTimeProviderStub(DateTime.Now),
            session,
            new OrganisationRegistryConfigurationStub());

    [Fact]
    public async Task PublishesOrganisationCreatedEventWithMinimumFields()
    {
        var outputRecord = new CreateOrganisationsFromImportCommandItemBuilder(_fixture.Create<string>(), _parentOrganisationId, _fixture.Create<string>(), 1)
            .Build();

        var command = CreateOrganisationsFromImportCommand(new List<CreateOrganisationsFromImportCommandItem> { outputRecord });

        await Given(Events).When(command, TestUser.AlgemeenBeheerder)
            .Then();
        PublishedEvents[0].Body.Should().BeEquivalentTo(
            new OrganisationCreatedBuilder(SequentialOvoNumberGenerator)
                .WithName(outputRecord.Name)
                .WithSource(OrganisationSource.CsvImport, command.ImportFileId, outputRecord.Reference)
                .Build(),
            opt =>
                opt.Excluding(e => e.OvoNumber)
                    .Excluding(e => e.OrganisationId)
                    .ExcludeEventProperties());
    }

    [Fact]
    public async Task PublishesOrganisationCreatedEventWithAllField()
    {
        var outputRecord = new CreateOrganisationsFromImportCommandItemBuilder(_fixture.Create<string>(), _parentOrganisationId, _fixture.Create<string>(), 1)
            .WithShortName(_fixture.Create<string>())
            .WithArticle("de")
            .WithValidityStart(_tomorow.ToString("yyyy-MM-dd"))
            .WithOperationalValidityStart(_tomorow.ToString("yyyy-MM-dd"))
            .Build();

        var command = CreateOrganisationsFromImportCommand(new List<CreateOrganisationsFromImportCommandItem> { outputRecord });

        await Given(Events).When(command, TestUser.AlgemeenBeheerder)
            .Then();
        PublishedEvents[0].Body.Should().BeEquivalentTo(
            new OrganisationCreatedBuilder(SequentialOvoNumberGenerator)
                .WithName(outputRecord.Name)
                .WithShortName(outputRecord.ShortName)
                .WithValidity(outputRecord.Validity_Start?.ToDateTime(TimeOnly.MinValue), null)
                .WithOperationalValidity(outputRecord.OperationalValidity_Start?.ToDateTime(TimeOnly.MinValue), null)
                .WithArticle(outputRecord.Article)
                .WithSource(OrganisationSource.CsvImport, command.ImportFileId, outputRecord.Reference)
                .Build(),
            opt =>
                opt.Excluding(e => e.OvoNumber)
                    .Excluding(e => e.OrganisationId)
                    .ExcludeEventProperties());
    }

    [Fact]
    public async Task PublishesMultipleOrganisationCreatedEvents()
    {
        var ref1 = _fixture.Create<string>();
        var ref2 = _fixture.Create<string>();
        var name1 = _fixture.Create<string>();
        var name2 = _fixture.Create<string>();
        var shortName2 = _fixture.Create<string>();
        var article2 = Article.Het;

        var outputRecords = new List<CreateOrganisationsFromImportCommandItem>()
        {
            new CreateOrganisationsFromImportCommandItemBuilder(ref1, _parentOrganisationId, name1, 1),
            new CreateOrganisationsFromImportCommandItemBuilder(ref2, _parentOrganisationId, name2, 2)
                .WithArticle(article2)
                .WithShortName(shortName2)
                .WithValidityStart(_tomorow.ToString("yyyy-MM-dd")),
        };

        var command = CreateOrganisationsFromImportCommand(outputRecords);
        await Given(Events).When(command, TestUser.AlgemeenBeheerder)
            .Then();
        var publishedOrganisationCreatedEvents = PublishedEvents.Where(e => e.Body is OrganisationCreated).ToArray();
        publishedOrganisationCreatedEvents[0].Body.Should().BeEquivalentTo(
            new OrganisationCreatedBuilder(SequentialOvoNumberGenerator)
                .WithName(name1)
                .WithSource(OrganisationSource.CsvImport, command.ImportFileId, ref1)
                .Build(),
            opt =>
                opt.Excluding(e => e.OvoNumber)
                    .Excluding(e => e.OrganisationId)
                    .ExcludeEventProperties());
        publishedOrganisationCreatedEvents[1].Body.Should().BeEquivalentTo(
            new OrganisationCreatedBuilder(SequentialOvoNumberGenerator)
                .WithName(name2)
                .WithArticle(Article.Het)
                .WithValidity(_tomorow, null)
                .WithShortName(shortName2)
                .WithSource(OrganisationSource.CsvImport, command.ImportFileId, ref2)
                .Build(),
            opt =>
                opt.Excluding(e => e.OvoNumber)
                    .Excluding(e => e.OrganisationId)
                    .ExcludeEventProperties());
    }

    [Fact]
    public async Task Publishes2OrganisationLabelAddedEvents()
    {
        var label1Value = _fixture.Create<string>();
        var label2Value = _fixture.Create<string>();

        var outputRecords = new List<CreateOrganisationsFromImportCommandItem>()
        {
            new CreateOrganisationsFromImportCommandItemBuilder(_fixture.Create<string>(), _parentOrganisationId, _fixture.Create<string>(), 1)
                .AddLabel(_label1TypeId, _label1TypeName, label1Value)
                .AddLabel(_label2TypeId, _label2TypeName, label2Value),
        };

        var command = CreateOrganisationsFromImportCommand(outputRecords);
        await Given(Events).When(command, TestUser.AlgemeenBeheerder)
            .Then();

        var organisationId = PublishedEvents
            .Single(x => x.Body is OrganisationCreated)
            .Body.As<OrganisationCreated>()
            .OrganisationId;

        var publishedOrganisationLabelAddedEvents = PublishedEvents.Where(e => e.Body is OrganisationLabelAdded).ToArray();
        publishedOrganisationLabelAddedEvents[0].Body.Should().BeEquivalentTo(
            new OrganisationLabelAddedBuilder()
                .WithLabelTypeId(_label1TypeId)
                .WithLabelTypeName(_label1TypeName)
                .WithValue(label1Value)
                .WithOrganisationId(organisationId)
                .Build(),
            opt =>
                opt.Excluding(e => e.OrganisationLabelId)
                    .ExcludeEventProperties());
        publishedOrganisationLabelAddedEvents[1].Body.Should().BeEquivalentTo(
            new OrganisationLabelAddedBuilder()
                .WithLabelTypeId(_label2TypeId)
                .WithLabelTypeName(_label2TypeName)
                .WithValue(label2Value)
                .WithOrganisationId(organisationId)
                .Build(),
            opt =>
                opt.Excluding(e => e.OrganisationLabelId)
                    .ExcludeEventProperties());
    }

    [Fact]
    public async Task WithOrderedInput_CreatesATree()
    {
        var ref1 = _fixture.Create<string>();
        var ref2=_fixture.Create<string>();
        var ref3=_fixture.Create<string>();
        var ref4=_fixture.Create<string>();
        var name1=_fixture.Create<string>();
        var name2=_fixture.Create<string>();
        var name3=_fixture.Create<string>();
        var name4=_fixture.Create<string>();
        var outputRecords = new List<CreateOrganisationsFromImportCommandItem>()
        {
            new CreateOrganisationsFromImportCommandItemBuilder(ref1, _parentOrganisationId, name1, 1),
            new CreateOrganisationsFromImportCommandItemBuilder(ref2, ref1, name2, 2),
            new CreateOrganisationsFromImportCommandItemBuilder(ref3, ref1, name3, 3),
            new CreateOrganisationsFromImportCommandItemBuilder(ref4, ref2, name4, 4),
        };
        var command = CreateOrganisationsFromImportCommand(outputRecords);
        await Given(Events).When(command, TestUser.AlgemeenBeheerder)
            .Then();
        var organisationIdDictionary = PublishedEvents
            .Where(e => e.Body is OrganisationCreated)
            .Select(e => (OrganisationCreated)e.Body)
            .ToDictionary(e => e.SourceOrganisationIdentifier!, e => e.OrganisationId);

        var parentAddedEvents = PublishedEvents
            .Where(e => e.Body is OrganisationParentAdded)
            .Select(e => (OrganisationParentAdded)e.Body)
            .ToArray();

        parentAddedEvents[0].OrganisationId.Should().Be(organisationIdDictionary[ref1]);
        parentAddedEvents[0].ParentOrganisationId.Should().Be(_parentOrganisationId);

        parentAddedEvents[1].OrganisationId.Should().Be(organisationIdDictionary[ref2]);
        parentAddedEvents[1].ParentOrganisationId.Should().Be(organisationIdDictionary[ref1]);

        parentAddedEvents[2].OrganisationId.Should().Be(organisationIdDictionary[ref3]);
        parentAddedEvents[2].ParentOrganisationId.Should().Be(organisationIdDictionary[ref1]);

        parentAddedEvents[3].OrganisationId.Should().Be(organisationIdDictionary[ref4]);
        parentAddedEvents[3].ParentOrganisationId.Should().Be(organisationIdDictionary[ref2]);
    }

    [Fact]
    public async Task WithInputInRandomOrder_CreatesATree()
    {
        var ref1 = _fixture.Create<string>();
        var ref2=_fixture.Create<string>();
        var ref3=_fixture.Create<string>();
        var name1=_fixture.Create<string>();
        var name2=_fixture.Create<string>();
        var name3=_fixture.Create<string>();
        var outputRecords = new List<CreateOrganisationsFromImportCommandItem>()
        {
            new CreateOrganisationsFromImportCommandItemBuilder(ref3, ref2, name3, 4),
            new CreateOrganisationsFromImportCommandItemBuilder(ref2, ref1, name2, 3),
            new CreateOrganisationsFromImportCommandItemBuilder(ref1, _parentOrganisationId, name1, 1),
        };
        var command = CreateOrganisationsFromImportCommand(outputRecords);
        await Given(Events).When(command, TestUser.AlgemeenBeheerder)
            .Then();
        var organisationIdDictionary = PublishedEvents
            .Where(e => e.Body is OrganisationCreated)
            .Select(e => (OrganisationCreated)e.Body)
            .ToDictionary(e => e.SourceOrganisationIdentifier!, e => e.OrganisationId);

        var parentAddedEvents = PublishedEvents
            .Where(e => e.Body is OrganisationParentAdded)
            .Select(e => (OrganisationParentAdded)e.Body)
            .ToArray();

        parentAddedEvents[0].OrganisationId.Should().Be(organisationIdDictionary[ref1]);
        parentAddedEvents[0].ParentOrganisationId.Should().Be(_parentOrganisationId);

        parentAddedEvents[1].OrganisationId.Should().Be(organisationIdDictionary[ref2]);
        parentAddedEvents[1].ParentOrganisationId.Should().Be(organisationIdDictionary[ref1]);

        parentAddedEvents[2].OrganisationId.Should().Be(organisationIdDictionary[ref3]);
        parentAddedEvents[2].ParentOrganisationId.Should().Be(organisationIdDictionary[ref2]);
    }

    [Fact]
    public async Task WithCircularDependencies_ThrowsAnException()
    {
        var ref1 = _fixture.Create<string>();
        var ref2=_fixture.Create<string>();
        var ref3=_fixture.Create<string>();
        var name1=_fixture.Create<string>();
        var name2=_fixture.Create<string>();
        var name3=_fixture.Create<string>();
        var outputRecords = new List<CreateOrganisationsFromImportCommandItem>()
        {
            new CreateOrganisationsFromImportCommandItemBuilder(ref3, ref2, name3, 4),
            new CreateOrganisationsFromImportCommandItemBuilder(ref2, ref3, name2, 3),
            new CreateOrganisationsFromImportCommandItemBuilder(ref1, _parentOrganisationId, name1, 1),
        };
        var command = CreateOrganisationsFromImportCommand(outputRecords);
        await Given(Events).When(command, TestUser.AlgemeenBeheerder)
            .ThenThrows<CircularDependencyOrFaultyReferenceDiscoveredBetweenOrganisations>();
    }
}
