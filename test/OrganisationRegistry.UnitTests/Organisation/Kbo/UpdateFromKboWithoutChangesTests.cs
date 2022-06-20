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

public class
    UpdateFromKboWithoutChangesTests : Specification<KboOrganisationCommandHandlers, SyncOrganisationWithKbo>
{
    private readonly OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub;

    private readonly Guid _organisationId;
    private readonly Guid _kboSyncItemId;
    private readonly Guid _legalFormOrganisationClassificationTypeId;
    private readonly Guid _organisationClassificationId;
    private readonly Guid _registeredOfficeLocationId;
    private readonly string _kboNumber;
    private readonly DateTimeProviderStub _dateTimeProviderStub;

    public UpdateFromKboWithoutChangesTests(ITestOutputHelper helper) : base(helper)
    {
        _organisationRegistryConfigurationStub = new OrganisationRegistryConfigurationStub
        {
            KboKeyTypeId = Guid.NewGuid(),
            Kbo = new KboConfigurationStub
            {
                KboV2LegalFormOrganisationClassificationTypeId = Guid.NewGuid(),
                KboV2RegisteredOfficeLocationTypeId = Guid.NewGuid(),
                KboV2FormalNameLabelTypeId = Guid.NewGuid(),
            },
        };
        _dateTimeProviderStub = new DateTimeProviderStub(new DateTime(2019, 9, 20));

        _organisationId = Guid.NewGuid();
        _registeredOfficeLocationId = Guid.NewGuid();
        _legalFormOrganisationClassificationTypeId = _organisationRegistryConfigurationStub.Kbo
            .KboV2LegalFormOrganisationClassificationTypeId;
        _organisationClassificationId = Guid.NewGuid();
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
            new LocationTypeCreated(
                _organisationRegistryConfigurationStub.Kbo.KboV2RegisteredOfficeLocationTypeId,
                "Registered KBO Office"),
            new LocationCreated(
                _registeredOfficeLocationId,
                null,
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
                "NAME FROM KBO",
                "OVO001234",
                "SHORT NAME FROM KBO",
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
                _registeredOfficeLocationId,
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
                "NAME FROM KBO",
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
                "GKCCBEBB",
                true,
                new DateTime(2000, 1, 1),
                new DateTime(2001, 1, 1)),
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
                            AccountNumber = "BE71 0961 9876 6769",
                            Bic = "GKCCBEBB",
                            ValidFrom = new DateTime(2000, 1, 1),
                            ValidTo = new DateTime(2001, 1, 1),
                        },
                    },
                    LegalForm =
                        new LegalFormStub
                        {
                            Code = "Some Legal Code",
                            ValidFrom = new DateTime(2020, 12, 11),
                            ValidTo = new DateTime(2020, 12, 12),
                        },
                    Address =
                        new AddressStub
                        {
                            City = "Adinkerke",
                            Street = "Derbylaan",
                            Country = "Belgie",
                            ZipCode = "8881",
                            ValidFrom = new DateTime(1999, 12, 31),
                            ValidTo = null,
                        },
                }),
            organisationClassificationRetriever: new KboOrganisationClassificationRetrieverStub(
                "Some Legal Code",
                _organisationClassificationId),
            locationRetriever: new KboLocationRetrieverStub(_ => _registeredOfficeLocationId));

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(Events).When(SyncOrganisationWithKboCommand, TestUser.User)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task MarksAsSynced()
    {
        await Given(Events).When(SyncOrganisationWithKboCommand, TestUser.User).Then();

        var organisationSyncedFromKbo = PublishedEvents[0].UnwrapBody<OrganisationSyncedFromKbo>();
        organisationSyncedFromKbo.Should().NotBeNull();

        organisationSyncedFromKbo.OrganisationId.Should().Be(_organisationId);
        organisationSyncedFromKbo.KBOSyncItemId.Should().Be(_kboSyncItemId);
    }
}
