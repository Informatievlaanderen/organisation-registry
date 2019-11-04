namespace OrganisationRegistry.Organisation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using KeyTypes;
    using LabelType;
    using Location;
    using Location.Commands;
    using LocationType;
    using Microsoft.Extensions.Logging;
    using OrganisationClassification;
    using OrganisationClassificationType;
    using Purpose;

    public class KboOrganisationCommandHandlers :
        BaseCommandHandler<KboOrganisationCommandHandlers>,
        ICommandHandler<CreateKboOrganisation>,
        ICommandHandler<CoupleOrganisationToKbo>
    {
        private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;
        private readonly IOvoNumberGenerator _ovoNumberGenerator;
        private readonly IUniqueOvoNumberValidator _uniqueOvoNumberValidator;
        private readonly IUniqueKboValidator _uniqueKboValidator;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IKboOrganisationRetriever _kboOrganisationRetriever;
        private readonly IKboOrganisationClassificationRetriever _organisationClassificationRetriever;
        private readonly IKboLocationRetriever _locationRetriever;

        public KboOrganisationCommandHandlers(
            ILogger<KboOrganisationCommandHandlers> logger,
            IOrganisationRegistryConfiguration organisationRegistryConfiguration,
            ISession session,
            IOvoNumberGenerator ovoNumberGenerator,
            IUniqueOvoNumberValidator uniqueOvoNumberValidator,
            IUniqueKboValidator uniqueKboValidator,
            IDateTimeProvider dateTimeProvider,
            IKboOrganisationRetriever kboOrganisationRetriever,
            IKboOrganisationClassificationRetriever organisationClassificationRetriever,
            IKboLocationRetriever locationRetriever) : base(logger, session)
        {
            _organisationRegistryConfiguration = organisationRegistryConfiguration;
            _ovoNumberGenerator = ovoNumberGenerator;
            _uniqueOvoNumberValidator = uniqueOvoNumberValidator;
            _uniqueKboValidator = uniqueKboValidator;
            _dateTimeProvider = dateTimeProvider;
            _kboOrganisationRetriever = kboOrganisationRetriever;
            _organisationClassificationRetriever = organisationClassificationRetriever;
            _locationRetriever = locationRetriever;
        }

        public void Handle(CreateKboOrganisation message)
        {
            if (_uniqueOvoNumberValidator.IsOvoNumberTaken(message.OvoNumber))
                throw new OvoNumberNotUniqueException();

            var ovoNumber = string.IsNullOrWhiteSpace(message.OvoNumber)
                ? _ovoNumberGenerator.GenerateNumber()
                : message.OvoNumber;

            var kboOrganisation =
                _kboOrganisationRetriever.RetrieveOrganisation(message.User, message.KboNumber).Result;

            if (kboOrganisation == null)
                throw new KboOrganisationNotFoundException();

            if (_uniqueKboValidator.IsKboNumberTaken(message.KboNumber, kboOrganisation.ValidFrom, message.ValidTo))
                throw new KboNumberNotUniqueException();

            var parentOrganisation =
                message.ParentOrganisationId != null
                    ? Session.Get<Organisation>(message.ParentOrganisationId)
                    : null;

            var purposes = message
                .Purposes
                .Select(purposeId => Session.Get<Purpose>(purposeId))
                .ToList();

            var locations = CreateLocations(kboOrganisation.Addresses);

            Session.Commit();

            var organisation = Organisation.CreateFromKbo(
                message,
                kboOrganisation,
                ovoNumber,
                parentOrganisation,
                purposes,
                _dateTimeProvider);

            Session.Add(organisation);

            AddKey(organisation, kboOrganisation, message.KboNumber, message.ValidTo);

            AddBankAccounts(organisation, kboOrganisation.BankAccounts);

            AddLegalForms(organisation, kboOrganisation.LegalForms);

            AddAddresses(organisation, locations, kboOrganisation.Addresses);

            AddLabel(organisation, kboOrganisation);

            Session.Commit();
        }

        public void Handle(CoupleOrganisationToKbo message)
        {
            var kboOrganisation =
                _kboOrganisationRetriever.RetrieveOrganisation(message.User, message.KboNumber).Result;

            if (_uniqueKboValidator.IsKboNumberTaken(message.KboNumber, kboOrganisation.ValidFrom, new ValidTo()))
                throw new KboNumberNotUniqueException();

            var locations = CreateLocations(kboOrganisation.Addresses);

            Session.Commit();

            var organisation = Session.Get<Organisation>(message.OrganisationId);

            // TODO: ask thomas: is this ValidFrom correct?
            organisation.CoupleToKbo(message.KboNumber, _dateTimeProvider);

            organisation.UpdateInfoFromKbo(kboOrganisation.Name, kboOrganisation.ShortName);

            // TODO: ask thomas: is this ValidTo correct? And if so, is the one in CreateFromKbo correct?
            AddKey(organisation, kboOrganisation, message.KboNumber, new ValidTo());

            AddBankAccounts(organisation, kboOrganisation.BankAccounts);

            AddLegalForms(organisation, kboOrganisation.LegalForms);

            AddAddresses(organisation, locations, kboOrganisation.Addresses);

            AddLabel(organisation, kboOrganisation);

            Session.Commit();
        }

        private Dictionary<MagdaAddressWithoutValidity, Location> CreateLocations(IEnumerable<IMagdaAddress> kboOrganisationAddresses)
        {
            var locations = new Dictionary<MagdaAddressWithoutValidity, Location>();

            foreach (var address in kboOrganisationAddresses)
            {
                var addressWithoutValidity = new MagdaAddressWithoutValidity(address);
                if (locations.ContainsKey(addressWithoutValidity))
                    continue;

                var existingLocationId = _locationRetriever.RetrieveLocation(address);

                if (!existingLocationId.HasValue)
                {
                    var location = new Location(new LocationId(Guid.NewGuid()), null, new Address(
                        address.Street, address.ZipCode, address.City, address.Country));

                    Session.Add(location);
                    locations[addressWithoutValidity] = location;
                }

                if (existingLocationId.HasValue)
                {
                    locations[addressWithoutValidity] = Session.Get<Location>(existingLocationId.Value);
                }
            }

            return locations;
        }

        private void AddKey(
            Organisation organisation,
            IMagdaOrganisationResponse kboOrganisation,
            KboNumber kboNumber,
            ValidTo validTo)
        {
            organisation.AddKey(
                Guid.NewGuid(),
                Session.Get<KeyType>(_organisationRegistryConfiguration.KboKeyTypeId),
                kboNumber.ToDigitsOnly(),
                new Period(new ValidFrom(kboOrganisation.ValidFrom), validTo));
        }

        private void AddLabel(Organisation organisation, IMagdaOrganisationResponse kboOrganisation)
        {
            var labelType = Session.Get<LabelType>(_organisationRegistryConfiguration.KboV2FormalNameLabelTypeId);

            organisation.AddKboFormalNameLabel(
                Guid.NewGuid(),
                labelType,
                kboOrganisation.Name,
                new Period(
                    new ValidFrom(kboOrganisation.ValidFrom),
                    new ValidTo()));
        }

        private void AddAddresses(Organisation organisation,
            IReadOnlyDictionary<MagdaAddressWithoutValidity, Location> locations, IEnumerable<IMagdaAddress> addresses)
        {
            foreach (var address in addresses)
            {
                var registeredOfficeLocationType =
                    Session.Get<LocationType>(_organisationRegistryConfiguration.KboV2RegisteredOfficeLocationTypeId);

                var addressWithoutValidity = new MagdaAddressWithoutValidity(address);

                if (!locations.ContainsKey(addressWithoutValidity))
                    throw new LocationNotFoundException(
                        $"{address.Street}, {address.ZipCode} {address.City}, {address.Country}");

                var existingLocation = locations[addressWithoutValidity];

                organisation.AddKboRegisteredOfficeLocation(
                    Guid.NewGuid(),
                    existingLocation,
                    registeredOfficeLocationType,
                    new Period(
                        new ValidFrom(address.ValidFrom),
                        new ValidTo(address.ValidTo)),
                    _dateTimeProvider);
            }
        }

        private void AddLegalForms(Organisation organisation, IEnumerable<IMagdaLegalForm> legalForms)
        {
            foreach (var legalForm in legalForms)
            {
                var organisationClassificationId =
                    _organisationClassificationRetriever
                        .FetchOrganisationClassificationForLegalFormCode(legalForm.Code);

                if (organisationClassificationId == null)
                    continue;

                var organisationClassification =
                    Session.Get<OrganisationClassification>(organisationClassificationId.Value);
                var organisationClassificationType =
                    Session.Get<OrganisationClassificationType>(organisationClassification
                        .OrganisationClassificationTypeId);

                organisation.AddKboLegalFormOrganisationClassification(
                    Guid.NewGuid(),
                    organisationClassificationType,
                    organisationClassification,
                    new Period(
                        new ValidFrom(legalForm.ValidFrom),
                        new ValidTo(legalForm.ValidTo)));
            }
        }

        private static void AddBankAccounts(Organisation organisation, IEnumerable<IMagdaBankAccount> bankAccounts)
        {
            foreach (var bankAccount in bankAccounts)
            {
                organisation.AddKboBankAccount(
                    Guid.NewGuid(),
                    new BankAccountNumber(bankAccount.Iban, true),
                    new BankAccountBic(bankAccount.Bic, true),
                    new Period(
                        new ValidFrom(bankAccount.ValidFrom),
                        new ValidTo(bankAccount.ValidTo)));
            }
        }
    }
}
