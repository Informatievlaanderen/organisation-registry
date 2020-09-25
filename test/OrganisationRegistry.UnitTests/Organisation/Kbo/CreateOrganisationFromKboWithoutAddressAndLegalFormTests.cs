namespace OrganisationRegistry.UnitTests.Organisation.Kbo
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using KeyTypes.Events;
    using LabelType.Events;
    using Location;
    using Location.Events;
    using LocationType.Events;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationClassification;
    using OrganisationClassification.Events;
    using OrganisationClassificationType;
    using OrganisationClassificationType.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Purpose;
    using Purpose.Events;
    using Tests.Shared;
    using Xunit;
    using Xunit.Abstractions;
    using Purpose = OrganisationRegistry.Organisation.Events.Purpose;

    public class CreateOrganisationFromKboWithoutAddressAndLegalFormTests: Specification<Organisation, KboOrganisationCommandHandlers, CreateOrganisationFromKbo>
    {
        private OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub;

        private OrganisationId _parentOrganisationId;
        private OrganisationId _organisationId;
        private PurposeId _purposeId;
        private OrganisationClassificationTypeId _organisationClassificationTypeId;
        private OrganisationClassificationId _organisationClassificationId;
        private LocationId _locationId;
        private readonly DateTime _kboOrganisationValidFromDate = new DateTime(2000, 12, 31);

        protected override IEnumerable<IEvent> Given()
        {
            _organisationRegistryConfigurationStub = new OrganisationRegistryConfigurationStub
            {
                KboKeyTypeId = Guid.NewGuid(),
                KboV2LegalFormOrganisationClassificationTypeId = Guid.NewGuid(),
                KboV2RegisteredOfficeLocationTypeId = Guid.NewGuid(),
                KboV2FormalNameLabelTypeId = Guid.NewGuid(),
            };

            _parentOrganisationId = new OrganisationId(Guid.NewGuid());
            _organisationId = new OrganisationId(Guid.NewGuid());
            _purposeId = new PurposeId(Guid.NewGuid());
            _locationId = new LocationId(Guid.NewGuid());
            _organisationClassificationTypeId = new OrganisationClassificationTypeId(_organisationRegistryConfigurationStub.KboV2LegalFormOrganisationClassificationTypeId);
            _organisationClassificationId = new OrganisationClassificationId(Guid.NewGuid());

            return new List<IEvent>
            {
                new OrganisationCreated(
                    _parentOrganisationId,
                    "parent",
                    "OVO001234",
                    "ouder",
                    "",
                    new List<Purpose>(),
                    false,
                    new ValidFrom(),
                    new ValidTo()),

                new PurposeCreated(
                    _purposeId,
                    "Purpose X"),

                new KeyTypeCreated(
                    _organisationRegistryConfigurationStub.KboKeyTypeId,
                    "KBO sleutel"),

                new OrganisationClassificationTypeCreated(
                    _organisationClassificationTypeId,
                    "ClassificatieType"),

                new OrganisationClassificationCreated(
                    _organisationClassificationId,
                    "Classificatie",
                    1,
                    "Some Legal Code",
                    true,
                    _organisationClassificationTypeId,
                    "ClassificatieType"),

                new LocationTypeCreated(
                    _organisationRegistryConfigurationStub.KboV2RegisteredOfficeLocationTypeId,
                    "Registered KBO Office"),

                new LocationCreated(_locationId,
                    null,
                    "Waregemsestraat, 8999 Evergem, Belgie",
                    "Waregemsestraat",
                    "8999",
                    "Evergem",
                    "Belgie"),

                new LabelTypeCreated(
                    _organisationRegistryConfigurationStub.KboV2FormalNameLabelTypeId,
                    "KBO formele naam")
            };
        }

        protected override CreateOrganisationFromKbo When()
        {
            return new CreateOrganisationFromKbo(
                _organisationId,
                "Naam",
                "OVO000070",
                "Korte naam",
                _parentOrganisationId,
                "Mijn omschrijving",
                new List<PurposeId> { _purposeId },
                true,
                new ValidFrom(),
                new ValidTo(),
                null,
                new KboNumber("BE0123456789"));
        }

        protected override KboOrganisationCommandHandlers BuildHandler()
        {
            return new KboOrganisationCommandHandlers(
                new Mock<ILogger<KboOrganisationCommandHandlers>>().Object,
                _organisationRegistryConfigurationStub,
                Session,
                new SequentialOvoNumberGenerator(),
                new UniqueOvoNumberValidatorStub(false),
                new UniqueKboNumberValidatorStub(false),
                new DateTimeProviderStub(DateTime.Today),
                new KboOrganisationRetrieverStub(new MockMagdaOrganisationResponse
                {
                    FormalName = new NameStub("NAME FROM KBO", _kboOrganisationValidFromDate),
                    ShortName = new NameStub("SHORT NAME FROM KBO", _kboOrganisationValidFromDate),
                    ValidFrom = _kboOrganisationValidFromDate,
                    BankAccounts =
                    {
                        new BankAccountStub
                        {
                            AccountNumber = "BE71 0961 2345 6769",
                            Bic = "GKCCBEBB",
                            ValidFrom = new DateTime(2000, 1, 1),
                            ValidTo = new DateTime(2001, 1, 1),
                        }
                    },
                    LegalForm = null,
                    Address = null,
                }),
                new KboOrganisationClassificationRetrieverStub("Some Legal Code", _organisationClassificationId),
                new KboLocationRetrieverStub(address => (Guid?)null));
        }

        protected override int ExpectedNumberOfEvents => 6;

        [Fact]
        public void CreatesAnOrganisation()
        {
            var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationCreatedFromKbo>();
            organisationCreated.Should().NotBeNull();

            organisationCreated.OrganisationId.Should().Be((Guid)_organisationId);
            organisationCreated.Name.Should().Be("NAME FROM KBO");
            organisationCreated.OvoNumber.Should().Be("OVO000070");
            organisationCreated.ShortName.Should().Be("SHORT NAME FROM KBO");
            organisationCreated.Description.Should().Be("Mijn omschrijving");
            organisationCreated.Purposes.Should().BeEquivalentTo(new List<Purpose> { new Purpose(_purposeId, "Purpose X") });
            organisationCreated.ShowOnVlaamseOverheidSites.Should().Be(true);
            organisationCreated.ValidFrom.Should().Be(new ValidFrom(_kboOrganisationValidFromDate));
            organisationCreated.ValidTo.Should().Be(new ValidTo());
        }

        [Fact]
        public void TheOrganisationBecomesActive()
        {
            var organisationBecameActive = PublishedEvents[1].UnwrapBody<OrganisationBecameActive>();
            organisationBecameActive.Should().NotBeNull();
        }

        [Fact]
        public void TheParentOrganisationWasAdded()
        {
            var parentAdded = PublishedEvents[2].UnwrapBody<OrganisationParentAdded>();
            parentAdded.Should().NotBeNull();
        }

        [Fact]
        public void TheParentOrganisationBecameActive()
        {
            var parentAssignedToOrganisation = PublishedEvents[3].UnwrapBody<ParentAssignedToOrganisation>();
            parentAssignedToOrganisation.Should().NotBeNull();
        }

        [Fact]
        public void AddsBankAccounts()
        {
            var organisationBankAccountAdded = PublishedEvents[4].UnwrapBody<KboOrganisationBankAccountAdded>();
            organisationBankAccountAdded.Should().NotBeNull();

            organisationBankAccountAdded.OrganisationId.Should().Be((Guid)_organisationId);
            organisationBankAccountAdded.OrganisationBankAccountId.Should().NotBeEmpty();
            organisationBankAccountAdded.BankAccountNumber.Should().Be("BE71096123456769");
            organisationBankAccountAdded.Bic.Should().Be("GKCCBEBB");
            organisationBankAccountAdded.IsIban.Should().Be(true);
            organisationBankAccountAdded.IsBic.Should().Be(true);
            organisationBankAccountAdded.ValidFrom.Should().Be(new ValidFrom(2000, 1, 1));
            organisationBankAccountAdded.ValidTo.Should().Be(new ValidTo(2001, 1, 1));
        }

        [Fact]
        public void AddsFormalNameLabel()
        {
            var organisationLabelAdded = PublishedEvents[5].UnwrapBody<KboFormalNameLabelAdded>();
            organisationLabelAdded.Should().NotBeNull();

            organisationLabelAdded.OrganisationId.Should().Be((Guid)_organisationId);
            organisationLabelAdded.OrganisationLabelId.Should().NotBeEmpty();
            organisationLabelAdded.LabelTypeId.Should().Be(_organisationRegistryConfigurationStub.KboV2FormalNameLabelTypeId);
            organisationLabelAdded.LabelTypeName.Should().Be("KBO formele naam");
            organisationLabelAdded.Value.Should().Be("NAME FROM KBO");
            organisationLabelAdded.ValidFrom.Should().Be(new ValidFrom(_kboOrganisationValidFromDate));
            organisationLabelAdded.ValidTo.Should().Be(new ValidTo());
        }

        public CreateOrganisationFromKboWithoutAddressAndLegalFormTests(ITestOutputHelper helper) : base(helper) { }
    }
}
