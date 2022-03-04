namespace OrganisationRegistry.ElasticSearch.Tests
{
    using System;
    using Api.Security;
    using Common;
    using FluentAssertions;
    using Infrastructure.Bus;
    using Infrastructure.Config;
    using Infrastructure.Events;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Organisation.Events;
    using OrganisationRegistry.Tests.Shared.Stubs;
    using Organisations;
    using Projections.Infrastructure;
    using Projections.Organisations;
    using Scenario;
    using Scenario.Specimen;
    using Security;
    using SqlServer.Infrastructure;
    using Xunit;

    [Collection(nameof(ElasticSearchFixture))]
    public class OrganisationHandlerTests
    {
        private readonly ElasticSearchFixture _fixture;
        private readonly InProcessBus _inProcessBus;

        public OrganisationHandlerTests(ElasticSearchFixture fixture)
        {
            _fixture = fixture;

            var dbContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseInMemoryDatabase(
                    $"org-es-test-{Guid.NewGuid()}",
                    builder => { }).Options;
            var context = new OrganisationRegistryContext(
                dbContextOptions);

            var organisationHandler = new Organisation(
                logger: _fixture.LoggerFactory.CreateLogger<Organisation>(),
                elastic: _fixture.Elastic,
                elasticSearchOptions: _fixture.ElasticSearchOptions,
                organisationManagementConfiguration: new OrganisationManagementConfigurationStub());

            var organisationBankAccountHandler = new OrganisationBankAccount(
                _fixture.LoggerFactory.CreateLogger<OrganisationBankAccount>());

            var testContextFactory = new TestContextFactory(dbContextOptions);
            var serviceProvider = new ServiceCollection()
                .AddSingleton(organisationHandler)
                .AddSingleton(organisationBankAccountHandler)
                .AddSingleton(new MemoryCachesMaintainer(new MemoryCaches(testContextFactory), testContextFactory))
                .BuildServiceProvider();

            _inProcessBus = new InProcessBus(
                new NullLogger<InProcessBus>(),
                new SecurityService(
                    fixture.ContextFactory,
                    new OrganisationRegistryConfigurationStub(),
                    Mock.Of<ICache<OrganisationSecurityInformation>>()));
            var registrar = new BusRegistrar(new NullLogger<BusRegistrar>(), _inProcessBus, () => serviceProvider);
            registrar.RegisterEventHandlers(OrganisationsRunner.EventHandlers);
        }

        [EnvVarIgnoreFact]
        public void InitializeProjection_CreatesIndex()
        {
            var scenario = new OrganisationScenario(Guid.NewGuid());

            Handle(scenario.Create<InitialiseProjection>());

            var indices = _fixture.Elastic.ReadClient.Indices
                .Get(_fixture.ElasticSearchOptions.Value.OrganisationsReadIndex).Indices;
            indices.Should().NotBeEmpty();
        }

        [EnvVarIgnoreFact]
        public void OrganisationCreated_CreatesDocument()
        {
            var scenario = new OrganisationScenario(Guid.NewGuid());

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var organisationCreated = scenario.Create<OrganisationCreated>();

            Handle(
                initialiseProjection,
                organisationCreated);

            var organisation =
                _fixture.Elastic.ReadClient.Get<OrganisationDocument>(organisationCreated.OrganisationId);

            organisation.Source.Name.Should().Be(organisationCreated.Name);
            organisation.Source.ShortName.Should().Be(organisationCreated.ShortName);
            organisation.Source.Description.Should().Be(organisationCreated.Description);
        }

        [EnvVarIgnoreFact]
        public void OrganisationKboBankAccountAdded_AddsBankAccount()
        {
            var scenario = new OrganisationScenario(Guid.NewGuid());

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var organisationCreated = scenario.Create<OrganisationCreated>();
            var kboOrganisationBankAccountAdded = scenario.Create<KboOrganisationBankAccountAdded>();
            var kboOrganisationBankAccountAdded2 = scenario.Create<KboOrganisationBankAccountAdded>();

            Handle(
                initialiseProjection,
                organisationCreated,
                kboOrganisationBankAccountAdded,
                kboOrganisationBankAccountAdded2);

            var organisation =
                _fixture.Elastic.ReadClient.Get<OrganisationDocument>(kboOrganisationBankAccountAdded.OrganisationId);

            organisation.Source.BankAccounts.Should().BeEquivalentTo(
                new OrganisationDocument.OrganisationBankAccount(
                    kboOrganisationBankAccountAdded.OrganisationBankAccountId,
                    kboOrganisationBankAccountAdded.BankAccountNumber,
                    kboOrganisationBankAccountAdded.IsIban,
                    kboOrganisationBankAccountAdded.Bic,
                    kboOrganisationBankAccountAdded.IsBic,
                    Period.FromDates(kboOrganisationBankAccountAdded.ValidFrom, kboOrganisationBankAccountAdded.ValidTo)),
                new OrganisationDocument.OrganisationBankAccount(
                    kboOrganisationBankAccountAdded2.OrganisationBankAccountId,
                    kboOrganisationBankAccountAdded2.BankAccountNumber,
                    kboOrganisationBankAccountAdded2.IsIban,
                    kboOrganisationBankAccountAdded2.Bic,
                    kboOrganisationBankAccountAdded2.IsBic,
                    Period.FromDates(kboOrganisationBankAccountAdded2.ValidFrom, kboOrganisationBankAccountAdded2.ValidTo)));
        }

        [EnvVarIgnoreFact]
        public void OrganisationKboBankAccountRemoved_RemovesBankAccount()
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

            Handle(
                initialiseProjection,
                organisationCreated,
                kboOrganisationBankAccountAdded,
                kboOrganisationBankAccountToRemoveAdded,
                kboOrganisationBankAccountRemoved);

            var organisation =
                _fixture.Elastic.ReadClient.Get<OrganisationDocument>(kboOrganisationBankAccountAdded.OrganisationId);

            organisation.Source.BankAccounts.Should().BeEquivalentTo(
                new OrganisationDocument.OrganisationBankAccount(
                    kboOrganisationBankAccountAdded.OrganisationBankAccountId,
                    kboOrganisationBankAccountAdded.BankAccountNumber,
                    kboOrganisationBankAccountAdded.IsIban,
                    kboOrganisationBankAccountAdded.Bic,
                    kboOrganisationBankAccountAdded.IsBic,
                    Period.FromDates(kboOrganisationBankAccountAdded.ValidFrom, kboOrganisationBankAccountAdded.ValidTo)));
        }

        [EnvVarIgnoreFact]
        public void OrganisationTerminated()
        {
            var scenario = new OrganisationScenario(Guid.NewGuid());
            scenario.AddCustomization(new ParameterNameArg<bool>("forcedKboTermination", false));


            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var organisationCreated = scenario.Create<OrganisationCreated>();
            var coupledWithKbo = scenario.Create<OrganisationCoupledWithKbo>();
            var organisationTerminated = scenario.Create<OrganisationTerminated>();

            Handle(
                initialiseProjection,
                organisationCreated,
                coupledWithKbo,
                organisationTerminated);

            var organisation =
                _fixture.Elastic.ReadClient.Get<OrganisationDocument>(organisationCreated.OrganisationId);

            organisation.Source.Name.Should().Be(organisationCreated.Name);
            organisation.Source.ShortName.Should().Be(organisationCreated.ShortName);
            organisation.Source.Description.Should().Be(organisationCreated.Description);
            organisation.Source.Validity.Start.Should().Be(organisationCreated.ValidFrom);
            organisation.Source.Validity.End.Should().Be(organisationTerminated.FieldsToTerminate.OrganisationValidity);
            organisation.Source.KboNumber.Should().Be(coupledWithKbo.KboNumber);
        }

        [EnvVarIgnoreFact]
        public void OrganisationTerminatedWithForcedKboTermination()
        {
            var scenario = new OrganisationScenario(Guid.NewGuid());
            scenario.AddCustomization(new ParameterNameArg<bool>("forcedKboTermination", true));


            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var organisationCreated = scenario.Create<OrganisationCreated>();
            var coupledWithKbo = scenario.Create<OrganisationCoupledWithKbo>();
            var organisationTerminated = scenario.Create<OrganisationTerminated>();

            Handle(
                initialiseProjection,
                organisationCreated,
                coupledWithKbo,
                organisationTerminated);

            var organisation =
                _fixture.Elastic.ReadClient.Get<OrganisationDocument>(organisationCreated.OrganisationId);

            organisation.Source.Name.Should().Be(organisationCreated.Name);
            organisation.Source.ShortName.Should().Be(organisationCreated.ShortName);
            organisation.Source.Description.Should().Be(organisationCreated.Description);
            organisation.Source.Validity.Start.Should().Be(organisationCreated.ValidFrom);
            organisation.Source.Validity.End.Should().Be(organisationTerminated.FieldsToTerminate.OrganisationValidity);
            organisation.Source.KboNumber.Should().BeEmpty();
        }

        private void Handle(params IEvent[] envelopes)
        {
            foreach (var envelope in envelopes)
            {
                _inProcessBus.Publish(null, null, (dynamic)envelope.ToEnvelope());
            }
        }
    }
}
