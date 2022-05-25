namespace OrganisationRegistry.UnitTests.Organisation.Kbo
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using OrganisationRegistry.KeyTypes.Events;
    using LabelType.Events;
    using LocationType.Events;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationClassification;
    using OrganisationClassification.Events;
    using OrganisationClassificationType;
    using OrganisationClassificationType.Events;
    using OrganisationRegistry.Infrastructure.Domain;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Tests.Shared;
    using Tests.Shared.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    public class
        UpdateFromKboWithoutLegalFormTests : Specification<KboOrganisationCommandHandlers, SyncOrganisationWithKbo>
    {
        private readonly OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub;

        private readonly Guid _organisationId;
        private readonly Guid _kboSyncItemId;
        private readonly Guid _legalFormOrganisationClassificationTypeId;
        private readonly Guid _organisationClassificationId;
        private readonly Guid _anotherOrganisationClassificationId;
        private readonly string _kboNumber;
        private readonly DateTimeProviderStub _dateTimeProviderStub;

        public UpdateFromKboWithoutLegalFormTests(ITestOutputHelper helper) : base(helper)
        {
            _organisationRegistryConfigurationStub = new OrganisationRegistryConfigurationStub
            {
                KboKeyTypeId = Guid.NewGuid(),
                Kbo = new KboConfigurationStub
                {
                    KboV2LegalFormOrganisationClassificationTypeId = Guid.NewGuid(),
                    KboV2RegisteredOfficeLocationTypeId = Guid.NewGuid(),
                    KboV2FormalNameLabelTypeId = Guid.NewGuid(),
                }
            };
            _dateTimeProviderStub = new DateTimeProviderStub(new DateTime(2019, 9, 20));
            _organisationId = new OrganisationId(Guid.NewGuid());
            _legalFormOrganisationClassificationTypeId = new OrganisationClassificationTypeId(
                _organisationRegistryConfigurationStub.Kbo.KboV2LegalFormOrganisationClassificationTypeId);
            _organisationClassificationId = new OrganisationClassificationId(Guid.NewGuid());
            _anotherOrganisationClassificationId = new OrganisationClassificationId(Guid.NewGuid());
            _kboSyncItemId = Guid.NewGuid();
            _kboNumber = "BE0123456789";
        }

        private IEvent[] Events
            => new IEvent[]
            {
                new KeyTypeCreated(_organisationRegistryConfigurationStub.KboKeyTypeId, "KBO sleutel"),
                new OrganisationClassificationTypeCreated(
                    _legalFormOrganisationClassificationTypeId,
                    "ClassificatieType"),
                new OrganisationClassificationCreated(
                    _organisationClassificationId,
                    "Classificatie",
                    1,
                    "Some Legal Code",
                    true,
                    _legalFormOrganisationClassificationTypeId,
                    "ClassificatieType"),
                new OrganisationClassificationCreated(
                    _anotherOrganisationClassificationId,
                    "Classificatie",
                    1,
                    "Another Legal Code",
                    true,
                    _legalFormOrganisationClassificationTypeId,
                    "ClassificatieType"),
                new LabelTypeCreated(
                    _organisationRegistryConfigurationStub.Kbo.KboV2FormalNameLabelTypeId,
                    "KBO formele naam"),
                new LocationTypeCreated(
                    _organisationRegistryConfigurationStub.Kbo.KboV2RegisteredOfficeLocationTypeId,
                    "Registered KBO Office"),
                new OrganisationCreatedFromKbo(
                    _organisationId,
                    new KboNumber(_kboNumber).ToDigitsOnly(),
                    "KBO formele naam",
                    "OVO001234",
                    "Korte naam",
                    Article.None,
                    "",
                    new List<Purpose>(),
                    false,
                    new ValidFrom(),
                    new ValidTo(),
                    new ValidFrom(),
                    new ValidTo()),
                new KboFormalNameLabelAdded(
                    _organisationId,
                    Guid.NewGuid(),
                    _organisationRegistryConfigurationStub.Kbo.KboV2FormalNameLabelTypeId,
                    "KBO Formal Name Type",
                    "KBO formele naam",
                    new ValidFrom(2008, 12, 22),
                    null),
                new KboLegalFormOrganisationOrganisationClassificationAdded(
                    _organisationId,
                    Guid.NewGuid(),
                    _legalFormOrganisationClassificationTypeId,
                    "ClassificatieType",
                    _organisationClassificationId,
                    "Classificatie",
                    new ValidFrom(2020, 12, 11),
                    new ValidTo(2020, 12, 12))
            };

        private SyncOrganisationWithKbo SyncOrganisationWithKboCommand
            => new(
                new OrganisationId(_organisationId),
                new DateTimeOffset(new DateTime(2019, 9, 9)),
                _kboSyncItemId);

        protected override KboOrganisationCommandHandlers BuildHandler(ISession session)
            => new(
                new Mock<ILogger<KboOrganisationCommandHandlers>>().Object,
                _organisationRegistryConfigurationStub,
                session,
                new SequentialOvoNumberGenerator(),
                new UniqueOvoNumberValidatorStub(false),
                new UniqueKboNumberValidatorStub(false),
                _dateTimeProviderStub,
                new KboOrganisationRetrieverStub(
                    new MockMagdaOrganisationResponse
                    {
                        FormalName = new NameStub("KBO formele naam", new DateTime(2009, 1, 1)),
                        ShortName = new NameStub("Korte naam", new DateTime(2010, 1, 1)),
                        ValidFrom = new DateTime(2000, 12, 31),
                        LegalForm = null,
                    }),
                Mock.Of<IKboOrganisationClassificationRetriever>(),
                Mock.Of<IKboLocationRetriever>());

        [Fact]
        public async Task PublishesTwoEvents()
        {
            await Given(Events).When(SyncOrganisationWithKboCommand, UserBuilder.User())
                .ThenItPublishesTheCorrectNumberOfEvents(2);
        }

        [Fact]
        public async Task UpdatesLegalForms()
        {
            await Given(Events).When(SyncOrganisationWithKboCommand, UserBuilder.User()).Then();
            var legalFormOrganisationOrganisationClassificationEnded = PublishedEvents[0]
                .UnwrapBody<KboLegalFormOrganisationOrganisationClassificationRemoved>();
            legalFormOrganisationOrganisationClassificationEnded.Should().NotBeNull();

            legalFormOrganisationOrganisationClassificationEnded.OrganisationId.Should().Be(_organisationId);
            legalFormOrganisationOrganisationClassificationEnded.OrganisationOrganisationClassificationId.Should()
                .NotBeEmpty();
            legalFormOrganisationOrganisationClassificationEnded.OrganisationClassificationTypeId.Should()
                .Be(_legalFormOrganisationClassificationTypeId);
            legalFormOrganisationOrganisationClassificationEnded.OrganisationClassificationTypeName.Should()
                .Be("ClassificatieType");
            legalFormOrganisationOrganisationClassificationEnded.OrganisationClassificationId.Should()
                .Be(_organisationClassificationId);
            legalFormOrganisationOrganisationClassificationEnded.OrganisationClassificationName.Should()
                .Be("Classificatie");
            legalFormOrganisationOrganisationClassificationEnded.ValidFrom.Should().Be(new ValidFrom(2020, 12, 11));
            legalFormOrganisationOrganisationClassificationEnded.ValidTo.Should().Be(new ValidTo(2020, 12, 12));
        }

        [Fact]
        public async Task MarksAsSynced()
        {
            await Given(Events).When(SyncOrganisationWithKboCommand, UserBuilder.User()).Then();
            var organisationSyncedFromKbo = PublishedEvents[1].UnwrapBody<OrganisationSyncedFromKbo>();
            organisationSyncedFromKbo.Should().NotBeNull();

            organisationSyncedFromKbo.OrganisationId.Should().Be(_organisationId);
            organisationSyncedFromKbo.KBOSyncItemId.Should().Be(_kboSyncItemId);
        }
    }
}
