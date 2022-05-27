namespace OrganisationRegistry.UnitTests.Organisation.Kbo
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using OrganisationRegistry.KeyTypes.Events;
    using LabelType.Events;
    using Location.Events;
    using LocationType.Events;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationClassification.Events;
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
    using Purpose = OrganisationRegistry.Organisation.Events.Purpose;

    public class CoupleOrganisationToKboTests: Specification<KboOrganisationCommandHandlers, CoupleOrganisationToKbo>
    {
        private readonly OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub= new()
        {
            KboKeyTypeId = Guid.NewGuid(),
            Kbo = new KboConfigurationStub
            {
                KboV2LegalFormOrganisationClassificationTypeId = Guid.NewGuid(),
                KboV2RegisteredOfficeLocationTypeId = Guid.NewGuid(),
                KboV2FormalNameLabelTypeId = Guid.NewGuid(),
            }
        };

        private readonly Guid _organisationId;
        private readonly Guid _legalFormOrganisationClassificationTypeId;
        private readonly Guid _organisationClassificationId;
        private readonly Guid _registeredOfficeLocationId;
        private readonly DateTime _kboOrganisationValidFromDate = new(2000, 12, 31);
        private readonly DateTimeProviderStub _dateTimeProviderStub= new(DateTime.Today);

        public CoupleOrganisationToKboTests(ITestOutputHelper helper) : base(helper)
        {
            _organisationId = Guid.NewGuid();
            _registeredOfficeLocationId = Guid.NewGuid();
            _legalFormOrganisationClassificationTypeId = _organisationRegistryConfigurationStub.Kbo.KboV2LegalFormOrganisationClassificationTypeId;
            _organisationClassificationId = Guid.NewGuid();
        }

        private IEvent[] Events
            => new IEvent[]
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

        private CoupleOrganisationToKbo CoupleOrganisationToKboCommand
            => new(
                new OrganisationId(_organisationId),
                new KboNumber("BE0123456789"));

        protected override KboOrganisationCommandHandlers BuildHandler(ISession session)
            => new(
                new Mock<ILogger<KboOrganisationCommandHandlers>>().Object,
                _organisationRegistryConfigurationStub,
                session,
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
                new KboLocationRetrieverStub(address => address.Street == "Waregemsestraat" ? _registeredOfficeLocationId : null));

        [Fact]
        public async Task CreatesTheMissingLocationsOnceBeforeCreatingTheOrganisation()
        {
            await Given(Events)
                .When(CoupleOrganisationToKboCommand, TestUser.User)
                .Then();

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
        public async Task CouplesTheOrganisationWithTheKboNumber()
        {
            await Given(Events)
                .When(CoupleOrganisationToKboCommand, TestUser.User)
                .Then();

            var organisationCoupledWithKbo = PublishedEvents[1].UnwrapBody<OrganisationCoupledWithKbo>();
            organisationCoupledWithKbo.Should().NotBeNull();

            organisationCoupledWithKbo.OrganisationId.Should().Be(_organisationId);
            organisationCoupledWithKbo.OvoNumber.Should().Be("OVO001234");
            organisationCoupledWithKbo.Name.Should().Be("organisation X");
            organisationCoupledWithKbo.KboNumber.Should().Be("0123456789");
            organisationCoupledWithKbo.ValidFrom.Should().Be(new ValidFrom(_dateTimeProviderStub.Today));
        }

        [Fact]
        public async Task UpdatesTheOrganisationInfoFromKbo()
        {
            await Given(Events)
                .When(CoupleOrganisationToKboCommand, TestUser.User)
                .Then();

            var organisationCoupledWithKbo = PublishedEvents[2].UnwrapBody<OrganisationInfoUpdatedFromKbo>();
            organisationCoupledWithKbo.Should().NotBeNull();

            organisationCoupledWithKbo.OrganisationId.Should().Be(_organisationId);
            organisationCoupledWithKbo.Name.Should().Be("NAME FROM KBO");
            organisationCoupledWithKbo.ShortName.Should().Be("SHORT NAME FROM KBO");
        }

        [Fact]
        public async Task AddsBankAccounts()
        {
            await Given(Events)
                .When(CoupleOrganisationToKboCommand, TestUser.User)
                .Then();

            var organisationBankAccountAdded = PublishedEvents[3].UnwrapBody<KboOrganisationBankAccountAdded>();
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

        [Fact]
        public async Task AddsLegalForms()
        {
            await Given(Events)
                .When(CoupleOrganisationToKboCommand, TestUser.User)
                .Then();

            var organisationClassificationAdded = PublishedEvents[4].UnwrapBody<KboLegalFormOrganisationOrganisationClassificationAdded>();
            organisationClassificationAdded.Should().NotBeNull();

            organisationClassificationAdded.OrganisationId.Should().Be(_organisationId);
            organisationClassificationAdded.OrganisationOrganisationClassificationId.Should().NotBeEmpty();
            organisationClassificationAdded.OrganisationClassificationTypeId.Should().Be(_legalFormOrganisationClassificationTypeId);
            organisationClassificationAdded.OrganisationClassificationTypeName.Should().Be("ClassificatieType");
            organisationClassificationAdded.OrganisationClassificationId.Should().Be(_organisationClassificationId);
            organisationClassificationAdded.OrganisationClassificationName.Should().Be("Classificatie");
            organisationClassificationAdded.ValidFrom.Should().Be(new ValidFrom(2020, 12, 11));
            organisationClassificationAdded.ValidTo.Should().Be(new ValidTo(null));
        }

        [Fact]
        public async Task AddsLocations()
        {
            await Given(Events)
                .When(CoupleOrganisationToKboCommand, TestUser.User)
                .Then();

            var organisationLocationAdded = PublishedEvents[5].UnwrapBody<KboRegisteredOfficeOrganisationLocationAdded>();
            organisationLocationAdded.Should().NotBeNull();

            organisationLocationAdded.OrganisationId.Should().Be(_organisationId);
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
        public async Task AddsFormalNameLabel()
        {
            await Given(Events)
                .When(CoupleOrganisationToKboCommand, TestUser.User)
                .Then();

            var organisationLabelAdded = PublishedEvents[6].UnwrapBody<KboFormalNameLabelAdded>();
            organisationLabelAdded.Should().NotBeNull();

            organisationLabelAdded.OrganisationId.Should().Be(_organisationId);
            organisationLabelAdded.OrganisationLabelId.Should().NotBeEmpty();
            organisationLabelAdded.LabelTypeId.Should().Be(_organisationRegistryConfigurationStub.Kbo.KboV2FormalNameLabelTypeId);
            organisationLabelAdded.LabelTypeName.Should().Be("KBO formele naam");
            organisationLabelAdded.Value.Should().Be("NAME FROM KBO");
            organisationLabelAdded.ValidFrom.Should().Be(new ValidFrom(_kboOrganisationValidFromDate));
            organisationLabelAdded.ValidTo.Should().Be(new ValidTo());
        }

        [Fact]
        public async Task PublishesTheCorrectNumberOfEvents()
            => await Given(Events)
                .When(CoupleOrganisationToKboCommand, TestUser.User)
                .ThenItPublishesTheCorrectNumberOfEvents(7);
    }
}
