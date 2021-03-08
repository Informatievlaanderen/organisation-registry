namespace OrganisationRegistry.UnitTests.Organisation.Kbo
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using KeyTypes.Events;
    using LabelType.Events;
    using Location;
    using LocationType.Events;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationClassification;
    using OrganisationClassification.Events;
    using OrganisationClassificationType;
    using OrganisationClassificationType.Events;
    using Tests.Shared;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class UpdateFromKboWithoutLegalFormTests: Specification<Organisation, KboOrganisationCommandHandlers, SyncOrganisationWithKbo>
    {
        private OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub;

        private Guid _organisationId;
        private Guid _kboSyncItemId;
        private Guid _legalFormOrganisationClassificationTypeId;
        private Guid _organisationClassificationId;
        private Guid _anotherOrganisationClassificationId;
        private DateTimeProviderStub _dateTimeProviderStub;
        private KboNumber _kboNumber;
        private readonly DateTime _today = new DateTime(2019, 9, 20);

        protected override IEnumerable<IEvent> Given()
        {
            _organisationRegistryConfigurationStub = new OrganisationRegistryConfigurationStub
            {
                KboKeyTypeId = Guid.NewGuid(),
                KboV2LegalFormOrganisationClassificationTypeId = Guid.NewGuid(),
                KboV2RegisteredOfficeLocationTypeId = Guid.NewGuid(),
                KboV2FormalNameLabelTypeId = Guid.NewGuid(),
            };
            _kboNumber = new KboNumber("BE0123456789");
            _organisationId = new OrganisationId(Guid.NewGuid());
            _legalFormOrganisationClassificationTypeId = new OrganisationClassificationTypeId(_organisationRegistryConfigurationStub.KboV2LegalFormOrganisationClassificationTypeId);
            _organisationClassificationId = new OrganisationClassificationId(Guid.NewGuid());
            _anotherOrganisationClassificationId = new OrganisationClassificationId(Guid.NewGuid());
            _kboSyncItemId = Guid.NewGuid();

            return new List<IEvent>
            {
                new KeyTypeCreated(_organisationRegistryConfigurationStub.KboKeyTypeId, "KBO sleutel"),
                new OrganisationClassificationTypeCreated(_legalFormOrganisationClassificationTypeId, "ClassificatieType"),
                new OrganisationClassificationCreated(_organisationClassificationId, "Classificatie", 1, "Some Legal Code", true, _legalFormOrganisationClassificationTypeId, "ClassificatieType"),
                new OrganisationClassificationCreated(_anotherOrganisationClassificationId, "Classificatie", 1, "Another Legal Code", true, _legalFormOrganisationClassificationTypeId, "ClassificatieType"),
                new LabelTypeCreated(_organisationRegistryConfigurationStub.KboV2FormalNameLabelTypeId, "KBO formele naam"),
                new LocationTypeCreated(_organisationRegistryConfigurationStub.KboV2RegisteredOfficeLocationTypeId, "Registered KBO Office"),
                new OrganisationCreatedFromKbo(
                    _organisationId,
                    _kboNumber.ToDigitsOnly(),
                    "KBO formele naam",
                    "OVO001234",
                    "Korte naam",
                    "",
                    new List<Purpose>(),
                    false,
                    new ValidFrom(),
                    new ValidTo()),
                new KboFormalNameLabelAdded(
                    _organisationId,
                    Guid.NewGuid(),
                    _organisationRegistryConfigurationStub.KboV2FormalNameLabelTypeId,
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
        }

        protected override SyncOrganisationWithKbo When()
        {
            return new SyncOrganisationWithKbo(
                new OrganisationId(_organisationId),
                new DateTimeOffset(new DateTime(2019, 9, 9)),
                _kboSyncItemId);
        }

        protected override KboOrganisationCommandHandlers BuildHandler()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(_today);
            return new KboOrganisationCommandHandlers(
                logger: new Mock<ILogger<KboOrganisationCommandHandlers>>().Object,
                organisationRegistryConfiguration: _organisationRegistryConfigurationStub,
                session: Session,
                ovoNumberGenerator: new SequentialOvoNumberGenerator(),
                uniqueOvoNumberValidator: new UniqueOvoNumberValidatorStub(false),
                uniqueKboValidator: new UniqueKboNumberValidatorStub(false),
                dateTimeProvider: _dateTimeProviderStub,
                kboOrganisationRetriever: new KboOrganisationRetrieverStub(
                    new MockMagdaOrganisationResponse
                    {
                        FormalName = new NameStub("KBO formele naam", new DateTime(2009, 1, 1)),
                        ShortName = new NameStub("Korte naam", new DateTime(2010, 1, 1)),
                        ValidFrom = new DateTime(2000, 12, 31),
                        LegalForm = null,
                    }),
                organisationClassificationRetriever: Mock.Of<IKboOrganisationClassificationRetriever>(),
                locationRetriever: Mock.Of<IKboLocationRetriever>());
        }

        protected override int ExpectedNumberOfEvents => 2;

        [Fact]
        public void UpdatesLegalForms()
        {
            var legalFormOrganisationOrganisationClassificationEnded = PublishedEvents[0].UnwrapBody<KboLegalFormOrganisationOrganisationClassificationRemoved>();
            legalFormOrganisationOrganisationClassificationEnded.Should().NotBeNull();

            legalFormOrganisationOrganisationClassificationEnded.OrganisationId.Should().Be(_organisationId);
            legalFormOrganisationOrganisationClassificationEnded.OrganisationOrganisationClassificationId.Should().NotBeEmpty();
            legalFormOrganisationOrganisationClassificationEnded.OrganisationClassificationTypeId.Should().Be(_legalFormOrganisationClassificationTypeId);
            legalFormOrganisationOrganisationClassificationEnded.OrganisationClassificationTypeName.Should().Be("ClassificatieType");
            legalFormOrganisationOrganisationClassificationEnded.OrganisationClassificationId.Should().Be(_organisationClassificationId);
            legalFormOrganisationOrganisationClassificationEnded.OrganisationClassificationName.Should().Be("Classificatie");
            legalFormOrganisationOrganisationClassificationEnded.ValidFrom.Should().Be(new ValidFrom(2020, 12, 11));
            legalFormOrganisationOrganisationClassificationEnded.ValidTo.Should().Be(new ValidTo(2020, 12, 12));
        }

        [Fact]
        public void MarksAsSynced()
        {
            var organisationSyncedFromKbo = PublishedEvents[1].UnwrapBody<OrganisationSyncedFromKbo>();
            organisationSyncedFromKbo.Should().NotBeNull();

            organisationSyncedFromKbo.OrganisationId.Should().Be(_organisationId);
            organisationSyncedFromKbo.KBOSyncItemId.Should().Be(_kboSyncItemId);
        }

        public UpdateFromKboWithoutLegalFormTests(ITestOutputHelper helper) : base(helper)
        {
        }
    }
}
