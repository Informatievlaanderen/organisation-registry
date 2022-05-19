namespace OrganisationRegistry.UnitTests.Organisation.Kbo
{
    using System;
    using System.Collections.Generic;
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
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Organisation.Exceptions;
    using Tests.Shared;
    using Tests.Shared.Stubs;
    using Xunit;
    using Xunit.Abstractions;
    using Purpose = OrganisationRegistry.Organisation.Events.Purpose;

    public class CoupleWithAlreadyUsedKboNumberTests : OldExceptionSpecification<Organisation,
        KboOrganisationCommandHandlers, CoupleOrganisationToKbo>
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

        private Guid _organisationId;
        private Guid _anotherOrganisationId;
        private Guid _legalFormOrganisationClassificationTypeId;
        private Guid _organisationClassificationId;
        private Guid _registeredOfficeLocationId;
        private readonly DateTime _kboOrganisationValidFromDate = new(2000, 12, 31);
        private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Today);

        public CoupleWithAlreadyUsedKboNumberTests(ITestOutputHelper helper) : base(helper)
        {
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationId = Guid.NewGuid();
            _anotherOrganisationId = Guid.NewGuid();
            _registeredOfficeLocationId = Guid.NewGuid();
            _legalFormOrganisationClassificationTypeId =
                _organisationRegistryConfigurationStub.Kbo.KboV2LegalFormOrganisationClassificationTypeId;
            _organisationClassificationId = Guid.NewGuid();

            return new List<IEvent>
            {
                new OrganisationCreated(
                    _organisationId,
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

                new OrganisationCreated(
                    _anotherOrganisationId,
                    "organisation Y",
                    "OVO001235",
                    "org y",
                    Article.None,
                    "",
                    new List<Purpose>(),
                    false,
                    new ValidFrom(),
                    new ValidTo(),
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

                new LocationCreated(
                    _registeredOfficeLocationId,
                    null,
                    "Waregemsestraat, 8999 Evergem, Belgie",
                    "Waregemsestraat",
                    "8999",
                    "Evergem",
                    "Belgie"),

                new LabelTypeCreated(
                    _organisationRegistryConfigurationStub.Kbo.KboV2FormalNameLabelTypeId,
                    "KBO formele naam"),

                new OrganisationCoupledWithKbo(
                    _organisationId,
                    "BE0123456789",
                    "organisation X",
                    "OVO001234",
                    _dateTimeProviderStub.Today)
            };
        }

        protected override CoupleOrganisationToKbo When()
            => new CoupleOrganisationToKbo(
                new OrganisationId(_anotherOrganisationId),
                new KboNumber("BE0123456789"));

        protected override KboOrganisationCommandHandlers BuildHandler()
        {
            return new KboOrganisationCommandHandlers(
                new Mock<ILogger<KboOrganisationCommandHandlers>>().Object,
                _organisationRegistryConfigurationStub,
                Session,
                new SequentialOvoNumberGenerator(),
                new UniqueOvoNumberValidatorStub(false),
                new UniqueKboNumberValidatorStub(true), // IMPORTANT => kbo number is already taken!
                _dateTimeProviderStub,
                new KboOrganisationRetrieverStub(
                    new MockMagdaOrganisationResponse
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
                new KboLocationRetrieverStub(
                    address => address.Street == "Waregemsestraat" ? _registeredOfficeLocationId : null));
        }

        protected override int ExpectedNumberOfEvents
            => 0;

        [Fact]
        public void ThrowsOrganisationAlreadyCoupledWithKbo()
        {
            Exception.Should().BeOfType<KboNumberNotUnique>();
        }
    }
}
