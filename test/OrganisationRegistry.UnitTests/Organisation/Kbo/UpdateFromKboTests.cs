namespace OrganisationRegistry.UnitTests.Organisation.Kbo;

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

public class UpdateFromKboTests : Specification<KboOrganisationCommandHandlers, SyncOrganisationWithKbo>
{
    private readonly OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub;
    private readonly Guid _organisationId;
    private readonly Guid _kboSyncItemId;
    private readonly Guid _legalFormOrganisationClassificationTypeId;
    private readonly Guid _organisationClassificationId;
    private readonly Guid _anotherOrganisationClassificationId;
    private readonly Guid _registeredOfficeLocationToRemoveId;
    private readonly string _kboNumber = "BE0123456789";
    private readonly DateTimeProviderStub _dateTimeProviderStub;

    public UpdateFromKboTests(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(new DateTime(2019, 9, 20));
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
        _organisationId = Guid.NewGuid();
        _registeredOfficeLocationToRemoveId = Guid.NewGuid();
        _legalFormOrganisationClassificationTypeId = _organisationRegistryConfigurationStub.Kbo
            .KboV2LegalFormOrganisationClassificationTypeId;
        _organisationClassificationId = Guid.NewGuid();
        _anotherOrganisationClassificationId = Guid.NewGuid();
        _kboSyncItemId = Guid.NewGuid();
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
            new LocationTypeCreated(
                _organisationRegistryConfigurationStub.Kbo.KboV2RegisteredOfficeLocationTypeId,
                "Registered KBO Office"),
            new LocationCreated(
                _registeredOfficeLocationToRemoveId,
                string.Empty,
                "Derbylaan, 8881 Adinkerke, Belgie",
                "Derbylaan",
                "8881",
                "Adinkerke",
                "Belgie"),
            new LabelTypeCreated(
                _organisationRegistryConfigurationStub.Kbo.KboV2FormalNameLabelTypeId,
                "KBO formele naam"),
            new OrganisationCreatedFromKbo(
                _organisationId,
                new KboNumber(_kboNumber).ToDigitsOnly(),
                "organisation X",
                "OVO001234",
                "org",
                Article.None,
                "",
                new List<Purpose>(),
                false,
                new ValidFrom(),
                new ValidTo(),
                new ValidFrom(),
                new ValidTo()),
            new KboRegisteredOfficeOrganisationLocationAdded(
                _organisationId,
                Guid.NewGuid(),
                _registeredOfficeLocationToRemoveId,
                "Derbylaan, 8881 Adinkerke, Belgie",
                false,
                _organisationRegistryConfigurationStub.Kbo.KboV2RegisteredOfficeLocationTypeId,
                "Registered KBO Office",
                new ValidFrom(1999, 12, 31),
                null),
            new KboFormalNameLabelAdded(
                _organisationId,
                Guid.NewGuid(),
                _organisationRegistryConfigurationStub.Kbo.KboV2FormalNameLabelTypeId,
                "KBO formele naam",
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
                "BE71 0961 2345 6769",
                true,
                "GKCCBEBB",
                true,
                new DateTime(2000, 1, 1),
                new DateTime(2001, 1, 1)),
            new KboOrganisationBankAccountAdded(
                _organisationId,
                Guid.NewGuid(),
                "BE71 0961 9876 6769",
                true,
                "GKCCBABB",
                true,
                new DateTime(2000, 1, 1),
                new DateTime(2001, 1, 1))
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
                    FormalName = new NameStub("NAME FROM KBO", new DateTime(2009, 1, 1)),
                    ShortName = new NameStub("SHORT NAME FROM KBO", new DateTime(2010, 1, 1)),
                    ValidFrom = new DateTime(2000, 12, 31),
                    BankAccounts =
                    {
                        new BankAccountStub
                        {
                            AccountNumber = "BE71 0961 2345 6769",
                            Bic = "GKCCBEBB",
                            ValidFrom = new DateTime(2000, 1, 1),
                            ValidTo = new DateTime(2001, 1, 1),
                        },
                        new BankAccountStub
                        {
                            AccountNumber = "00300000123",
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
                            Street = "AndereStraat",
                            Country = "Belgie",
                            ZipCode = "8999",
                            ValidFrom = null,
                            ValidTo = null
                        }
                }),
            organisationClassificationRetriever: new KboOrganisationClassificationRetrieverStub(
                "Another Legal Code",
                _anotherOrganisationClassificationId),
            locationRetriever: new KboLocationRetrieverStub(_ => null));


    [Fact]
    public async Task PublishesElevenEvents()
    {
        await Given(Events).When(SyncOrganisationWithKboCommand, TestUser.User)
            .ThenItPublishesTheCorrectNumberOfEvents(11);
    }

    [Fact]
    public async Task CreatesTheMissingLocationsOnceBeforeCreatingTheOrganisation()
    {
        await Given(Events).When(SyncOrganisationWithKboCommand, TestUser.User).Then();
        var organisationCreated = PublishedEvents[0]
            .UnwrapBody<LocationCreated>();
        organisationCreated.Should().NotBeNull();

        organisationCreated.LocationId.Should().NotBeEmpty();
        organisationCreated.Street.Should().Be("AndereStraat");
        organisationCreated.City.Should().Be("Evergem");
        organisationCreated.ZipCode.Should().Be("8999");
        organisationCreated.Country.Should().Be("Belgie");
        organisationCreated.CrabLocationId.Should().BeNullOrEmpty();
        organisationCreated.FormattedAddress.Should().Be("AndereStraat, 8999 Evergem, Belgie");
    }

    [Fact]
    public async Task UpdatesTheOrganisationInfoFromKbo()
    {
        await Given(Events).When(SyncOrganisationWithKboCommand, TestUser.User).Then();

        var organisationCoupledWithKbo = PublishedEvents[1].UnwrapBody<OrganisationInfoUpdatedFromKbo>();
        organisationCoupledWithKbo.Should().NotBeNull();

        organisationCoupledWithKbo.OrganisationId.Should().Be(_organisationId);
        organisationCoupledWithKbo.Name.Should().Be("NAME FROM KBO");
        organisationCoupledWithKbo.ShortName.Should().Be("SHORT NAME FROM KBO");
    }

    [Fact]
    public async Task UpdatesTheLocations()
    {
        await Given(Events).When(SyncOrganisationWithKboCommand, TestUser.User).Then();

        var organisationLocationRemoved =
            PublishedEvents[2].UnwrapBody<KboRegisteredOfficeOrganisationLocationRemoved>();
        organisationLocationRemoved.Should().NotBeNull();

        organisationLocationRemoved.OrganisationId.Should().Be(_organisationId);
        organisationLocationRemoved.OrganisationLocationId.Should().NotBeEmpty();
        organisationLocationRemoved.LocationId.Should().Be(_registeredOfficeLocationToRemoveId);
        organisationLocationRemoved.IsMainLocation.Should().BeFalse();
        organisationLocationRemoved.LocationFormattedAddress.Should().Be("Derbylaan, 8881 Adinkerke, Belgie");
        organisationLocationRemoved.LocationTypeId.Should().Be(
            _organisationRegistryConfigurationStub.Kbo.KboV2RegisteredOfficeLocationTypeId);
        organisationLocationRemoved.LocationTypeName.Should().Be("Registered KBO Office");
        organisationLocationRemoved.ValidFrom.Should().Be(new ValidFrom(1999, 12, 31));
        organisationLocationRemoved.ValidTo.Should().Be(new ValidTo(null));

        var organisationLocationAdded =
            PublishedEvents[3].UnwrapBody<KboRegisteredOfficeOrganisationLocationAdded>();
        organisationLocationAdded.Should().NotBeNull();

        organisationLocationAdded.OrganisationId.Should().Be(_organisationId);
        organisationLocationAdded.OrganisationLocationId.Should().NotBeEmpty();
        organisationLocationAdded.LocationId.Should()
            .Be(PublishedEvents[0].UnwrapBody<LocationCreated>().LocationId);
        organisationLocationAdded.IsMainLocation.Should().BeFalse();
        organisationLocationAdded.LocationFormattedAddress.Should().Be("AndereStraat, 8999 Evergem, Belgie");
        organisationLocationAdded.LocationTypeId.Should().Be(
            _organisationRegistryConfigurationStub.Kbo.KboV2RegisteredOfficeLocationTypeId);
        organisationLocationAdded.LocationTypeName.Should().Be("Registered KBO Office");
        organisationLocationAdded.ValidFrom.Should().Be(null);
        organisationLocationAdded.ValidTo.Should().Be(null);
    }

    [Fact]
    public async Task UpdatesTheFormalNameLabel()
    {
        await Given(Events).When(SyncOrganisationWithKboCommand, TestUser.User).Then();

        var kboFormalNameLabelRemoved = PublishedEvents[4].UnwrapBody<KboFormalNameLabelRemoved>();
        kboFormalNameLabelRemoved.Should().NotBeNull();

        kboFormalNameLabelRemoved.OrganisationId.Should().Be(_organisationId);
        kboFormalNameLabelRemoved.OrganisationLabelId.Should().NotBeEmpty();
        kboFormalNameLabelRemoved.LabelTypeId.Should()
            .Be(_organisationRegistryConfigurationStub.Kbo.KboV2FormalNameLabelTypeId);
        kboFormalNameLabelRemoved.LabelTypeName.Should().Be("KBO formele naam");
        kboFormalNameLabelRemoved.Value.Should().Be("Formele naam organisatie");
        kboFormalNameLabelRemoved.ValidFrom.Should().Be(new ValidFrom(new DateTime(2008, 12, 22)));
        kboFormalNameLabelRemoved.ValidTo.Should().Be(new ValidTo(new DateTime(2009, 1, 1)));

        var kboFormalNameLabelAdded = PublishedEvents[5].UnwrapBody<KboFormalNameLabelAdded>();
        kboFormalNameLabelAdded.Should().NotBeNull();

        kboFormalNameLabelAdded.OrganisationId.Should().Be(_organisationId);
        kboFormalNameLabelAdded.OrganisationLabelId.Should().NotBeEmpty().And
            .NotBe(kboFormalNameLabelRemoved.OrganisationLabelId);
        kboFormalNameLabelAdded.LabelTypeId.Should()
            .Be(_organisationRegistryConfigurationStub.Kbo.KboV2FormalNameLabelTypeId);
        kboFormalNameLabelAdded.LabelTypeName.Should().Be("KBO formele naam");
        kboFormalNameLabelAdded.Value.Should().Be("NAME FROM KBO");
        kboFormalNameLabelAdded.ValidFrom.Should().Be(new ValidFrom(new DateTime(2009, 1, 1)));
        kboFormalNameLabelAdded.ValidTo.Should().Be(new ValidTo());
    }

    [Fact]
    public async Task UpdatesLegalForms()
    {
        await Given(Events).When(SyncOrganisationWithKboCommand, TestUser.User).Then();

        var legalFormOrganisationOrganisationClassificationEnded = PublishedEvents[6]
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

        var legalFormOrganisationOrganisationClassificationAdded = PublishedEvents[7]
            .UnwrapBody<KboLegalFormOrganisationOrganisationClassificationAdded>();
        legalFormOrganisationOrganisationClassificationAdded.Should().NotBeNull();

        legalFormOrganisationOrganisationClassificationAdded.OrganisationId.Should().Be(_organisationId);
        legalFormOrganisationOrganisationClassificationAdded.OrganisationOrganisationClassificationId.Should()
            .NotBeEmpty();
        legalFormOrganisationOrganisationClassificationAdded.OrganisationClassificationTypeId.Should()
            .Be(_legalFormOrganisationClassificationTypeId);
        legalFormOrganisationOrganisationClassificationAdded.OrganisationClassificationTypeName.Should()
            .Be("ClassificatieType");
        legalFormOrganisationOrganisationClassificationAdded.OrganisationClassificationId.Should()
            .Be(_anotherOrganisationClassificationId);
        legalFormOrganisationOrganisationClassificationAdded.OrganisationClassificationName.Should()
            .Be("Classificatie");
        legalFormOrganisationOrganisationClassificationAdded.ValidFrom.Should().Be(new ValidFrom(2020, 12, 11));
        legalFormOrganisationOrganisationClassificationAdded.ValidTo.Should().Be(null);
    }

    [Fact]
    public async Task AddsBankAccounts()
    {
        await Given(Events).When(SyncOrganisationWithKboCommand, TestUser.User).Then();
        var organisationBankAccountRemoved = PublishedEvents[8].UnwrapBody<KboOrganisationBankAccountRemoved>();
        organisationBankAccountRemoved.Should().NotBeNull();

        organisationBankAccountRemoved.OrganisationId.Should().Be(_organisationId);
        organisationBankAccountRemoved.OrganisationBankAccountId.Should().NotBeEmpty();
        organisationBankAccountRemoved.BankAccountNumber.Should().Be("BE71 0961 9876 6769");
        organisationBankAccountRemoved.Bic.Should().Be("GKCCBABB");
        organisationBankAccountRemoved.IsIban.Should().Be(true);
        organisationBankAccountRemoved.IsBic.Should().Be(true);
        organisationBankAccountRemoved.ValidFrom.Should().Be(new ValidFrom(2000, 1, 1));
        organisationBankAccountRemoved.ValidTo.Should().Be(new ValidTo(2001, 1, 1));

        var organisationBankAccountAdded = PublishedEvents[9].UnwrapBody<KboOrganisationBankAccountAdded>();
        organisationBankAccountAdded.Should().NotBeNull();

        organisationBankAccountAdded.OrganisationId.Should().Be(_organisationId);
        organisationBankAccountAdded.OrganisationBankAccountId.Should().NotBeEmpty();
        organisationBankAccountAdded.BankAccountNumber.Should().Be("00300000123");
        organisationBankAccountAdded.Bic.Should().Be("");
        organisationBankAccountAdded.IsIban.Should().Be(false);
        organisationBankAccountAdded.IsBic.Should().Be(false);
        organisationBankAccountAdded.ValidFrom.Should().Be(new ValidFrom(2000, 1, 1));
        organisationBankAccountAdded.ValidTo.Should().Be(new ValidTo(2001, 1, 1));
    }

    [Fact]
    public async Task MarksAsSynced()
    {
        await Given(Events).When(SyncOrganisationWithKboCommand, TestUser.User).Then();

        var organisationSyncedFromKbo = PublishedEvents[10].UnwrapBody<OrganisationSyncedFromKbo>();
        organisationSyncedFromKbo.Should().NotBeNull();

        organisationSyncedFromKbo.OrganisationId.Should().Be(_organisationId);
        organisationSyncedFromKbo.KBOSyncItemId.Should().Be(_kboSyncItemId);
    }
}
