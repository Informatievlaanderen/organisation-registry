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
    using Tests.Shared;
    using Tests.Shared.Stubs;
    using Xunit;
    using Xunit.Abstractions;
    using Purpose = OrganisationRegistry.Organisation.Events.Purpose;

    public class CoupleOrganisationToKboTests: Specification<Organisation, KboOrganisationCommandHandlers, CoupleOrganisationToKbo>
    {
        private OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub;

        private OrganisationId _organisationId;
        private OrganisationClassificationTypeId _legalFormOrganisationClassificationTypeId;
        private OrganisationClassificationId _organisationClassificationId;
        private LocationId _registeredOfficeLocationId;
        private readonly DateTime _kboOrganisationValidFromDate = new DateTime(2000, 12, 31);
        private DateTimeProviderStub _dateTimeProviderStub;

        protected override IEnumerable<IEvent> Given()
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
            _organisationId = new OrganisationId(Guid.NewGuid());
            _registeredOfficeLocationId = new LocationId(Guid.NewGuid());
            _legalFormOrganisationClassificationTypeId = new OrganisationClassificationTypeId(_organisationRegistryConfigurationStub.Kbo.KboV2LegalFormOrganisationClassificationTypeId);
            _organisationClassificationId = new OrganisationClassificationId(Guid.NewGuid());

            return new List<IEvent>
            {
                new OrganisationCreated(
                    _organisationId,
                    "organisation X",
                    "OVO001234",
                    "org", Article.None, "", new List<Purpose>(), false, new ValidFrom(), new ValidTo(),
                    new ValidFrom(),
                    new ValidTo()),

                new KeyTypeCreated(
                    _organisationRegistryConfigurationStub.KboKeyTypeId,
                    "KBO sleutel"),

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

                new LocationTypeCreated(
                    _organisationRegistryConfigurationStub.Kbo.KboV2RegisteredOfficeLocationTypeId,
                    "Registered KBO Office"),

                new LocationCreated(_registeredOfficeLocationId,
                    null,
                    "Waregemsestraat, 8999 Evergem, Belgie",
                    "Waregemsestraat",
                    "8999",
                    "Evergem",
                    "Belgie"),

                new LabelTypeCreated(
                    _organisationRegistryConfigurationStub.Kbo.KboV2FormalNameLabelTypeId,
                    "KBO formele naam")
            };
        }

        protected override CoupleOrganisationToKbo When()
        {
            return new CoupleOrganisationToKbo(
                _organisationId,
                new KboNumber("BE0123456789"));
        }

        protected override KboOrganisationCommandHandlers BuildHandler()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Today);

            return new KboOrganisationCommandHandlers(
                new Mock<ILogger<KboOrganisationCommandHandlers>>().Object,
                _organisationRegistryConfigurationStub,
                Session,
                new SequentialOvoNumberGenerator(),
                new UniqueOvoNumberValidatorStub(false),
                new UniqueKboNumberValidatorStub(false),
                _dateTimeProviderStub,
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
                    LegalForm =
                        new LegalFormStub
                        {
                            Code = "Some Legal Code",
                            ValidFrom = new DateTime(2020, 12, 11),
                            ValidTo = new DateTime(2020, 12, 12)
                        },
                    Address =
                        new AddressStub
                        {
                            City = "Evergem",
                            Street = "Zedelgemsestraat",
                            Country = "Belgie",
                            ZipCode = "9999",
                            ValidFrom = new DateTime(2015, 5, 5),
                            ValidTo = new DateTime(2016, 6, 6)
                        }
                }),
                new KboOrganisationClassificationRetrieverStub("Some Legal Code", _organisationClassificationId),
                new KboLocationRetrieverStub(address => address.Street == "Waregemsestraat" ? _registeredOfficeLocationId : (Guid?)null));
        }

        protected override int ExpectedNumberOfEvents => 7;

        [Fact]
        public void CreatesTheMissingLocationsOnceBeforeCreatingTheOrganisation()
        {
            var organisationCreated = PublishedEvents[0].UnwrapBody<LocationCreated>();
            organisationCreated.Should().NotBeNull();

            organisationCreated.LocationId.Should().NotBeEmpty();
            organisationCreated.Street.Should().Be("Zedelgemsestraat");
            organisationCreated.City.Should().Be("Evergem");
            organisationCreated.ZipCode.Should().Be("9999");
            organisationCreated.Country.Should().Be("Belgie");
            organisationCreated.CrabLocationId.Should().BeNullOrEmpty();
            organisationCreated.FormattedAddress.Should().Be("Zedelgemsestraat, 9999 Evergem, Belgie");
        }

        [Fact]
        public void CouplesTheOrganisationWithTheKboNumber()
        {
            var organisationCoupledWithKbo = PublishedEvents[1].UnwrapBody<OrganisationCoupledWithKbo>();
            organisationCoupledWithKbo.Should().NotBeNull();

            organisationCoupledWithKbo.OrganisationId.Should().Be((Guid)_organisationId);
            organisationCoupledWithKbo.OvoNumber.Should().Be("OVO001234");
            organisationCoupledWithKbo.Name.Should().Be("organisation X");
            organisationCoupledWithKbo.KboNumber.Should().Be("0123456789");
            organisationCoupledWithKbo.ValidFrom.Should().Be(new ValidFrom(_dateTimeProviderStub.Today));
        }

        [Fact]
        public void UpdatesTheOrganisationInfoFromKbo()
        {
            var organisationCoupledWithKbo = PublishedEvents[2].UnwrapBody<OrganisationInfoUpdatedFromKbo>();
            organisationCoupledWithKbo.Should().NotBeNull();

            organisationCoupledWithKbo.OrganisationId.Should().Be((Guid)_organisationId);
            organisationCoupledWithKbo.Name.Should().Be("NAME FROM KBO");
            organisationCoupledWithKbo.ShortName.Should().Be("SHORT NAME FROM KBO");
        }

        [Fact]
        public void AddsBankAccounts()
        {
            var organisationBankAccountAdded = PublishedEvents[3].UnwrapBody<KboOrganisationBankAccountAdded>();
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
        public void AddsLegalForms()
        {
            var organisationClassificationAdded = PublishedEvents[4].UnwrapBody<KboLegalFormOrganisationOrganisationClassificationAdded>();
            organisationClassificationAdded.Should().NotBeNull();

            organisationClassificationAdded.OrganisationId.Should().Be((Guid)_organisationId);
            organisationClassificationAdded.OrganisationOrganisationClassificationId.Should().NotBeEmpty();
            organisationClassificationAdded.OrganisationClassificationTypeId.Should().Be((Guid)_legalFormOrganisationClassificationTypeId);
            organisationClassificationAdded.OrganisationClassificationTypeName.Should().Be("ClassificatieType");
            organisationClassificationAdded.OrganisationClassificationId.Should().Be((Guid)_organisationClassificationId);
            organisationClassificationAdded.OrganisationClassificationName.Should().Be("Classificatie");
            organisationClassificationAdded.ValidFrom.Should().Be(new ValidFrom(2020, 12, 11));
            organisationClassificationAdded.ValidTo.Should().Be(new ValidTo(null));
        }

        [Fact]
        public void AddsLocations()
        {
            var organisationLocationAdded = PublishedEvents[5].UnwrapBody<KboRegisteredOfficeOrganisationLocationAdded>();
            organisationLocationAdded.Should().NotBeNull();

            organisationLocationAdded.OrganisationId.Should().Be((Guid)_organisationId);
            organisationLocationAdded.OrganisationLocationId.Should().NotBeEmpty();
            organisationLocationAdded.LocationId.Should().Be(PublishedEvents[0].UnwrapBody<LocationCreated>().LocationId);
            organisationLocationAdded.IsMainLocation.Should().BeFalse();
            organisationLocationAdded.LocationFormattedAddress.Should().Be("Zedelgemsestraat, 9999 Evergem, Belgie");
            organisationLocationAdded.LocationTypeId.Should().Be(_organisationRegistryConfigurationStub.Kbo.KboV2RegisteredOfficeLocationTypeId);
            organisationLocationAdded.LocationTypeName.Should().Be("Registered KBO Office");
            organisationLocationAdded.ValidFrom.Should().Be(new ValidFrom(2015, 5, 5));
            organisationLocationAdded.ValidTo.Should().Be(new ValidTo(2016, 6, 6));
        }

        [Fact]
        public void AddsFormalNameLabel()
        {
            var organisationLabelAdded = PublishedEvents[6].UnwrapBody<KboFormalNameLabelAdded>();
            organisationLabelAdded.Should().NotBeNull();

            organisationLabelAdded.OrganisationId.Should().Be((Guid)_organisationId);
            organisationLabelAdded.OrganisationLabelId.Should().NotBeEmpty();
            organisationLabelAdded.LabelTypeId.Should().Be(_organisationRegistryConfigurationStub.Kbo.KboV2FormalNameLabelTypeId);
            organisationLabelAdded.LabelTypeName.Should().Be("KBO formele naam");
            organisationLabelAdded.Value.Should().Be("NAME FROM KBO");
            organisationLabelAdded.ValidFrom.Should().Be(new ValidFrom(_kboOrganisationValidFromDate));
            organisationLabelAdded.ValidTo.Should().Be(new ValidTo());
        }

        public CoupleOrganisationToKboTests(ITestOutputHelper helper) : base(helper) { }
    }
}
