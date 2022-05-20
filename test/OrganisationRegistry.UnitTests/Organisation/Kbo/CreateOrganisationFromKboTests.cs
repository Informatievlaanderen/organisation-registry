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
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Domain;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;

    using OrganisationRegistry.Organisation.Events;
    using Purpose;
    using Purpose.Events;
    using Tests.Shared;
    using Tests.Shared.Stubs;
    using Xunit;
    using Xunit.Abstractions;
    using Purpose = OrganisationRegistry.Organisation.Events.Purpose;

    public class CreateOrganisationFromKboTests: Specification<KboOrganisationCommandHandlers, CreateOrganisationFromKbo>
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

        private readonly Guid _parentOrganisationId;
        private readonly Guid _organisationId;
        private readonly Guid _purposeId;
        private readonly Guid _organisationClassificationTypeId;
        private readonly Guid _organisationClassificationId;
        private readonly Guid _locationId;
        private readonly DateTime _kboOrganisationValidFromDate = new(2000, 12, 31);

        public CreateOrganisationFromKboTests(ITestOutputHelper helper) : base(helper)
        {
            _parentOrganisationId = Guid.NewGuid();
            _organisationId =  Guid.NewGuid();
            _purposeId =  Guid.NewGuid();
            _locationId =  Guid.NewGuid();
            _organisationClassificationTypeId = _organisationRegistryConfigurationStub.Kbo.KboV2LegalFormOrganisationClassificationTypeId;
            _organisationClassificationId =  Guid.NewGuid();
        }

        private IEvent[] Events
            => new IEvent[]
            {
                new OrganisationCreated(
                    _parentOrganisationId,
                    "parent",
                    "OVO001234",
                    "ouder", Article.None, "", new List<Purpose>(), false, new ValidFrom(), new ValidTo(), new ValidFrom(), new ValidTo()),

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
                    _organisationRegistryConfigurationStub.Kbo.KboV2RegisteredOfficeLocationTypeId,
                    "Registered KBO Office"),

                new LocationCreated(_locationId,
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

        private CreateOrganisationFromKbo CreateOrganisationFromKboCommand
            => new(
                new OrganisationId(_organisationId),
                "Naam",
                "OVO000070",
                "Korte naam",
                Article.None,
                new OrganisationId(_parentOrganisationId),
                "Mijn omschrijving",
                new List<PurposeId> { new(_purposeId) },
                true,
                new ValidFrom(),
                new ValidTo(),
                new KboNumber("BE0123456789"),
                new ValidFrom(), new ValidTo());

        protected override KboOrganisationCommandHandlers BuildHandler(ISession session)
            => new(
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
                new KboLocationRetrieverStub(address => address.Street == "Waregemsestraat" ? _locationId : null));

        [Fact]
        public async Task CreatesTheMissingLocationsOnceBeforeCreatingTheOrganisation()
        {
            await Given(Events)
                .When(CreateOrganisationFromKboCommand, UserBuilder.User())
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
        public async Task CreatesAnOrganisation()
        {
            await Given(Events)
                .When(CreateOrganisationFromKboCommand, UserBuilder.User())
                .Then();

            var organisationCreated = PublishedEvents[1].UnwrapBody<OrganisationCreatedFromKbo>();
            organisationCreated.Should().NotBeNull();

            organisationCreated.OrganisationId.Should().Be(_organisationId);
            organisationCreated.Name.Should().Be("NAME FROM KBO");
            organisationCreated.OvoNumber.Should().Be("OVO000070");
            organisationCreated.ShortName.Should().Be("SHORT NAME FROM KBO");
            organisationCreated.Description.Should().Be("Mijn omschrijving");
            organisationCreated.Purposes.Should().BeEquivalentTo(new List<Purpose> { new(_purposeId, "Purpose X") });
            organisationCreated.ShowOnVlaamseOverheidSites.Should().Be(true);
            organisationCreated.ValidFrom.Should().Be(new ValidFrom(_kboOrganisationValidFromDate));
            organisationCreated.ValidTo.Should().Be(new ValidTo());
        }

        [Fact]
        public async Task TheOrganisationBecomesActive()
        {
            await Given(Events)
                .When(CreateOrganisationFromKboCommand, UserBuilder.User())
                .Then();

            var organisationBecameActive = PublishedEvents[2].UnwrapBody<OrganisationBecameActive>();
            organisationBecameActive.Should().NotBeNull();
        }

        [Fact]
        public async Task TheParentOrganisationWasAdded()
        {
            await Given(Events)
                .When(CreateOrganisationFromKboCommand, UserBuilder.User())
                .Then();

            var parentAdded = PublishedEvents[3].UnwrapBody<OrganisationParentAdded>();
            parentAdded.Should().NotBeNull();
        }

        [Fact]
        public async Task TheParentOrganisationBecameActive()
        {
            await Given(Events)
                .When(CreateOrganisationFromKboCommand, UserBuilder.User())
                .Then();

            var parentAssignedToOrganisation = PublishedEvents[4].UnwrapBody<ParentAssignedToOrganisation>();
            parentAssignedToOrganisation.Should().NotBeNull();
        }

        [Fact]
        public async Task AddsBankAccounts()
        {
            await Given(Events)
                .When(CreateOrganisationFromKboCommand, UserBuilder.User())
                .Then();

            var organisationBankAccountAdded = PublishedEvents[5].UnwrapBody<KboOrganisationBankAccountAdded>();
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
                .When(CreateOrganisationFromKboCommand, UserBuilder.User())
                .Then();

            var organisationClassificationAdded = PublishedEvents[6].UnwrapBody<KboLegalFormOrganisationOrganisationClassificationAdded>();
            organisationClassificationAdded.Should().NotBeNull();

            organisationClassificationAdded.OrganisationId.Should().Be(_organisationId);
            organisationClassificationAdded.OrganisationOrganisationClassificationId.Should().NotBeEmpty();
            organisationClassificationAdded.OrganisationClassificationTypeId.Should().Be(_organisationClassificationTypeId);
            organisationClassificationAdded.OrganisationClassificationTypeName.Should().Be("ClassificatieType");
            organisationClassificationAdded.OrganisationClassificationId.Should().Be(_organisationClassificationId);
            organisationClassificationAdded.OrganisationClassificationName.Should().Be("Classificatie");
            organisationClassificationAdded.ValidFrom.Should().Be(new ValidFrom(2020, 12, 11));
            organisationClassificationAdded.ValidTo.Should().Be(new ValidTo());
        }

        [Fact]
        public async Task AddsLocations()
        {
            await Given(Events)
                .When(CreateOrganisationFromKboCommand, UserBuilder.User())
                .Then();

            var organisationLocationAdded = PublishedEvents[7].UnwrapBody<KboRegisteredOfficeOrganisationLocationAdded>();
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
                .When(CreateOrganisationFromKboCommand, UserBuilder.User())
                .Then();

            var organisationLabelAdded = PublishedEvents[8].UnwrapBody<KboFormalNameLabelAdded>();
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
                .When(CreateOrganisationFromKboCommand, UserBuilder.User())
                .ThenItPublishesTheCorrectNumberOfEvents(9);
    }
}
