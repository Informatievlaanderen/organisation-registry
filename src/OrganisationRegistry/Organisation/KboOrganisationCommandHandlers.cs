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
        ICommandHandler<CoupleOrganisationToKbo>,
        ICommandHandler<UpdateFromKbo>
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
            var registeredOfficeLocationType =
                Session.Get<LocationType>(_organisationRegistryConfiguration.KboV2RegisteredOfficeLocationTypeId);

            var legalFormOrganisationClassificationType = Session.Get<OrganisationClassificationType>(_organisationRegistryConfiguration.KboV2LegalFormOrganisationClassificationTypeId);


            if (_uniqueOvoNumberValidator.IsOvoNumberTaken(message.OvoNumber))
                throw new OvoNumberNotUniqueException();

            var ovoNumber = string.IsNullOrWhiteSpace(message.OvoNumber)
                ? _ovoNumberGenerator.GenerateNumber()
                : message.OvoNumber;

            var kboOrganisation =
                _kboOrganisationRetriever.RetrieveOrganisation(message.User, message.KboNumber).GetAwaiter().GetResult();

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

            var location = GetOrAddLocations(kboOrganisation.Address);

            Session.Commit();

            var organisation = Organisation.CreateFromKbo(
                message,
                kboOrganisation,
                ovoNumber,
                parentOrganisation,
                purposes,
                _dateTimeProvider);

            Session.Add(organisation);

            AddBankAccounts(organisation, kboOrganisation.BankAccounts);

            AddLegalForm(organisation, kboOrganisation.LegalForm, legalFormOrganisationClassificationType);

            AddAddresses(organisation, location, registeredOfficeLocationType);

            AddLabel(organisation, kboOrganisation);

            Session.Commit();
        }

        public void Handle(CoupleOrganisationToKbo message)
        {
            var registeredOfficeLocationType =
                Session.Get<LocationType>(_organisationRegistryConfiguration.KboV2RegisteredOfficeLocationTypeId);

            var legalFormOrganisationClassificationType = Session.Get<OrganisationClassificationType>(_organisationRegistryConfiguration.KboV2LegalFormOrganisationClassificationTypeId);

            var kboOrganisation =
                _kboOrganisationRetriever.RetrieveOrganisation(message.User, message.KboNumber).GetAwaiter().GetResult();

            if (kboOrganisation == null)
                throw new KboOrganisationNotFoundException();

            if (_uniqueKboValidator.IsKboNumberTaken(message.KboNumber, kboOrganisation.ValidFrom, new ValidTo()))
                throw new KboNumberNotUniqueException();

            var location = GetOrAddLocations(kboOrganisation.Address);

            Session.Commit();

            var organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.CoupleToKbo(message.KboNumber, _dateTimeProvider);

            organisation.UpdateInfoFromKbo(kboOrganisation.FormalName.Value, kboOrganisation.ShortName.Value);

            AddBankAccounts(organisation, kboOrganisation.BankAccounts);

            AddLegalForm(organisation, kboOrganisation.LegalForm, legalFormOrganisationClassificationType);

            AddAddresses(organisation, location, registeredOfficeLocationType);

            AddLabel(organisation, kboOrganisation);

            Session.Commit();
        }

        public void Handle(UpdateFromKbo message)
        {
            var registeredOfficeLocationType =
                Session.Get<LocationType>(_organisationRegistryConfiguration.KboV2RegisteredOfficeLocationTypeId);

            var formalNameLabelType = Session.Get<LabelType>(_organisationRegistryConfiguration.KboV2FormalNameLabelTypeId);

            var legalFormOrganisationClassificationType = Session.Get<OrganisationClassificationType>(_organisationRegistryConfiguration.KboV2LegalFormOrganisationClassificationTypeId);

            var organisation = Session.Get<Organisation>(message.OrganisationId);

            var kboOrganisation =
                _kboOrganisationRetriever.RetrieveOrganisation(message.User, organisation.KboNumber).GetAwaiter().GetResult();

            var location = GetOrAddLocations(kboOrganisation.Address);

            Session.Commit();

            // IMPORTANT: Need to re-Get the organisation, otherwise the Session will not properly handle the events.
            organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.UpdateInfoFromKbo(kboOrganisation.FormalName.Value, kboOrganisation.ShortName.Value);

            organisation.UpdateKboRegisteredOfficeLocations(location, registeredOfficeLocationType, message.ModificationTime);

            organisation.UpdateKboFormalNameLabel(kboOrganisation.FormalName, formalNameLabelType);

            organisation.UpdateKboLegalFormOrganisationClassification(
                _organisationClassificationRetriever,
                legalFormOrganisationClassificationType,
                kboOrganisation.LegalForm,
                guid => Session.Get<OrganisationClassification>(guid),
                message.ModificationTime);

            organisation.UpdateKboBankAccount(kboOrganisation.BankAccounts, message.ModificationTime);

            Session.Commit();
        }

        private KboRegisteredOffice GetOrAddLocations(IMagdaAddress address)
        {
            if (address == null)
                return null;

            var location = AddOrGetLocation(address);
            return new KboRegisteredOffice(location, address.ValidFrom, address.ValidTo);
        }

        private Location AddOrGetLocation(IMagdaAddress address)
        {
            var existingLocationId = _locationRetriever.RetrieveLocation(address);
            Location location;
            if (!existingLocationId.HasValue)
            {
                location = new Location(new LocationId(Guid.NewGuid()), null, new Address(
                    address.Street, address.ZipCode, address.City, address.Country));

                Session.Add(location);
            }
            else
            {
                location = Session.Get<Location>(existingLocationId.Value);
            }

            return location;
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
                kboOrganisation.FormalName.Value,
                new Period(
                    new ValidFrom(kboOrganisation.ValidFrom),
                    new ValidTo()));
        }

        private void AddAddresses(Organisation organisation,
            KboRegisteredOffice address,
            LocationType registeredOfficeLocationType)
        {
            if (address == null)
                return;

            organisation.AddKboRegisteredOfficeLocation(
                Guid.NewGuid(),
                address.Location,
                registeredOfficeLocationType,
                new Period(
                    new ValidFrom(address.ValidFrom),
                    new ValidTo(address.ValidTo)),
                _dateTimeProvider);
        }

        private void AddLegalForm(Organisation organisation, IMagdaLegalForm legalForm,
            OrganisationClassificationType legalFormOrganisationClassificationType)
        {
            if (legalForm == null)
                return;

            var organisationClassificationId =
                _organisationClassificationRetriever
                    .FetchOrganisationClassificationForLegalFormCode(legalForm.Code);

            if (organisationClassificationId == null)
                return;

            var organisationClassification =
                Session.Get<OrganisationClassification>(organisationClassificationId.Value);

            organisation.AddKboLegalFormOrganisationClassification(
                Guid.NewGuid(),
                legalFormOrganisationClassificationType,
                organisationClassification,
                new Period(
                    new ValidFrom(legalForm.ValidFrom),
                    new ValidTo()));
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

    public class KboRegisteredOffice
    {
        public Location Location { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public KboRegisteredOffice(Location location, DateTime? validFrom, DateTime? validTo)
        {
            Location = location;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }

}
