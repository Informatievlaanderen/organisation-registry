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
    using Tests.Shared;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class UpdateFromKboTests: Specification<Organisation, KboOrganisationCommandHandlers, UpdateFromKbo>
    {
        private OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub;

        private Guid _organisationId;
        private Guid _legalFormOrganisationClassificationTypeId;
        private Guid _organisationClassificationId;
        private Guid _anotherOrganisationClassificationId;
        private Guid _registeredOfficeLocationId;
        private Guid _registeredOfficeLocationToRemoveId;
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
            _registeredOfficeLocationId = new LocationId(Guid.NewGuid());
            _registeredOfficeLocationToRemoveId = new LocationId(Guid.NewGuid());
            _legalFormOrganisationClassificationTypeId = new OrganisationClassificationTypeId(_organisationRegistryConfigurationStub.KboV2LegalFormOrganisationClassificationTypeId);
            _organisationClassificationId = new OrganisationClassificationId(Guid.NewGuid());
            _anotherOrganisationClassificationId = new OrganisationClassificationId(Guid.NewGuid());

            return new List<IEvent>
            {
                new KeyTypeCreated(_organisationRegistryConfigurationStub.KboKeyTypeId, "KBO sleutel"),
                new OrganisationClassificationTypeCreated(_legalFormOrganisationClassificationTypeId, "ClassificatieType"),
                new OrganisationClassificationCreated(_organisationClassificationId, "Classificatie", 1, "Some Legal Code", true, _legalFormOrganisationClassificationTypeId, "ClassificatieType"),
                new OrganisationClassificationCreated(_anotherOrganisationClassificationId, "Classificatie", 1, "Another Legal Code", true, _legalFormOrganisationClassificationTypeId, "ClassificatieType"),
                new LocationTypeCreated(_organisationRegistryConfigurationStub.KboV2RegisteredOfficeLocationTypeId, "Registered KBO Office"),
                new LocationCreated(_registeredOfficeLocationId,
                    null,
                    "Waregemsestraat, 8999 Evergem, Belgie",
                    "Waregemsestraat",
                    "8999",
                    "Evergem",
                    "Belgie"),
                new LocationCreated(_registeredOfficeLocationToRemoveId,
                    null,
                    "Derbylaan, 8881 Adinkerke, Belgie",
                    "Derbylaan",
                    "8881",
                    "Adinkerke",
                    "Belgie"),
                new LabelTypeCreated(_organisationRegistryConfigurationStub.KboV2FormalNameLabelTypeId, "Kbo formele naam"),
                new OrganisationCreatedFromKbo(
                    _organisationId,
                    _kboNumber.ToDigitsOnly(),
                    "organisation X",
                    "OVO001234",
                    "org",
                    "",
                    new List<Purpose>(),
                    false,
                    new ValidFrom(),
                    new ValidTo()),
                new KboRegisteredOfficeOrganisationLocationAdded(
                    _organisationId,
                    Guid.NewGuid(),
                    _registeredOfficeLocationToRemoveId,
                    "Derbylaan, 8881 Adinkerke, Belgie",
                    false,
                    _organisationRegistryConfigurationStub.KboV2RegisteredOfficeLocationTypeId,
                    "Registered KBO Office",
                    new ValidFrom(1999, 12, 31),
                    null),
                new KboFormalNameLabelAdded(
                    _organisationId,
                    Guid.NewGuid(),
                    _organisationRegistryConfigurationStub.KboV2FormalNameLabelTypeId,
                    "Kbo formele naam",
                    "Formele naam organisatie",
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
                    new ValidTo(2020, 12, 12)),
                new KboOrganisationBankAccountAdded(
                    _organisationId,
                    Guid.NewGuid(),
                    "BE71 0000 2345 6769",
                    true,
                    "GKCCBEBB",
                    true,
                    new DateTime(2000, 1, 1),
                    new DateTime(2001, 1, 1)),
                new KboOrganisationBankAccountAdded(
                    _organisationId,
                    Guid.NewGuid(),
                    "BE71 0961 2345 6769",
                    true,
                    "GKCCBEBB",
                    true,
                    new DateTime(2000, 1, 1),
                    new DateTime(2001, 1, 1))
            };
        }

        protected override UpdateFromKbo When()
        {
            return new UpdateFromKbo(
                new OrganisationId(_organisationId),
                null,
                new DateTimeOffset(new DateTime(2019, 9, 9)));
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
                        FormalName = new NameStub("NAME FROM KBO", new DateTime(2009, 1, 1)),
                        ShortName = new NameStub("SHORT NAME FROM KBO", new DateTime(2010, 1, 1)),
                        ValidFrom = new DateTime(2000, 12, 31),
                        BankAccounts =
                        {
                            new BankAccountStub
                            {
                                Iban = "BE71 0961 2345 6769",
                                Bic = "GKCCBEBB",
                                ValidFrom = new DateTime(1990, 1, 1),
                                ValidTo = new DateTime(2010, 1, 1),
                            },
                            new BankAccountStub
                            {
                                Iban = "BE88 8881 2345 6769",
                                Bic = "GKCCBEBB",
                                ValidFrom = new DateTime(2000, 1, 1),
                                ValidTo = new DateTime(2001, 1, 1),
                            }

                        },
                        LegalForm =
                            new LegalFormStub
                            {
                                Code = "Another Legal Code",
                                ValidFrom = new DateTime(2020, 12, 11),
                                ValidTo = new DateTime(2020, 12, 12)
                            },
                        Address =
                            new AddressStub
                            {
                                City = "Evergem",
                                Street = "Waregemsestraat",
                                Country = "Belgie",
                                ZipCode = "8999",
                                ValidFrom = null,
                                ValidTo = null
                            }
                    }),
                organisationClassificationRetriever: new KboOrganisationClassificationRetrieverStub(
                    "Another Legal Code", _anotherOrganisationClassificationId),
                locationRetriever: new KboLocationRetrieverStub(address => (Guid?) null));
        }

        protected override int ExpectedNumberOfEvents => 8;

        [Fact(Skip = "Under construction")]
        public void CreatesTheMissingLocationsOnceBeforeCreatingTheOrganisation()
        {
            var organisationCreated = PublishedEvents[0].UnwrapBody<LocationCreated>();
            organisationCreated.Should().NotBeNull();

            organisationCreated.LocationId.Should().NotBeEmpty();
            organisationCreated.Street.Should().Be("Waregemsestraat");
            organisationCreated.City.Should().Be("Evergem");
            organisationCreated.ZipCode.Should().Be("8999");
            organisationCreated.Country.Should().Be("Belgie");
            organisationCreated.CrabLocationId.Should().BeNullOrEmpty();
            organisationCreated.FormattedAddress.Should().Be("Waregemsestraat, 8999 Evergem, Belgie");
        }

        [Fact(Skip = "Under construction")]
        public void UpdatesTheOrganisationInfoFromKbo()
        {
            var organisationCoupledWithKbo = PublishedEvents[1].UnwrapBody<OrganisationInfoUpdatedFromKbo>();
            organisationCoupledWithKbo.Should().NotBeNull();

            organisationCoupledWithKbo.OrganisationId.Should().Be(_organisationId);
            organisationCoupledWithKbo.Name.Should().Be("NAME FROM KBO");
            organisationCoupledWithKbo.ShortName.Should().Be("SHORT NAME FROM KBO");
        }

        [Fact(Skip = "Under construction")]
        public void UpdatesTheLocations()
        {
            var organisationLocationAdded = PublishedEvents[2].UnwrapBody<KboRegisteredOfficeOrganisationLocationAdded>();
            organisationLocationAdded.Should().NotBeNull();

            organisationLocationAdded.OrganisationId.Should().Be(_organisationId);
            organisationLocationAdded.OrganisationLocationId.Should().NotBeEmpty();
            organisationLocationAdded.LocationId.Should().Be(PublishedEvents[0].UnwrapBody<LocationCreated>().LocationId);
            organisationLocationAdded.IsMainLocation.Should().BeFalse();
            organisationLocationAdded.LocationFormattedAddress.Should().Be("Waregemsestraat, 8999 Evergem, Belgie");
            organisationLocationAdded.LocationTypeId.Should().Be(_organisationRegistryConfigurationStub.KboV2RegisteredOfficeLocationTypeId);
            organisationLocationAdded.LocationTypeName.Should().Be("Registered KBO Office");
            organisationLocationAdded.ValidFrom.Should().Be(new ValidFrom(2019, 9, 9));
            organisationLocationAdded.ValidTo.Should().Be(null);

            var organisationLocationUpdated = PublishedEvents[3].UnwrapBody<KboRegisteredOfficeOrganisationLocationEnded>();
            organisationLocationUpdated.Should().NotBeNull();

            organisationLocationUpdated.OrganisationId.Should().Be(_organisationId);
            organisationLocationUpdated.OrganisationLocationId.Should().NotBeEmpty();
            organisationLocationUpdated.LocationId.Should().Be(_registeredOfficeLocationToRemoveId);
            organisationLocationUpdated.IsMainLocation.Should().BeFalse();
            organisationLocationUpdated.LocationFormattedAddress.Should().Be("Derbylaan, 8881 Adinkerke, Belgie");
            organisationLocationUpdated.LocationTypeId.Should().Be(_organisationRegistryConfigurationStub.KboV2RegisteredOfficeLocationTypeId);
            organisationLocationUpdated.LocationTypeName.Should().Be("Registered KBO Office");
            organisationLocationUpdated.ValidFrom.Should().Be(new ValidFrom(1999, 12, 31));
            organisationLocationUpdated.ValidTo.Should().Be(new ValidTo(2019, 9, 8));
        }

        [Fact(Skip = "Under construction")]
        public void UpdatesTheFormalNameLabel()
        {
            var kboFormalNameLabelEnded = PublishedEvents[4].UnwrapBody<KboFormalNameLabelEnded>();
            kboFormalNameLabelEnded.Should().NotBeNull();

            kboFormalNameLabelEnded.OrganisationId.Should().Be(_organisationId);
            kboFormalNameLabelEnded.OrganisationLabelId.Should().NotBeEmpty();
            kboFormalNameLabelEnded.LabelTypeId.Should().Be(_organisationRegistryConfigurationStub.KboV2FormalNameLabelTypeId);
            kboFormalNameLabelEnded.LabelTypeName.Should().Be("Kbo formele naam");
            kboFormalNameLabelEnded.Value.Should().Be("Formele naam organisatie");
            kboFormalNameLabelEnded.ValidFrom.Should().Be(new ValidFrom(new DateTime(2008, 12, 22)));
            kboFormalNameLabelEnded.ValidTo.Should().Be(new ValidTo(new DateTime(2009, 1, 1)));

            var kboFormalNameLabelAdded = PublishedEvents[5].UnwrapBody<KboFormalNameLabelAdded>();
            kboFormalNameLabelAdded.Should().NotBeNull();

            kboFormalNameLabelAdded.OrganisationId.Should().Be(_organisationId);
            kboFormalNameLabelAdded.OrganisationLabelId.Should().NotBeEmpty().And.Should().NotBe(kboFormalNameLabelEnded.OrganisationLabelId);
            kboFormalNameLabelAdded.LabelTypeId.Should().Be(_organisationRegistryConfigurationStub.KboV2FormalNameLabelTypeId);
            kboFormalNameLabelAdded.LabelTypeName.Should().Be("Kbo formele naam");
            kboFormalNameLabelAdded.Value.Should().Be("NAME FROM KBO");
            kboFormalNameLabelAdded.ValidFrom.Should().Be(new ValidFrom(new DateTime(2009, 1, 1)));
            kboFormalNameLabelAdded.ValidTo.Should().Be(new ValidTo());
        }

        [Fact(Skip = "Under construction")]
        public void UpdatesLegalForms()
        {
            var legalFormOrganisationOrganisationClassificationAdded = PublishedEvents[6].UnwrapBody<KboLegalFormOrganisationOrganisationClassificationAdded>();
            legalFormOrganisationOrganisationClassificationAdded.Should().NotBeNull();

            legalFormOrganisationOrganisationClassificationAdded.OrganisationId.Should().Be(_organisationId);
            legalFormOrganisationOrganisationClassificationAdded.OrganisationOrganisationClassificationId.Should().NotBeEmpty();
            legalFormOrganisationOrganisationClassificationAdded.OrganisationClassificationTypeId.Should().Be(_legalFormOrganisationClassificationTypeId);
            legalFormOrganisationOrganisationClassificationAdded.OrganisationClassificationTypeName.Should().Be("ClassificatieType");
            legalFormOrganisationOrganisationClassificationAdded.OrganisationClassificationId.Should().Be(_anotherOrganisationClassificationId);
            legalFormOrganisationOrganisationClassificationAdded.OrganisationClassificationName.Should().Be("Classificatie");
            legalFormOrganisationOrganisationClassificationAdded.ValidFrom.Should().Be(new ValidFrom(2019, 9, 9));
            legalFormOrganisationOrganisationClassificationAdded.ValidTo.Should().Be(null);

            var legalFormOrganisationOrganisationClassificationEnded = PublishedEvents[7].UnwrapBody<KboLegalFormOrganisationOrganisationClassificationEnded>();
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

        [Fact(Skip = "Under construction")]
        public void AddsBankAccounts()
        {
            var organisationBankAccountAdded = PublishedEvents[7].UnwrapBody<KboOrganisationBankAccountAdded>();
            organisationBankAccountAdded.Should().NotBeNull();

            organisationBankAccountAdded.OrganisationId.Should().Be(_organisationId);
            organisationBankAccountAdded.OrganisationBankAccountId.Should().NotBeEmpty();
            organisationBankAccountAdded.BankAccountNumber.Should().Be("BE71096123456769");
            organisationBankAccountAdded.Bic.Should().Be("GKCCBEBB");
            organisationBankAccountAdded.IsIban.Should().Be(true);
            organisationBankAccountAdded.IsBic.Should().Be(true);
            organisationBankAccountAdded.ValidFrom.Should().Be(new ValidFrom(2000, 1, 1));
            organisationBankAccountAdded.ValidTo.Should().Be(new ValidTo(2001, 1, 1));
        }

        public UpdateFromKboTests(ITestOutputHelper helper) : base(helper)
        {
        }
    }
}
