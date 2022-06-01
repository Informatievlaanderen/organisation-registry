namespace OrganisationRegistry.ElasticSearch.Tests;

using FluentAssertions;
using Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Projections.Infrastructure;
using Scenario;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Organisation.Events;
using OrganisationRegistry.Tests.Shared.Stubs;
using Organisations;
using Osc;
using Projections.Organisations;
using Scenario.Specimen;
using SqlServer.Infrastructure;

[Collection(nameof(ElasticSearchFixture))]
public class OrganisationHandlerTests
{
    private readonly ElasticSearchFixture _fixture;
    private readonly TestEventProcessor _eventProcessor;

    public OrganisationHandlerTests(ElasticSearchFixture fixture)
    {
        _fixture = fixture;

        var dbContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
            .UseInMemoryDatabase($"org-es-test-{Guid.NewGuid()}", _ => { }).Options;

        var logger = _fixture.LoggerFactory.CreateLogger<Organisation>();
        var elastic = _fixture.Elastic;
        var elasticSearchOptions = _fixture.ElasticSearchOptions;
        var organisationManagementConfiguration = new OrganisationManagementConfigurationStub();

        var organisationHandler = new Organisation(logger, elastic, elasticSearchOptions, organisationManagementConfiguration);

        var testContextFactory = new TestContextFactory(dbContextOptions);
        var organisationBankAccountHandler = new OrganisationBankAccount(new NullLogger<OrganisationBankAccount>());
        var organisationBody = new OrganisationBody(new NullLogger<OrganisationBody>(), testContextFactory);
        var organisationBuilding = new OrganisationBuilding(new NullLogger<OrganisationBuilding>());
        var organisationCapacity = new OrganisationCapacity(new NullLogger<OrganisationCapacity>(), testContextFactory);
        var organisationContact = new OrganisationContact(new NullLogger<OrganisationContact>());
        var organisationFormalFramework = new OrganisationFormalFramework(new NullLogger<OrganisationFormalFramework>());
        var organisationFunction = new OrganisationFunction(new NullLogger<OrganisationFunction>(), testContextFactory);
        var organisationKey = new OrganisationKey(new NullLogger<OrganisationKey>());
        var organisationLabel = new OrganisationLabel(new NullLogger<OrganisationLabel>());
        var organisationLocation = new OrganisationLocation(new NullLogger<OrganisationLocation>());
        var organisationOpeningHoursSpecification = new OrganisationOpeningHoursSpecification(new NullLogger<OrganisationOpeningHoursSpecification>());
        var organisationOrganisationClassification = new OrganisationOrganisationClassification(new NullLogger<OrganisationOrganisationClassification>());
        var organisationParent = new OrganisationParent(new NullLogger<OrganisationParent>());
        var organisationRegulation = new OrganisationRegulation(new NullLogger<OrganisationRegulation>());
        var organisationRelation = new OrganisationRelation(new NullLogger<OrganisationRelation>(), testContextFactory);

        var serviceProvider = new ServiceCollection()
            .AddSingleton(organisationHandler)
            .AddSingleton(organisationBankAccountHandler)
            .AddSingleton(organisationBody)
            .AddSingleton(organisationBuilding)
            .AddSingleton(organisationCapacity)
            .AddSingleton(organisationContact)
            .AddSingleton(organisationFormalFramework)
            .AddSingleton(organisationFunction)
            .AddSingleton(organisationKey)
            .AddSingleton(organisationLabel)
            .AddSingleton(organisationLocation)
            .AddSingleton(organisationOpeningHoursSpecification)
            .AddSingleton(organisationOrganisationClassification)
            .AddSingleton(organisationParent)
            .AddSingleton(organisationRegulation)
            .AddSingleton(organisationRelation)
            .AddSingleton(new MemoryCachesMaintainer(new MemoryCaches(testContextFactory), testContextFactory))
            .BuildServiceProvider();

        var bus = new ElasticBus(new NullLogger<ElasticBus>());
        _eventProcessor = new TestEventProcessor(bus, fixture);

        var registrar = new ElasticBusRegistrar(new NullLogger<ElasticBusRegistrar>(), bus, () => serviceProvider);
        registrar.RegisterEventHandlers(OrganisationsRunner.EventHandlers);
    }

    [Fact]
    public async void InitializeProjection_CreatesIndex()
    {
        var scenario = new OrganisationScenario(Guid.NewGuid());

        await _eventProcessor.Handle<OrganisationDocument>(
            new List<IEnvelope>
            {
                scenario.Create<InitialiseProjection>().ToEnvelope()
            }
        );

        var indices = (await _fixture.Elastic.ReadClient.Indices.GetAsync(_fixture.ElasticSearchOptions.Value.OrganisationsReadIndex)).Indices;
        indices.Should().NotBeEmpty();
    }

    [Fact]
    public async void OrganisationCreated_CreatesDocument()
    {
        var scenario = new OrganisationScenario(Guid.NewGuid());

        var initialiseProjection = scenario.Create<InitialiseProjection>();
        var organisationCreated = scenario.Create<OrganisationCreated>();

        await _eventProcessor.Handle<OrganisationDocument>(
            new List<IEnvelope>()
            {
                initialiseProjection.ToEnvelope(),
                organisationCreated.ToEnvelope(),
            }
        );

        var organisation = _fixture.Elastic.ReadClient.Get<OrganisationDocument>(organisationCreated.OrganisationId);

        organisation.Source.Name.Should().Be(organisationCreated.Name);
        organisation.Source.ShortName.Should().Be(organisationCreated.ShortName);
        organisation.Source.Description.Should().Be(organisationCreated.Description);
    }

    [Fact]
    public async void OrganisationCreated_CreatesParent()
    {
        var scenario = new ScenarioBase<Organisation>();

        var initialiseProjection = scenario.Create<InitialiseProjection>();
        var organisationCreated = scenario.Create<OrganisationCreated>();
        var organisationParentCreated = scenario.Create<OrganisationCreated>();
        var parentCoupled = new OrganisationParentAdded(
            organisationCreated.OrganisationId,
            Guid.NewGuid(),
            organisationParentCreated.OrganisationId,
            organisationParentCreated.Name,
            null,
            null);
        var parentNameUpdated = new OrganisationNameUpdated(
            organisationParentCreated.OrganisationId,
            "new name koen 123");

        await _eventProcessor.Handle<OrganisationDocument>(
            new List<IEnvelope>()
            {
                initialiseProjection.ToEnvelope(),
                organisationCreated.ToEnvelope(),
                organisationParentCreated.ToEnvelope(),
                parentCoupled.ToEnvelope(),
                parentNameUpdated.ToEnvelope()
            }
        );

        var organisation = _fixture.Elastic.ReadClient.Get<OrganisationDocument>(organisationCreated.OrganisationId);

        organisation.Source.Name.Should().Be(organisationCreated.Name);
        organisation.Source.ShortName.Should().Be(organisationCreated.ShortName);
        organisation.Source.Description.Should().Be(organisationCreated.Description);
        organisation.Source.Parents.First().ParentOrganisationId.Should()
            .Be(organisationParentCreated.OrganisationId);
        organisation.Source.Parents.First().ParentOrganisationName.Should()
            .Be(parentNameUpdated.Name);
    }

    [Fact]
    public async void OrganisationKboBankAccountAdded_AddsBankAccount()
    {
        var scenario = new OrganisationScenario(Guid.NewGuid());

        var initialiseProjection = scenario.Create<InitialiseProjection>();
        var organisationCreated = scenario.Create<OrganisationCreated>();
        var kboOrganisationBankAccountAdded = scenario.Create<KboOrganisationBankAccountAdded>();
        var kboOrganisationBankAccountAdded2 = scenario.Create<KboOrganisationBankAccountAdded>();

        await _eventProcessor.Handle<OrganisationDocument>(
            new List<IEnvelope>
            {
                initialiseProjection.ToEnvelope(),
                organisationCreated.ToEnvelope(),
                kboOrganisationBankAccountAdded.ToEnvelope(),
                kboOrganisationBankAccountAdded2.ToEnvelope()
            }
        );

        var organisation = _fixture.Elastic.ReadClient.Get<OrganisationDocument>(kboOrganisationBankAccountAdded.OrganisationId);

        organisation.Source.BankAccounts.Should().BeEquivalentTo(
            new List<OrganisationDocument.OrganisationBankAccount>
            {
                new(
                    kboOrganisationBankAccountAdded.OrganisationBankAccountId,
                    kboOrganisationBankAccountAdded.BankAccountNumber,
                    kboOrganisationBankAccountAdded.IsIban,
                    kboOrganisationBankAccountAdded.Bic,
                    kboOrganisationBankAccountAdded.IsBic,
                    Common.Period.FromDates(kboOrganisationBankAccountAdded.ValidFrom, kboOrganisationBankAccountAdded.ValidTo)),
                new(
                    kboOrganisationBankAccountAdded2.OrganisationBankAccountId,
                    kboOrganisationBankAccountAdded2.BankAccountNumber,
                    kboOrganisationBankAccountAdded2.IsIban,
                    kboOrganisationBankAccountAdded2.Bic,
                    kboOrganisationBankAccountAdded2.IsBic,
                    Common.Period.FromDates(kboOrganisationBankAccountAdded2.ValidFrom, kboOrganisationBankAccountAdded2.ValidTo))
            });
    }

    [Fact]
    public async void OrganisationKboBankAccountRemoved_RemovesBankAccount()
    {
        var scenario = new OrganisationScenario(Guid.NewGuid());

        var initialiseProjection = scenario.Create<InitialiseProjection>();
        var organisationCreated = scenario.Create<OrganisationCreated>();
        var kboOrganisationBankAccountAdded = scenario.Create<KboOrganisationBankAccountAdded>();
        var kboOrganisationBankAccountToRemoveAdded = scenario.Create<KboOrganisationBankAccountAdded>();
        var kboOrganisationBankAccountRemoved = new KboOrganisationBankAccountRemoved(
            kboOrganisationBankAccountToRemoveAdded.OrganisationId,
            kboOrganisationBankAccountToRemoveAdded.OrganisationBankAccountId,
            kboOrganisationBankAccountToRemoveAdded.BankAccountNumber,
            kboOrganisationBankAccountToRemoveAdded.IsIban,
            kboOrganisationBankAccountToRemoveAdded.Bic,
            kboOrganisationBankAccountToRemoveAdded.IsBic,
            kboOrganisationBankAccountToRemoveAdded.ValidFrom,
            kboOrganisationBankAccountToRemoveAdded.ValidTo);

        await _eventProcessor.Handle<OrganisationDocument>(
            new List<IEnvelope>
            {
                initialiseProjection.ToEnvelope(),
                organisationCreated.ToEnvelope(),
                kboOrganisationBankAccountAdded.ToEnvelope(),
                kboOrganisationBankAccountToRemoveAdded.ToEnvelope(),
                kboOrganisationBankAccountRemoved.ToEnvelope()
            }
        );

        var organisation = _fixture.Elastic.ReadClient.Get<OrganisationDocument>(kboOrganisationBankAccountAdded.OrganisationId);

        organisation.Source.BankAccounts.Should().BeEquivalentTo(
            new List<OrganisationDocument.OrganisationBankAccount>
            {
                new(
                    kboOrganisationBankAccountAdded.OrganisationBankAccountId,
                    kboOrganisationBankAccountAdded.BankAccountNumber,
                    kboOrganisationBankAccountAdded.IsIban,
                    kboOrganisationBankAccountAdded.Bic,
                    kboOrganisationBankAccountAdded.IsBic,
                    Common.Period.FromDates(kboOrganisationBankAccountAdded.ValidFrom, kboOrganisationBankAccountAdded.ValidTo))
            });
    }

    [Fact]
    public async void OrganisationTerminated()
    {
        var scenario = new OrganisationScenario(Guid.NewGuid());
        var forcedKboTermination = false;
        scenario.AddCustomization(new ParameterNameArg<bool>("forcedKboTermination", forcedKboTermination));

        var organisationValidity = scenario.Create<DateTime?>() ?? scenario.Create<DateTime>();

        var initialiseProjection = scenario.Create<InitialiseProjection>();
        var organisationCreated = scenario.Create<OrganisationCreated>();
        var coupledWithKbo = scenario.Create<OrganisationCoupledWithKbo>();
        var organisationTerminated = scenario.CreateOrganisationTerminated(organisationCreated.OrganisationId, organisationValidity, forcedKboTermination);

        await _eventProcessor.Handle<OrganisationDocument>(
            new List<IEnvelope>
            {
                initialiseProjection.ToEnvelope(),
                organisationCreated.ToEnvelope(),
                coupledWithKbo.ToEnvelope(),
                organisationTerminated.ToEnvelope(),
            }
        );

        await _fixture.Elastic.ReadClient.Indices.RefreshAsync(Indices.Index<OrganisationDocument>());
        var organisation = _fixture.Elastic.ReadClient.Get<OrganisationDocument>(organisationCreated.OrganisationId);

        organisation.Source.Name.Should().Be(organisationCreated.Name);
        organisation.Source.ShortName.Should().Be(organisationCreated.ShortName);
        organisation.Source.Description.Should().Be(organisationCreated.Description);
        organisation.Source.Validity.Start.Should().Be(organisationCreated.ValidFrom);
        organisation.Source.KboNumber.Should().Be(coupledWithKbo.KboNumber);
        organisation.Source.Validity.End.Should().Be(organisationValidity);
    }

    [Fact]
    public async void OrganisationTerminatedWithForcedKboTermination()
    {
        var scenario = new OrganisationScenario(Guid.NewGuid());
        var forcedKboTermination = true;
        scenario.AddCustomization(new ParameterNameArg<bool>("forcedKboTermination", forcedKboTermination));

        var organisationValidity = scenario.Create<DateTime?>() ?? scenario.Create<DateTime>();

        var initialiseProjection = scenario.Create<InitialiseProjection>();
        var organisationCreated = scenario.Create<OrganisationCreated>();
        var coupledWithKbo = scenario.Create<OrganisationCoupledWithKbo>();
        var organisationTerminated = scenario.CreateOrganisationTerminated(organisationCreated.OrganisationId, organisationValidity, forcedKboTermination);

        await _eventProcessor.Handle<OrganisationDocument>(
            new List<IEnvelope>
            {
                initialiseProjection.ToEnvelope(),
                organisationCreated.ToEnvelope(),
                coupledWithKbo.ToEnvelope(),
                organisationTerminated.ToEnvelope(),
            }
        );

        var organisation = _fixture.Elastic.ReadClient.Get<OrganisationDocument>(organisationCreated.OrganisationId);

        organisation.Source.Name.Should().Be(organisationCreated.Name);
        organisation.Source.ShortName.Should().Be(organisationCreated.ShortName);
        organisation.Source.Description.Should().Be(organisationCreated.Description);
        organisation.Source.Validity.Start.Should().Be(organisationCreated.ValidFrom);
        organisation.Source.Validity.End.Should().Be(organisationTerminated.FieldsToTerminate.OrganisationValidity);
        organisation.Source.KboNumber.Should().BeEmpty();
    }

    [Fact]
    public async void BugFix_OrganisationRegulationAdded_DifferentiatesBetweenUrlAndWorkRulesUrl()
    {
        var scenario = new OrganisationScenario(Guid.NewGuid());

        var initialiseProjection = scenario.Create<InitialiseProjection>();
        var organisationCreated = scenario.Create<OrganisationCreated>();
        var organisationRegulationAdded = scenario.Create<OrganisationRegulationAdded>();

        await _eventProcessor.Handle<OrganisationDocument>(
            new List<IEnvelope>
            {
                initialiseProjection.ToEnvelope(),
                organisationCreated.ToEnvelope(),
                organisationRegulationAdded.ToEnvelope()
            }
        );

        var organisation = _fixture.Elastic.ReadClient.Get<OrganisationDocument>(organisationCreated.OrganisationId);

        organisation.Source.Regulations.Count.Should().Be(1);
        organisation.Source.Regulations.First().Url.Should().Be(organisationRegulationAdded.Uri);
        organisation.Source.Regulations.First().WorkRulesUrl.Should().Be(organisationRegulationAdded.WorkRulesUrl);
    }

    [Fact]
    public async void BugFix_OrganisationRegulationUpdated_DifferentiatesBetweenUrlAndWorkRulesUrl()
    {
        var scenario = new OrganisationScenario(Guid.NewGuid());

        var initialiseProjection = scenario.Create<InitialiseProjection>();
        var organisationCreated = scenario.Create<OrganisationCreated>();
        var organisationRegulationAdded = scenario.Create<OrganisationRegulationAdded>();
        var organisationRegulationUpdated =
            scenario.CreateOrganisationRegulationUpdated(organisationRegulationAdded);

        await _eventProcessor.Handle<OrganisationDocument>(
            new List<IEnvelope>
            {
                initialiseProjection.ToEnvelope(),
                organisationCreated.ToEnvelope(),
                organisationRegulationAdded.ToEnvelope(),
                organisationRegulationUpdated.ToEnvelope()
            }
        );

        var organisation = _fixture.Elastic.ReadClient.Get<OrganisationDocument>(organisationCreated.OrganisationId);

        organisation.Source.Regulations.Count.Should().Be(1);
        organisation.Source.Regulations.First().Url.Should().Be(organisationRegulationUpdated.Url);
        organisation.Source.Regulations.First().WorkRulesUrl.Should().Be(organisationRegulationUpdated.WorkRulesUrl);
    }

    [Fact]
    public async void OrganisationRegulationAdded_UsesDescriptionRenderedForDescription()
    {
        var scenario = new OrganisationScenario(Guid.NewGuid());

        var initialiseProjection = scenario.Create<InitialiseProjection>();
        var organisationCreated = scenario.Create<OrganisationCreated>();
        var organisationRegulationAdded = scenario.Create<OrganisationRegulationAdded>();

        await _eventProcessor.Handle<OrganisationDocument>(
            new List<IEnvelope>
            {
                initialiseProjection.ToEnvelope(),
                organisationCreated.ToEnvelope(),
                organisationRegulationAdded.ToEnvelope(),
            }
        );

        var organisation = _fixture.Elastic.ReadClient.Get<OrganisationDocument>(organisationCreated.OrganisationId);

        organisation.Source.Regulations.First().Description.Should().Be(organisationRegulationAdded.DescriptionRendered);
    }

    [Fact]
    public async void OrganisationRegulationUpdated_UsesDescriptionRenderedForDescription()
    {
        var scenario = new OrganisationScenario(Guid.NewGuid());

        var initialiseProjection = scenario.Create<InitialiseProjection>();
        var organisationCreated = scenario.Create<OrganisationCreated>();
        var organisationRegulationAdded = scenario.Create<OrganisationRegulationAdded>();
        var organisationRegulationUpdated =
            scenario.CreateOrganisationRegulationUpdated(organisationRegulationAdded);

        await _eventProcessor.Handle<OrganisationDocument>(
            new List<IEnvelope>
            {
                initialiseProjection.ToEnvelope(),
                organisationCreated.ToEnvelope(),
                organisationRegulationAdded.ToEnvelope(),
                organisationRegulationUpdated.ToEnvelope()
            }
        );

        var organisation = _fixture.Elastic.ReadClient.Get<OrganisationDocument>(organisationCreated.OrganisationId);

        organisation.Source.Regulations.First().Description.Should().Be(organisationRegulationUpdated.DescriptionRendered);
    }
}
